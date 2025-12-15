using UnityEngine;
using UnityEngine.SceneManagement;

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
    public int highScore = 0;

    private bool isGameActive = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Load Highscore from disk
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        
        ShowMainMenu();
    }

    void Update()
    {
        if (!isGameActive) return;

        // 1. Handle Timer
        survivalTime += Time.deltaTime;

        // 2. Score based on Survival Time (10 points per second * multiplier)
        // This encourages staying alive, not just killing
        float timeScore = 10f * Time.deltaTime * scoreMultiplier;
        currentScore += timeScore;

        // 3. Update UI
        if (UIController.Instance != null)
        {
            UIController.Instance.UpdateTime(survivalTime);
            UIController.Instance.UpdateScore((int)currentScore);
        }
    }

    // --- MENU LOGIC ---

    public void ShowMainMenu()
    {
        isGameActive = false;
        Time.timeScale = 1f;

        if(mainMenuPanel) mainMenuPanel.SetActive(true);
        if(hudPanel) hudPanel.SetActive(false);
        if(gameOverPanel) gameOverPanel.SetActive(false);

        ToggleGameplayScripts(false);

        // Show Highscore on Menu
        if (UIController.Instance != null)
        {
            UIController.Instance.UpdateHighScoreText(highScore);
        }
    }

    // Called by difficulty buttons in the future
    public void SetDifficulty(float multiplier)
    {
        scoreMultiplier = multiplier;
    }

    public void StartGame()
    {
        // Reset Stats
        currentScore = 0;
        survivalTime = 0;
        
        isGameActive = true;

        if(mainMenuPanel) mainMenuPanel.SetActive(false);
        if(hudPanel) hudPanel.SetActive(true);

        ToggleGameplayScripts(true);
    }

    void ToggleGameplayScripts(bool state)
    {
        if (playerScript != null) playerScript.enabled = state;
        if (gunScript != null) gunScript.enabled = state;
        if (spawnerScript != null) spawnerScript.enabled = state;
    }

    // --- GAMEPLAY LOGIC ---

    public void AddScore(float basePoints)
    {
        if (!isGameActive) return;
        
        // Add score from kills
        float finalPoints = basePoints * scoreMultiplier;
        currentScore += finalPoints;
    }

    public void GameOver()
    {
        if (!isGameActive) return;
        
        isGameActive = false;
        Time.timeScale = 0f; // Pause Physics

        // Check High Score
        if (currentScore > highScore)
        {
            highScore = (int)currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            Debug.Log("New High Score Saved: " + highScore);
        }

        if (UIController.Instance != null)
        {
            UIController.Instance.ShowGameOverScreen((int)currentScore, highScore);
        }
        
        Debug.Log("Game Over Triggered.");
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}