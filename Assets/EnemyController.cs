using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float speed = 3.0f;
    public int maxHealth = 3; // Basic enemies take 3 hits
    private int currentHealth;
    public float scoreValue = 100f; // How much is this enemy worth?

    void Start()
    {
        // Initialize health when the enemy spawns
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

    // New Public Method: Allows bullets to call this function
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        
        // Debug log to see it working in the console
        Debug.Log("Enemy hit! HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Notify the GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        // Add Score logic here later!
        Destroy(gameObject);
    }
}