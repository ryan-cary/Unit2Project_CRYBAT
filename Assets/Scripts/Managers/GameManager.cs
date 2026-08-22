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
    [SerializeField] private GameObject[] enemyPrefabs;

    private Player player;
    private Dictionary<EnemyType, GameObject> enemyTypeToPrefab = new Dictionary<EnemyType, GameObject>();

    // Actions
    public UnityEvent OnGameStart;
    public UnityEvent OnGameEnd;

    // References
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private PickupSpawner pickupSpawner;

    // Game data
    [SerializeField] private float bulletLifetime;
    [SerializeField] private float enemySpawnRate = 7f;
    [SerializeField] private float spawnRadius = 10f;

    // Game State
    bool isGameInProgress = false;
    bool shouldEnemiesSpawn = false;

    /*
    TODO
    main menu
    game over screen
    event for game start and stop
    a way to clean up the game when it's over
    */

    void Awake()
    {
        SetSingleton();
        SetEnemyPrefabDictionary();
    }

    void SetEnemyPrefabDictionary()
    {
        foreach (GameObject enemyPrefab in enemyPrefabs)
        {
            if (enemyPrefab != null)
            {
                Enemy enemyScript = enemyPrefab.GetComponent<Enemy>();

                if (enemyScript != null)
                {
                    EnemyType type = enemyScript.GetEnemyType();
                    enemyTypeToPrefab[type] = enemyPrefab; 
                }
            }
        }
    }

    public void StartGame()
    {
        isGameInProgress = true;
        scoreManager.ResetScore();
        player = Instantiate(playerPrefab).GetComponent<Player>();
        player.OnDefeated.AddListener(StopGame);
        OnGameStart?.Invoke();
        StartCoroutine(GameStartRoutine());
    }

    public void StopGame()
    {
        scoreManager.SaveHighScore();
        OnGameEnd?.Invoke();
        shouldEnemiesSpawn = false;
        StartCoroutine(GameStopRoutine());
    }

    private IEnumerator GameStartRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        shouldEnemiesSpawn = true;
        SpawnEnemy();
        StartCoroutine(EnemySpawnRoutine());
        
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
        foreach (NukeBlast nukeBlast in FindObjectsByType<NukeBlast>(FindObjectsSortMode.None))
        {
            Destroy(nukeBlast.gameObject);
        }
    }

    private IEnumerator EnemySpawnRoutine()
    {
        while (shouldEnemiesSpawn)
        {
            yield return new WaitForSeconds(enemySpawnRate);
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        int randomEnemyIndex = Random.Range(0, enemyTypeToPrefab.Count);
        GameObject randomEnemyPrefab = enemyTypeToPrefab.ElementAt(randomEnemyIndex).Value;
        Vector2 spawnPosition = Random.insideUnitCircle.normalized * spawnRadius;
        Instantiate(randomEnemyPrefab, spawnPosition, Quaternion.identity);
    }

    public void OnEnemyDefeated(Enemy enemy)
    {
        pickupSpawner.SpawnPickup(enemy.transform.position);
    }

    public Player GetPlayer()
    {
        return player;
    }

    public ScoreManager GetScoreManager()
    {
        return scoreManager;
    }

    public float GetBulletLifetime()
    {
        return bulletLifetime;
    }

    public bool IsGameInProgress()
    {
        return isGameInProgress;
    }

    public bool ShouldEnemiesSpawn()
    {
        return shouldEnemiesSpawn;
    }
}
