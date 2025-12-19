using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1; // How much this bullet hurts
    public float lifetime = 2f; // Auto-destroy after 2 seconds
    public GameObject impactEffectPrefab;

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
            if (impactEffectPrefab != null)
            {
                Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);
            }
            
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}