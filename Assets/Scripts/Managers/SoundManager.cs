using UnityEngine;

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

    [Header("Enemies")]
    [SerializeField] private AudioClip missileLaunchDetectedClip;
    [SerializeField, Range(0f, 1f)] private float missileLaunchDetectedVolume = 0.8f;

    private AudioSource engineSource;
    private AudioSource sfxSource;
    private bool isEnginePlaying;

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
    }

    void OnEnable()
    {
        if (GameManager.GetInstance() != null)
            GameManager.GetInstance().OnGameEnd.AddListener(StopEngine);
    }

    void OnDisable()
    {
        if (GameManager.GetInstance() != null)
            GameManager.GetInstance().OnGameEnd.RemoveListener(StopEngine);

        StopEngine();
    }

    void Update()
    {
        bool shouldPlayEngine = IsGameplayActive() && IsMovementKeyHeld();

        if (shouldPlayEngine)
            StartEngine();
        else
            StopEngine();
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

    public void PlayMissileLaunchDetected()
    {
        if (sfxSource == null || missileLaunchDetectedClip == null)
            return;

        sfxSource.PlayOneShot(missileLaunchDetectedClip, missileLaunchDetectedVolume);
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
}
