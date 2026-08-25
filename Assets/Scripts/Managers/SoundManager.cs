using UnityEngine;
using UnityEngine.UI;
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
        BindButtonSounds();
    }

    void Update()
    {
        bool shouldPlayEngine = IsGameplayActive() && IsMovementKeyHeld();

        if (shouldPlayEngine)
            StartEngine();
        else
            StopEngine();

        if (IsGameplayActive() && Input.GetMouseButtonDown(0))
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

        NukeBehavior nukeBehavior = player.GetPickupBehaviorController().GetNukeBehavior();
        nukeBehavior.OnUseNuke += PlayBigNuke;
        nukeBehavior.OnCollectNuke += PlayNukePickup;
        player.GetPickupBehaviorController().GetGunPowerUpBehavior().OnPowerUpTimerChange += OnGunPowerUpTimerChange;

        wasGunPowerUpActive = player.GetPickupBehaviorController().HasGunPowerUp();
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
            NukeBehavior nukeBehavior = subscribedPlayer.GetPickupBehaviorController().GetNukeBehavior();
            nukeBehavior.OnUseNuke -= PlayBigNuke;
            nukeBehavior.OnCollectNuke -= PlayNukePickup;
            subscribedPlayer.GetPickupBehaviorController().GetGunPowerUpBehavior().OnPowerUpTimerChange -= OnGunPowerUpTimerChange;
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

    void BindButtonSounds()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            if (button.GetComponent<UIButtonSound>() == null)
                button.gameObject.AddComponent<UIButtonSound>();
        }
    }
}
