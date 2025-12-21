using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic; // Ensure this is at the top

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Player Stats")]
    public int maxHealth = 5;
    private int currentHealth;
    
    private Camera mainCamera;
    private AutoShooter myShooter; // Reference to our gun

    [Header("Defensive Components")]
    // Tracks current levels for the 6 defensive types
    public Dictionary<string, int> defenseLevels = new Dictionary<string, int>()
    {
        { "HP", 0 }, // Max HP
        { "RG", 0 }, // Regen
        { "AR", 0 }, // Reflexive Armor
        { "MG", 0 }, // Magnet
        { "LK", 0 }, // Luck
        { "TM", 0 }  // Time Machine
    };

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
        else if (other.CompareTag("PowerUp"))
        {
            // OLD: Directly upgraded weapon
            // NEW: Trigger the Choice UI
            OpenUpgradeUI(); 
            Destroy(other.gameObject);
        }
    }

    void OpenUpgradeUI()
    {
        // Pause the game
        Time.timeScale = 0f;
        
        // Get choices from UpgradeManager
        var choices = UpgradeManager.Instance.GetUpgradeChoices();
        
        // Tell UIController to show these choices
        UIController.Instance.ShowUpgradePanel(choices);
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