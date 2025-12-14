using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Stats")]
    public int maxHealth = 5;
    private int currentHealth;
    
    private Camera mainCamera;
    private AutoShooter myShooter; // Reference to our gun

    void Start()
    {
        mainCamera = Camera.main;
        currentHealth = maxHealth;
        
        // Get the AutoShooter component attached to this same GameObject
        myShooter = GetComponent<AutoShooter>();

        if (UIController.Instance != null) {
            UIController.Instance.InitializeHealth(currentHealth, maxHealth);
        }
    }

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.isPressed)
        {
            Vector2 inputPos = Pointer.current.position.ReadValue();
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(inputPos.x, inputPos.y, 10f));
            worldPos.z = 0;
            transform.position = worldPos;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
        // NEW: Check for PowerUp collision
        else if (other.CompareTag("PowerUp"))
        {
            CollectPowerUp();
            Destroy(other.gameObject); // Remove the item from screen
        }
    }

    void CollectPowerUp()
    {
        Debug.Log("PowerUp Collected!");
        if (myShooter != null)
        {
            myShooter.UpgradeWeapon();
        }
    }

    void TakeDamage(int amount)
    {
        currentHealth -= amount;
        
        if (UIController.Instance != null) { 
            UIController.Instance.UpdateHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        Destroy(gameObject);
    }
}