using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager GetInstance()
    {
        return instance;
    }

    [Header("Sub Managers")]
    [SerializeField] private StartMenuSoundManager startMenu;
    [SerializeField] private GamePlaySoundManager gamePlay;
    [SerializeField] private GameOverSoundManager gameOver;

    [Header("UI")]
    [SerializeField] private Sprite soundToggleIcon;
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;

    private AudioSource engineSource;
    private AudioSource sfxSource;
    private AudioSource musicSource;
    private bool isEnginePlaying;
    private RectTransform soundControlsRect;
    private GameObject volumeSliderPanel;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        if (startMenu == null)
            startMenu = GetComponentInChildren<StartMenuSoundManager>(true);
        if (gamePlay == null)
            gamePlay = GetComponentInChildren<GamePlaySoundManager>(true);
        if (gameOver == null)
            gameOver = GetComponentInChildren<GameOverSoundManager>(true);

        SetupEngineSource();
        SetupSfxSource();
        SetupMusicSource();
        ApplyMasterVolume();
    }

    void OnEnable()
    {
        if (GameManager.GetInstance() != null)
        {
            GameManager.GetInstance().OnGameStart.AddListener(OnGameStarted);
            GameManager.GetInstance().OnGameEnd.AddListener(OnGameEnded);
        }
    }

    void OnDisable()
    {
        if (GameManager.GetInstance() != null)
        {
            GameManager.GetInstance().OnGameStart.RemoveListener(OnGameStarted);
            GameManager.GetInstance().OnGameEnd.RemoveListener(OnGameEnded);
        }

        StopEngine();
        StopMusic();
    }

    void Start()
    {
        BuildSoundControls();
    }

    void OnGameStarted()
    {
        PositionSoundControls(true);
        HideVolumeSlider();
    }

    void OnGameEnded()
    {
        PositionSoundControls(false);
        HideVolumeSlider();
    }

    public void PlayBigNuke()
    {
        gamePlay?.PlayBigNuke();
    }

    public void PlayPlayerFireWeapon()
    {
        gamePlay?.PlayPlayerFireWeapon();
    }

    public void PlayShieldPickup()
    {
        gamePlay?.PlayShieldPickup();
    }

    public void PlayNukePickup()
    {
        gamePlay?.PlayNukePickup();
    }

    public void PlayGunPickup()
    {
        gamePlay?.PlayGunPickup();
    }

    public void PlayHealthPickup()
    {
        gamePlay?.PlayHealthPickup();
    }

    public void PlayMissileLaunchDetected()
    {
        gamePlay?.PlayMissileLaunchDetected();
    }

    public void PlayEnemyDying()
    {
        gamePlay?.PlayEnemyDying();
    }

    public void PlayButtonHover()
    {
        startMenu?.PlayButtonHover();
    }

    public void PlayButtonClick()
    {
        startMenu?.PlayButtonClick();
    }

    public void PlaySfx(AudioClip clip, float volume)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayMusic(AudioClip clip, float volume, bool loop)
    {
        if (musicSource == null || clip == null)
            return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null)
            return;

        musicSource.Stop();
    }

    public void StartEngineLoop(AudioClip clip, float volume)
    {
        if (isEnginePlaying || engineSource == null || clip == null)
            return;

        engineSource.clip = clip;
        engineSource.volume = volume;
        engineSource.Play();
        isEnginePlaying = true;
    }

    public void StopEngine()
    {
        if (!isEnginePlaying || engineSource == null)
            return;

        engineSource.Stop();
        isEnginePlaying = false;
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

    void SetupEngineSource()
    {
        engineSource = gameObject.AddComponent<AudioSource>();
        engineSource.playOnAwake = false;
        engineSource.loop = true;
        engineSource.spatialBlend = 0f;
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

    void HideVolumeSlider()
    {
        if (volumeSliderPanel != null)
            volumeSliderPanel.SetActive(false);
    }

    void PositionSoundControls(bool gameplay)
    {
        if (soundControlsRect == null)
            return;

        soundControlsRect.anchoredPosition = gameplay
            ? new Vector2(-24f, -120f)
            : new Vector2(-24f, -16f);
    }
}
