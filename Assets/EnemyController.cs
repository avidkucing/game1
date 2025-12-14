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
    [Range(0f, 1f)]
    public float dropChance = 0.2f; // 20% chance to drop

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
        // 1. Notify Score
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        // 2. Try to drop PowerUp
        TryDropLoot();

        // 3. Destroy Enemy
        Destroy(gameObject);
    }

    void TryDropLoot()
    {
        if (powerUpPrefab != null)
        {
            // Random.value returns a float between 0.0 and 1.0
            if (Random.value <= dropChance)
            {
                Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}