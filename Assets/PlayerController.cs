using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Stats")]
    public int maxHealth = 5;
    private int currentHealth;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        currentHealth = maxHealth;

        // Update UI immediately
        if (UIController.Instance != null) {
            UIController.Instance.InitializeHealth(currentHealth, maxHealth);
        }
    }

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.isPressed)
        {
            Vector2 inputPos = Pointer.current.position.ReadValue();
            
            // 10f is the critical distance between Camera (-10) and Plane (0)
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(inputPos.x, inputPos.y, 10f));
            worldPos.z = 0;

            transform.position = worldPos;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Player takes collision damage (e.g., 1 damage)
            TakeDamage(1);

            // Destroy the enemy that crashed into us (Kamikaze)
            Destroy(other.gameObject);
        }
    }

    void TakeDamage(int amount)
    {
        currentHealth -= amount;
        
        // Update UI
        if (UIController.Instance != null) { // Make sure this matches your script name (GameUIController vs UIController)
            UIController.Instance.UpdateHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // 1. Trigger the Game Over Logic
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }

        // 2. Visual Effects (Optional: Spawn explosion prefab here later)
        // Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // 3. Remove the Player
        // We destroy the object so it stops moving/shooting, 
        // but since we called GameManager.GameOver() first, the UI will handle the rest.
        Destroy(gameObject);
    }
}