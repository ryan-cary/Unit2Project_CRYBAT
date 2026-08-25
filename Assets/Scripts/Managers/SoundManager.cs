using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager GetInstance()
    {
        return instance;
    }

    [Header("Movement")]
    [SerializeField] private AudioClip spaceshipEngineLightClip;
    [SerializeField, Range(0f, 1f)] private float engineVolume = 0.45f;

    [Header("Player")]
    [SerializeField] private AudioClip bigNukeClip;
    [SerializeField, Range(0f, 1f)] private float bigNukeVolume = 0.85f;
    [SerializeField] private AudioClip playerFireWeaponClip;
    [SerializeField, Range(0f, 1f)] private float playerFireWeaponVolume = 0.6f;
    [SerializeField] private AudioClip shieldPickupClip;
    [SerializeField, Range(0f, 1f)] private float shieldPickupVolume = 0.8f;
    [SerializeField] private AudioClip nukePickupClip;
    [SerializeField, Range(0f, 1f)] private float nukePickupVolume = 0.8f;
    [SerializeField] private AudioClip gunPickupClip;
    [SerializeField, Range(0f, 1f)] private float gunPickupVolume = 0.8f;
    [SerializeField] private AudioClip healthPickupClip;
    [SerializeField, Range(0f, 1f)] private float healthPickupVolume = 0.8f;

    [Header("Enemies")]
    [SerializeField] private AudioClip missileLaunchDetectedClip;
    [SerializeField, Range(0f, 1f)] private float missileLaunchDetectedVolume = 0.8f;
    [SerializeField] private AudioClip enemyDyingClip;
    [SerializeField, Range(0f, 1f)] private float enemyDyingVolume = 0.75f;

    [Header("Music")]
    [SerializeField] private AudioClip cosmicFuryClip;
    [SerializeField, Range(0f, 1f)] private float cosmicFuryVolume = 0.4f;
    [SerializeField] private AudioClip startMenuClip;
    [SerializeField, Range(0f, 1f)] private float startMenuVolume = 0.4f;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField, Range(0f, 1f)] private float gameOverVolume = 0.4f;

    [Header("UI")]
    [SerializeField] private AudioClip buttonHoverClip;
    [SerializeField, Range(0f, 1f)] private float buttonHoverVolume = 0.7f;
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField, Range(0f, 1f)] private float buttonClickVolume = 0.7f;
    [SerializeField] private Sprite soundToggleIcon;
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;

    private AudioSource engineSource;
    private AudioSource sfxSource;
    private AudioSource musicSource;
    private bool isEnginePlaying;
    private bool isSubscribedToPlayerEvents;
    private Player subscribedPlayer;
    private bool wasGunPowerUpActive;
    private float lastGunPowerUpRemaining;
    private int lastShieldCount;
    private readonly List<TrackedHealthPickup> trackedHealthPickups = new List<TrackedHealthPickup>();
    private RectTransform soundControlsRect;
    private GameObject volumeSliderPanel;

    struct TrackedHealthPickup
    {
        public HealthPickup pickup;
        public Vector3 position;
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        SetupEngineSource();
        SetupSfxSource();
        SetupMusicSource();
        ApplyMasterVolume();
    }

    void OnEnable()
    {
        if (GameManager.GetInstance() != null)
        {
            GameManager.GetInstance().OnGameStart.AddListener(StartGameplayMusic);
            GameManager.GetInstance().OnGameEnd.AddListener(OnGameEnded);
        }
    }

    void OnDisable()
    {
        if (GameManager.GetInstance() != null)
        {
            GameManager.GetInstance().OnGameStart.RemoveListener(StartGameplayMusic);
            GameManager.GetInstance().OnGameEnd.RemoveListener(OnGameEnded);
        }

        UnsubscribeFromPlayerEvents();
        StopEngine();
        StopMusic();
    }

    void Start()
    {
        PlayStartMenuMusic();
        BuildSoundControls();
        BindButtonSounds();
    }

    void Update()
    {
        bool shouldPlayEngine = IsGameplayActive() && IsMovementKeyHeld();

        if (shouldPlayEngine)
            StartEngine();
        else
            StopEngine();

        if (IsGameplayActive() && Input.GetMouseButtonDown(0) && !IsPointerOverUi())
            PlayPlayerFireWeapon();

        if (!isSubscribedToPlayerEvents)
            SubscribeToPlayerEvents();

        CheckShieldPickup();
        CheckHealthPickups();
    }

    void SetupEngineSource()
    {
        engineSource = gameObject.AddComponent<AudioSource>();
        engineSource.playOnAwake = false;
        engineSource.loop = true;
        engineSource.spatialBlend = 0f;
        engineSource.clip = spaceshipEngineLightClip;
        engineSource.volume = engineVolume;
    }

    void SetupSfxSource()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.spatialBlend = 0f;
    }

    void SetupMusicSource()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = false;
        musicSource.spatialBlend = 0f;
    }

    void SubscribeToPlayerEvents()
    {
        Player player = GameManager.GetInstance()?.GetPlayer();
        if (player == null)
            return;

        NukeBehavior nukeBehavior = player.GetNukeBehavior();
        nukeBehavior.OnUseNuke += PlayBigNuke;
        nukeBehavior.OnCollectNuke += PlayNukePickup;
        player.GetGunPowerUpBehavior().OnPowerUpTimerChange += OnGunPowerUpTimerChange;

        wasGunPowerUpActive = player.HasGunPowerUp();
        lastGunPowerUpRemaining = 0f;
        lastShieldCount = player.GetComponentsInChildren<Shield>(true).Length;
        trackedHealthPickups.Clear();
        subscribedPlayer = player;
        isSubscribedToPlayerEvents = true;
    }

    void UnsubscribeFromPlayerEvents()
    {
        if (subscribedPlayer != null)
        {
            NukeBehavior nukeBehavior = subscribedPlayer.GetNukeBehavior();
            nukeBehavior.OnUseNuke -= PlayBigNuke;
            nukeBehavior.OnCollectNuke -= PlayNukePickup;
            subscribedPlayer.GetGunPowerUpBehavior().OnPowerUpTimerChange -= OnGunPowerUpTimerChange;
        }

        subscribedPlayer = null;
        trackedHealthPickups.Clear();
        isSubscribedToPlayerEvents = false;
    }

    void OnGunPowerUpTimerChange(bool isActive, float remainingTime, Vector2 screenPosition)
    {
        bool collected = isActive && remainingTime == 0f
            && (!wasGunPowerUpActive || lastGunPowerUpRemaining > 0.25f);

        if (collected)
            PlayGunPickup();

        wasGunPowerUpActive = isActive;
        lastGunPowerUpRemaining = remainingTime;
    }

    void CheckHealthPickups()
    {
        if (subscribedPlayer == null || !IsGameplayActive())
        {
            trackedHealthPickups.Clear();
            return;
        }

        Vector3 playerPosition = subscribedPlayer.transform.position;
        const float collectDistance = 5f;

        for (int i = 0; i < trackedHealthPickups.Count; i++)
        {
            TrackedHealthPickup tracked = trackedHealthPickups[i];
            if (tracked.pickup == null && Vector2.Distance(playerPosition, tracked.position) <= collectDistance)
                PlayHealthPickup();
        }

        trackedHealthPickups.Clear();
        HealthPickup[] pickups = FindObjectsByType<HealthPickup>(FindObjectsSortMode.None);
        for (int i = 0; i < pickups.Length; i++)
        {
            TrackedHealthPickup tracked;
            tracked.pickup = pickups[i];
            tracked.position = pickups[i].transform.position;
            trackedHealthPickups.Add(tracked);
        }
    }

    void CheckShieldPickup()
    {
        if (subscribedPlayer == null)
            return;

        int shieldCount = subscribedPlayer.GetComponentsInChildren<Shield>(true).Length;
        if (shieldCount > lastShieldCount)
            PlayShieldPickup();

        lastShieldCount = shieldCount;
    }

    void OnGameEnded()
    {
        UnsubscribeFromPlayerEvents();
        StopEngine();
        PlayGameOverMusic();
        PositionSoundControls(false);
        if (volumeSliderPanel != null)
            volumeSliderPanel.SetActive(false);
    }

    public void PlayBigNuke()
    {
        PlaySfx(bigNukeClip, bigNukeVolume);
    }

    public void PlayPlayerFireWeapon()
    {
        PlaySfx(playerFireWeaponClip, playerFireWeaponVolume);
    }

    public void PlayShieldPickup()
    {
        PlaySfx(shieldPickupClip, shieldPickupVolume);
    }

    public void PlayNukePickup()
    {
        PlaySfx(nukePickupClip, nukePickupVolume);
    }

    public void PlayGunPickup()
    {
        PlaySfx(gunPickupClip, gunPickupVolume);
    }

    public void PlayHealthPickup()
    {
        PlaySfx(healthPickupClip, healthPickupVolume);
    }

    public void PlayMissileLaunchDetected()
    {
        PlaySfx(missileLaunchDetectedClip, missileLaunchDetectedVolume);
    }

    public void PlayEnemyDying()
    {
        PlaySfx(enemyDyingClip, enemyDyingVolume);
    }

    public void PlayButtonHover()
    {
        PlaySfx(buttonHoverClip, buttonHoverVolume);
    }

    public void PlayButtonClick()
    {
        PlaySfx(buttonClickClip, buttonClickVolume);
    }

    void PlaySfx(AudioClip clip, float volume)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip, volume);
    }

    bool IsMovementKeyHeld()
    {
        return Input.GetKey(KeyCode.W)
            || Input.GetKey(KeyCode.A)
            || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.D);
    }

    bool IsGameplayActive()
    {
        return GameManager.GetInstance() != null && GameManager.GetInstance().IsGameInProgress();
    }

    void StartEngine()
    {
        if (isEnginePlaying || engineSource == null || engineSource.clip == null)
            return;

        engineSource.volume = engineVolume;
        engineSource.Play();
        isEnginePlaying = true;
    }

    void StopEngine()
    {
        if (!isEnginePlaying || engineSource == null)
            return;

        engineSource.Stop();
        isEnginePlaying = false;
    }

    void PlayStartMenuMusic()
    {
        PlayMusic(startMenuClip, startMenuVolume, true);
    }

    void StartGameplayMusic()
    {
        PlayMusic(cosmicFuryClip, cosmicFuryVolume, true);
        PositionSoundControls(true);
        if (volumeSliderPanel != null)
            volumeSliderPanel.SetActive(false);
    }

    void PlayGameOverMusic()
    {
        PlayMusic(gameOverClip, gameOverVolume, false);
    }

    void PlayMusic(AudioClip clip, float volume, bool loop)
    {
        if (musicSource == null || clip == null)
            return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.loop = loop;
        musicSource.Play();
    }

    void StopMusic()
    {
        if (musicSource == null)
            return;

        musicSource.Stop();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyMasterVolume();
    }

    void ApplyMasterVolume()
    {
        AudioListener.volume = masterVolume;
    }

    bool IsPointerOverUi()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    void BuildSoundControls()
    {
        Canvas canvas = GameObject.Find("Canvas")?.GetComponent<Canvas>();
        if (canvas == null)
            return;

        GameObject root = new GameObject("SoundControls", typeof(RectTransform));
        root.layer = 5;
        root.transform.SetParent(canvas.transform, false);
        root.transform.SetAsLastSibling();

        soundControlsRect = root.GetComponent<RectTransform>();
        soundControlsRect.anchorMin = new Vector2(1f, 1f);
        soundControlsRect.anchorMax = new Vector2(1f, 1f);
        soundControlsRect.pivot = new Vector2(1f, 1f);
        soundControlsRect.sizeDelta = new Vector2(56f, 56f);
        PositionSoundControls(false);

        Button soundToggleButton = CreateIconButton(root.transform, "SoundToggleButton", soundToggleIcon);
        RectTransform buttonRect = soundToggleButton.GetComponent<RectTransform>();
        buttonRect.anchorMin = Vector2.zero;
        buttonRect.anchorMax = Vector2.one;
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;

        soundToggleButton.onClick.AddListener(ToggleVolumeSlider);

        volumeSliderPanel = CreateVolumeSlider(root.transform);
        volumeSliderPanel.SetActive(false);
    }

    Button CreateIconButton(Transform parent, string name, Sprite icon)
    {
        GameObject buttonObject = new GameObject(name, typeof(RectTransform));
        buttonObject.layer = 5;
        buttonObject.transform.SetParent(parent, false);

        LayoutElement layoutElement = buttonObject.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = 56f;
        layoutElement.preferredHeight = 56f;
        layoutElement.minWidth = 56f;
        layoutElement.minHeight = 56f;
        layoutElement.ignoreLayout = true;

        Image image = buttonObject.AddComponent<Image>();
        image.sprite = icon;
        image.preserveAspect = true;
        image.color = new Color(1f, 1f, 1f, 0.55f);
        image.raycastTarget = true;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(1f, 1f, 1f, 1f);
        colors.highlightedColor = new Color(1f, 1f, 1f, 1f);
        colors.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        colors.selectedColor = new Color(1f, 1f, 1f, 1f);
        button.colors = colors;
        return button;
    }

    GameObject CreateVolumeSlider(Transform parent)
    {
        GameObject sliderObject = new GameObject("VolumeSlider", typeof(RectTransform));
        sliderObject.layer = 5;
        sliderObject.transform.SetParent(parent, false);

        RectTransform sliderRect = sliderObject.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.5f, 0f);
        sliderRect.anchorMax = new Vector2(0.5f, 0f);
        sliderRect.pivot = new Vector2(0.5f, 1f);
        sliderRect.anchoredPosition = new Vector2(0f, -8f);
        sliderRect.sizeDelta = new Vector2(28f, 140f);

        Image background = sliderObject.AddComponent<Image>();
        background.color = new Color(0f, 0f, 0f, 0.45f);

        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.layer = 5;
        fillArea.transform.SetParent(sliderObject.transform, false);
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0.25f, 0f);
        fillAreaRect.anchorMax = new Vector2(0.75f, 1f);
        fillAreaRect.offsetMin = new Vector2(0f, 10f);
        fillAreaRect.offsetMax = new Vector2(0f, -10f);

        GameObject fill = new GameObject("Fill", typeof(RectTransform));
        fill.layer = 5;
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(1f, 1f, 1f, 0.8f);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        GameObject handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleArea.layer = 5;
        handleArea.transform.SetParent(sliderObject.transform, false);
        RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(0f, 10f);
        handleAreaRect.offsetMax = new Vector2(0f, -10f);

        GameObject handle = new GameObject("Handle", typeof(RectTransform));
        handle.layer = 5;
        handle.transform.SetParent(handleArea.transform, false);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = new Color(1f, 1f, 1f, 0.9f);
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20f, 20f);

        Slider slider = sliderObject.AddComponent<Slider>();
        slider.direction = Slider.Direction.BottomToTop;
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = masterVolume;
        slider.onValueChanged.AddListener(SetMasterVolume);
        return sliderObject;
    }

    void ToggleVolumeSlider()
    {
        if (volumeSliderPanel == null)
            return;

        volumeSliderPanel.SetActive(!volumeSliderPanel.activeSelf);
    }

    void PositionSoundControls(bool gameplay)
    {
        if (soundControlsRect == null)
            return;

        soundControlsRect.anchoredPosition = gameplay
            ? new Vector2(-24f, -120f)
            : new Vector2(-24f, -16f);
    }

    void BindButtonSounds()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            if (button.gameObject.name == "SoundToggleButton")
                continue;

            if (button.GetComponent<UIButtonSound>() == null)
                button.gameObject.AddComponent<UIButtonSound>();
        }
    }
}
