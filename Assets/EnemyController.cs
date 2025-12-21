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
        if (powerUpPrefab != null && GameManager.Instance != null)
        {
            float chance = GameManager.Instance.GetCurrentDropChance();
            
            if (Random.value <= chance)
            {
                Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
                GameManager.Instance.dropsInCurrentRun++; // Increment the run counter
            }
        }
    }
}