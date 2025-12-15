using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;
    
    [Tooltip("Starting time between spawns")]
    public float initialSpawnInterval = 2.0f;
    
    [Tooltip("The fastest enemies will ever spawn")]
    public float minSpawnInterval = 0.5f;
    
    [Tooltip("How much the interval decreases per second of gameplay")]
    public float difficultyRampRate = 0.01f; 

    [Header("Spawn Area")]
    public float xMin = -2.5f; 
    public float xMax = 2.5f;
    public float spawnY = 6.0f;

    private float _currentSpawnInterval;
    private float _timer;

    void Start()
    {
        _currentSpawnInterval = initialSpawnInterval;
    }

    void Update()
    {
        // 1. Difficulty Ramping
        // Every frame, make the spawn interval slightly smaller, down to the minimum
        if (_currentSpawnInterval > minSpawnInterval)
        {
            _currentSpawnInterval -= difficultyRampRate * Time.deltaTime;
        }

        // 2. Spawning Logic
        _timer += Time.deltaTime;

        if (_timer >= _currentSpawnInterval)
        {
            SpawnEnemy();
            _timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        float randomX = Random.Range(xMin, xMax);
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0);
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}