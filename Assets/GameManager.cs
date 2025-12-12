using UnityEngine;
using UnityEngine.SceneManagement; // Needed to reload scenes

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float scoreMultiplier = 1.0f; // Set this based on difficulty later

    [Header("Live Stats")]
    public float survivalTime = 0f;
    public float currentScore = 0f;

    private bool isGameActive = true;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!isGameActive) return;

        // 1. Handle Timer
        survivalTime += Time.deltaTime;

        // 2. Update UI (Timer)
        // We format it to minutes:seconds
        if (UIController.Instance != null)
        {
            UIController.Instance.UpdateTime(survivalTime);
        }
    }

    public void AddScore(float basePoints)
    {
        if (!isGameActive) return;

        // Apply the multiplier logic
        float finalPoints = basePoints * scoreMultiplier;
        currentScore += finalPoints;

        // Update UI (Score)
        if (UIController.Instance != null)
        {
            UIController.Instance.UpdateScore((int)currentScore);
        }
    }

    public void GameOver()
    {
        if (!isGameActive) return; // Prevent double-triggering
        
        isGameActive = false;

        // 1. Stop the game physics/movement
        // This freezes the background, enemies, and bullets in place
        Time.timeScale = 0f; 

        // 2. Show the UI
        if (UIController.Instance != null)
        {
            UIController.Instance.ShowGameOverScreen((int)currentScore);
        }
        
        Debug.Log("Game Over Triggered.");
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        // For now, just reload the game or log plain text
        Debug.Log("Returning to Menu... (Scene not made yet)");
        // SceneManager.LoadScene("MainMenu"); 
    }
}