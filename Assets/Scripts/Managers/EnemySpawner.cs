using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : MonoBehaviour, IDifficultyOverridden
{
	
	[SerializeField] private GameObject[] enemyPrefabs;
	private Dictionary<EnemyType, GameObject> enemyTypeToPrefab = new Dictionary<EnemyType, GameObject>();
	
	[SerializeField] private float enemySpawnRate = 7f;
    [SerializeField] private float spawnRadius = 10f;
	
	private bool doSpawnEnemies = false;
	
	public UnityEvent<Enemy> OnEnemyDefeated;
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {		
        SetEnemyPrefabDictionary();
		GameManager.GetInstance().OnGameStart.AddListener(StartSpawning);
		GameManager.GetInstance().OnGameEnd.AddListener(StopSpawning);
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
	
	private void StartSpawning()
	{
		doSpawnEnemies = true;
		StartCoroutine(EnemySpawnRoutine());
	}
	
	private void StopSpawning()
	{
		doSpawnEnemies = false;
	}
	
	private IEnumerator EnemySpawnRoutine()
    {
        while (doSpawnEnemies)
        {
            yield return new WaitForSeconds(enemySpawnRate);
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
		//determine what to spawn
        int randomEnemyIndex = Random.Range(0, enemyTypeToPrefab.Count);
        GameObject randomEnemyPrefab = enemyTypeToPrefab.ElementAt(randomEnemyIndex).Value;
		
		//spawning
        Vector2 spawnPosition = Random.insideUnitCircle.normalized * spawnRadius;
        Instantiate(randomEnemyPrefab, spawnPosition, Quaternion.identity);
    }
	
	// ====== Difficulty ======//
	public void DifficultyOverride(DifficultySetting difficulty)
	{
		this.enemySpawnRate = difficulty.enemySpawnRate;
	}
}
