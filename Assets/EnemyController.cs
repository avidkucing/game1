using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float speed = 3.0f;
    public int maxHealth = 3;
    private int currentHealth;
    public float scoreValue = 100f;

    [Header("Loot Settings")]
    public GameObject powerUpPrefab;
    // The static dropChance is now handled dynamically by Luck logic

    [Header("VFX")]
    public GameObject explosionPrefab;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);

        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        TryDropLoot();

        Destroy(gameObject);
    }

    void TryDropLoot()
    {
        if (powerUpPrefab == null || GameManager.Instance == null) return;

        // --- LUCK & SUSTAINABILITY LOGIC ---
        
        int luckLevel = GameManager.Instance.currentLuckLevel; // Assume range 1-5
        int dropsThisRun = GameManager.Instance.totalPowerUpsDropped;

        // 1. Calculate Decay Rate: Each Luck point reduces base 50% decay by 10%
        float baseDecay = 0.5f;
        float decayReduction = (luckLevel - 1) * 0.1f; 
        float actualDecayRate = Mathf.Max(0f, baseDecay - decayReduction);

        // 2. Calculate Current Drop Probability: Multiplicative decay per drop
        // First drop (dropsThisRun = 0) is always 100%
        float currentProbability = Mathf.Pow(actualDecayRate, dropsThisRun);

        // 3. Calculate Drop Floor: Base 0.01% increased by 20% compound per Luck level
        float baseFloor = 0.0001f;
        float actualFloor = baseFloor * Mathf.Pow(1.2f, luckLevel - 1);

        // Final Probability Check
        float finalChance = Mathf.Max(currentProbability, actualFloor);

        if (Random.value <= finalChance)
        {
            Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
            GameManager.Instance.totalPowerUpsDropped++; // Increment run-wide drop counter
        }
    }
}