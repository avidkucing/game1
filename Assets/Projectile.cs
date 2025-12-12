using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1; // How much this bullet hurts
    public float lifetime = 2f; // Auto-destroy after 2 seconds

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move UP relative to the screen
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object we hit has the tag "Enemy"
        if (other.CompareTag("Enemy"))
        {
            // 1. Get the Enemy script component
            EnemyController enemy = other.GetComponent<EnemyController>();

            // 2. If the script exists, deal damage
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // 3. Destroy the bullet (it hit something)
            Destroy(gameObject);
        }
    }
}