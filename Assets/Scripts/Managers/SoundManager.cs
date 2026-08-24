using UnityEngine;
using UnityEngine.UI;

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
        isSubscribedToPlayerEvents = true;
    }

    void UnsubscribeFromPlayerEvents()
    {
        Player player = GameManager.GetInstance()?.GetPlayer();
        if (player != null)
        {
            NukeBehavior nukeBehavior = player.GetNukeBehavior();
            nukeBehavior.OnUseNuke -= PlayBigNuke;
            nukeBehavior.OnCollectNuke -= PlayNukePickup;
        }

        isSubscribedToPlayerEvents = false;
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
