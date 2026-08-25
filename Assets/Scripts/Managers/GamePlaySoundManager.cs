using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GamePlaySoundManager : MonoBehaviour
{
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

    SoundManager soundManager;
    bool isSubscribedToPlayerEvents;
    Player subscribedPlayer;
    bool wasGunPowerUpActive;
    float lastGunPowerUpRemaining;
    int lastShieldCount;
    readonly List<TrackedHealthPickup> trackedHealthPickups = new List<TrackedHealthPickup>();

    struct TrackedHealthPickup
    {
        public HealthPickup pickup;
        public Vector3 position;
    }

    void Awake()
    {
        soundManager = GetComponentInParent<SoundManager>();
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
        soundManager?.StopEngine();
    }

    void Update()
    {
        bool shouldPlayEngine = IsGameplayActive() && IsMovementKeyHeld();

        if (shouldPlayEngine)
            StartEngine();
        else
            soundManager?.StopEngine();

        if (IsGameplayActive() && Input.GetMouseButtonDown(0) && !IsPointerOverUi())
            PlayPlayerFireWeapon();

        if (!isSubscribedToPlayerEvents)
            SubscribeToPlayerEvents();

        CheckShieldPickup();
        CheckHealthPickups();
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
        soundManager?.StopEngine();
    }

    void StartGameplayMusic()
    {
        soundManager?.PlayMusic(cosmicFuryClip, cosmicFuryVolume, true);
    }

    void StartEngine()
    {
        soundManager?.StartEngineLoop(spaceshipEngineLightClip, engineVolume);
    }

    public void PlayBigNuke()
    {
        soundManager?.PlaySfx(bigNukeClip, bigNukeVolume);
    }

    public void PlayPlayerFireWeapon()
    {
        soundManager?.PlaySfx(playerFireWeaponClip, playerFireWeaponVolume);
    }

    public void PlayShieldPickup()
    {
        soundManager?.PlaySfx(shieldPickupClip, shieldPickupVolume);
    }

    public void PlayNukePickup()
    {
        soundManager?.PlaySfx(nukePickupClip, nukePickupVolume);
    }

    public void PlayGunPickup()
    {
        soundManager?.PlaySfx(gunPickupClip, gunPickupVolume);
    }

    public void PlayHealthPickup()
    {
        soundManager?.PlaySfx(healthPickupClip, healthPickupVolume);
    }

    public void PlayMissileLaunchDetected()
    {
        soundManager?.PlaySfx(missileLaunchDetectedClip, missileLaunchDetectedVolume);
    }

    public void PlayEnemyDying()
    {
        soundManager?.PlaySfx(enemyDyingClip, enemyDyingVolume);
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

    bool IsPointerOverUi()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
