using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    [Header("Gun Settings")]
    public GameObject bulletPrefab; // The bullet template you just made
    public float fireInterval = 0.2f; // How fast it shoots (lower = faster)
    public Vector3 firePointOffset = new Vector3(0, 0.5f, 0); // Spawns bullet slightly ahead of ship

    private float timer;

    void Update()
    {
        // Simple Timer Logic
        timer += Time.deltaTime;

        if (timer >= fireInterval)
        {
            Shoot();
            timer = 0f;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null) return;

        // Create the bullet at Ship Position + Offset
        Instantiate(bulletPrefab, transform.position + firePointOffset, Quaternion.identity);
    }
}