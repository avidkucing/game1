using UnityEngine;
using UnityEngine.SceneManagement; // Needed to reload scenes

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float scoreMultiplier = 1.0f;

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject hudPanel;
    public GameObject gameOverPanel;

    [Header("Game References")]
    public PlayerController playerScript;
    public AutoShooter gunScript;
    public EnemySpawner spawnerScript;

    [Header("Live Stats")]
    public float survivalTime = 0f;
    public float currentScore = 0f;

    private bool isGameActive = false; // Start false so we see the menu first

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // When the game launches, we don't start playing immediately.
        // We show the Main Menu instead.
        ShowMainMenu();
    }

    void Update()
    {
        if (!isGameActive) return;

        // 1. Handle Timer
        survivalTime += Time.deltaTime;

        // 2. Update UI (Timer)
        if (UIController.Instance != null)
        {
            UIController.Instance.UpdateTime(survivalTime);
        }
    }

    // --- NEW: MENU LOGIC ---

    public void ShowMainMenu()
    {
        isGameActive = false;
        Time.timeScale = 1f; // Ensure time is running so background scrolls

        if(mainMenuPanel) mainMenuPanel.SetActive(true);
        if(hudPanel) hudPanel.SetActive(false);
        if(gameOverPanel) gameOverPanel.SetActive(false);

        if (playerScript != null) playerScript.enabled = false;
        if (gunScript != null) gunScript.enabled = false;
        if (spawnerScript != null) spawnerScript.enabled = false;
    }

    public void StartGame()
    {
        isGameActive = true;

        if(mainMenuPanel) mainMenuPanel.SetActive(false);
        if(hudPanel) hudPanel.SetActive(true);

        if (playerScript != null) playerScript.enabled = true;
        if (gunScript != null) gunScript.enabled = true;
        if (spawnerScript != null) spawnerScript.enabled = true;
    }

    // --- EXISTING LOGIC ---

    public void AddScore(float basePoints)
    {
        if (!isGameActive) return;

        float finalPoints = basePoints * scoreMultiplier;
        currentScore += finalPoints;

        if (UIController.Instance != null)
        {
            UIController.Instance.UpdateScore((int)currentScore);
        }
    }

    public void GameOver()
    {
        if (!isGameActive) return;
        
        isGameActive = false;

        // Freeze physics
        Time.timeScale = 0f; 

        // Show Game Over UI
        if (UIController.Instance != null)
        {
            // We pass the score to the UI
            UIController.Instance.ShowGameOverScreen((int)currentScore);
        }
        
        Debug.Log("Game Over Triggered.");
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f; // Unpause!
        // Reloads the scene to clean up all bullets/enemies automatically
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
