using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{

    // ------ Singleton Setup ------
    private static GameManager instance;
    public static GameManager GetInstance()
    {
        return instance;
    }

    void SetSingleton()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        instance = this;
    }

    // ------ End Singleton Setup ------

    [SerializeField] private GameObject playerPrefab;

    private Player player;

    // Actions
    public UnityEvent OnGameStart;
    public UnityEvent OnGameEnd;

    [Header("Manager References")]
    [SerializeField] private ScoreManager scoreManager;
	[SerializeField] private DifficultyManager difficultyManager;
    [SerializeField] private EnemySpawner enemySpawner;
	[SerializeField] private PickupSpawner pickupSpawner;
	

    [Header("Game Data")]
    [SerializeField] private float bulletLifetime;

    // Game State
    bool isGameInProgress = false;
    void Awake()
    {
        SetSingleton();
    }

    public void StartGame()
    {
        isGameInProgress = true;
        scoreManager.ResetScore();
        player = Instantiate(playerPrefab).GetComponent<Player>();
        player.OnDefeated.AddListener(StopGame);
        OnGameStart?.Invoke();
    }

    public void StopGame()
    {
        scoreManager.SaveHighScore();
        OnGameEnd?.Invoke();
        StartCoroutine(GameStopRoutine());
    }

    private IEnumerator GameStopRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        isGameInProgress = false;

        foreach (Enemy enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            Destroy(enemy.gameObject);
        }
        foreach (Pickup pickup in FindObjectsByType<Pickup>(FindObjectsSortMode.None))
        {
            Destroy(pickup.gameObject);
        }
        foreach (Bullet bullet in FindObjectsByType<Bullet>(FindObjectsSortMode.None))
        {
            Destroy(bullet.gameObject);
        }
        foreach (NukeBlast nukeBlast in FindObjectsByType<NukeBlast>(FindObjectsSortMode.None))
        {
            Destroy(nukeBlast.gameObject);
        }
    }
	
	public void OnEnemyDefeated(Enemy enemy)
    {
        pickupSpawner.SpawnPickup(enemy.transform.position);
        SoundManager.GetInstance()?.PlayEnemyDying();
    }

    public Player GetPlayer()
    {
        return player;
    }

    public ScoreManager GetScoreManager()
    {
        return scoreManager;
    }
	
	public DifficultyManager GetDifficultyManager()
    {
        return difficultyManager;
    }
	
	public EnemySpawner GetEnemySpawner()
    {
        return enemySpawner;
    }
	
	public PickupSpawner GetPickupSpawner()
    {
        return pickupSpawner;
    }

    public float GetBulletLifetime()
    {
        return bulletLifetime;
    }

    public bool IsGameInProgress()
    {
        return isGameInProgress;
    }
	
	public void QuitGame()
	{ Application.Quit(); }
}
