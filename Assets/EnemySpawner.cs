using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab; // Drag your enemy prefab here
    public float spawnInterval = 2.0f; // How often to spawn (seconds)
    
    [Header("Spawn Area")]
    // Adjust these X values to match your screen width limits
    public float xMin = -2.5f; 
    public float xMax = 2.5f;
    public float spawnY = 6.0f; // Just above the top of the screen

    private float _timer;

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= spawnInterval)
        {
            SpawnEnemy();
            _timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        // 1. Calculate a random X position within bounds
        float randomX = Random.Range(xMin, xMax);
        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0);

        // 2. Instantiate the enemy
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}