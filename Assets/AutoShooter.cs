using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    [Header("Gun Settings")]
    public GameObject bulletPrefab;
    public float fireInterval = 0.2f;
    public Vector3 firePointOffset = new Vector3(0, 0.5f, 0);

    [Header("Upgrade System")]
    public int currentWeaponLevel = 1;
    public int maxWeaponLevel = 3;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= fireInterval)
        {
            Shoot();
            timer = 0f;
        }
    }

    public void UpgradeWeapon()
    {
        if (currentWeaponLevel < maxWeaponLevel)
        {
            currentWeaponLevel++;
            Debug.Log("Weapon Upgraded to Level " + currentWeaponLevel);
            
            // Optional: Increase fire rate slightly on upgrade
            fireInterval = Mathf.Max(0.1f, fireInterval - 0.02f);
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null) return;

        Vector3 spawnPos = transform.position + firePointOffset;

        switch (currentWeaponLevel)
        {
            case 1:
                // Level 1: Single straight shot
                Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
                break;

            case 2:
                // Level 2: Double parallel shot (Left and Right)
                Vector3 leftPos = spawnPos + new Vector3(-0.2f, 0, 0);
                Vector3 rightPos = spawnPos + new Vector3(0.2f, 0, 0);
                Instantiate(bulletPrefab, leftPos, Quaternion.identity);
                Instantiate(bulletPrefab, rightPos, Quaternion.identity);
                break;

            case 3:
                // Level 3: Spread Shot (Left, Center, Right angled)
                // Center
                Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
                // Left Angle (-15 degrees)
                Instantiate(bulletPrefab, spawnPos, Quaternion.Euler(0, 0, 15f));
                // Right Angle (15 degrees)
                Instantiate(bulletPrefab, spawnPos, Quaternion.Euler(0, 0, -15f));
                break;

            default:
                // Fallback to Level 3 behavior if level is higher
                Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
                Instantiate(bulletPrefab, spawnPos, Quaternion.Euler(0, 0, 15f));
                Instantiate(bulletPrefab, spawnPos, Quaternion.Euler(0, 0, -15f));
                break;
        }
    }
}