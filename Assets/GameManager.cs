using System; // Required for DateTime
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float scoreMultiplier = 1.0f;

    [Header("Energy System")]
    public int maxEnergy = 5;
    public int currentEnergy;
    public int energyCost = 1;
    [Tooltip("Time in seconds to restore 1 Energy")]
    public float rechargeDuration = 60f; // Set to 60 seconds for testing, increase for release
    
    private DateTime nextEnergyTime;
    private bool isRestoring = false;

    // Keys for saving data
    private const string EnergyKey = "Energy";
    private const string EnergyTimeKey = "EnergyTime";
    private const string HighScoreKey = "HighScore";

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
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        
        // Initialize Energy System
        LoadEnergy();
        
        ShowMainMenu();
    }

    void Update()
    {
        // 1. Handle Energy Timer (Always runs, even in menu)
        HandleEnergyRestoration();

        // 2. Handle Gameplay
        if (!isGameActive) return;

        survivalTime += Time.deltaTime;
        float timeScore = 10f * Time.deltaTime * scoreMultiplier;
        currentScore += timeScore;

        if (UIController.Instance != null)
        {
            UIController.Instance.UpdateTime(survivalTime);
            UIController.Instance.UpdateScore((int)currentScore);
        }
    }

    // --- ENERGY SYSTEM ---

    void LoadEnergy()
    {
        currentEnergy = PlayerPrefs.GetInt(EnergyKey, maxEnergy);
        
        // Load the time we were supposed to get the next energy point
        string timeStr = PlayerPrefs.GetString(EnergyTimeKey, string.Empty);

        if (!string.IsNullOrEmpty(timeStr))
        {
            long binaryTime = Convert.ToInt64(timeStr);
            nextEnergyTime = DateTime.FromBinary(binaryTime);
        }
        else
        {
            // First time playing, or data missing
            nextEnergyTime = DateTime.Now;
        }
    }

    void HandleEnergyRestoration()
    {
        // If we are full, don't do anything
        if (currentEnergy >= maxEnergy)
        {
            isRestoring = false;
            UpdateEnergyUI();
            return;
        }

        isRestoring = true;

        // Check if we passed the "Next Energy Time"
        if (DateTime.Now >= nextEnergyTime)
        {
            // Restore 1 energy
            currentEnergy++;
            PlayerPrefs.SetInt(EnergyKey, currentEnergy);

            // Calculate the NEXT time (Add interval to the previous target time)
            // This handles "offline" time. If we were gone for 2 hours, this logic ensures
            // we fill up sequentially.
            nextEnergyTime = nextEnergyTime.AddSeconds(rechargeDuration);

            // If still behind current time (meaning we generated multiple energies), 
            // catch up immediately to avoid a loop, but cap at max.
            if (DateTime.Now >= nextEnergyTime)
            {
                // Just reset to now for simplicity if we lagged way behind
                if(currentEnergy < maxEnergy)
                    nextEnergyTime = DateTime.Now.AddSeconds(rechargeDuration);
            }

            SaveEnergyTime();
            UpdateEnergyUI();
        }
        
        // Update UI Timer text
        if (UIController.Instance != null)
        {
            TimeSpan timeRemaining = nextEnergyTime - DateTime.Now;
            UIController.Instance.UpdateEnergyTimer(timeRemaining);
        }
    }

    void SaveEnergyTime()
    {
        PlayerPrefs.SetString(EnergyTimeKey, nextEnergyTime.ToBinary().ToString());
        PlayerPrefs.Save();
    }

    void UpdateEnergyUI()
    {
        if (UIController.Instance != null)
        {
            UIController.Instance.UpdateEnergyDisplay(currentEnergy, maxEnergy);
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

        if (UIController.Instance != null)
        {
            UIController.Instance.UpdateHighScoreText(highScore);
        }
        
        // Ensure UI is up to date when entering menu
        UpdateEnergyUI();
    }

    // UPDATED: Now checks for energy before starting
    public void TryStartGame()
    {
        if (currentEnergy >= energyCost)
        {
            // Deduct Energy
            currentEnergy -= energyCost;
            PlayerPrefs.SetInt(EnergyKey, currentEnergy);
            
            // If we were full, start the timer now
            if (currentEnergy == maxEnergy - energyCost)
            {
                nextEnergyTime = DateTime.Now.AddSeconds(rechargeDuration);
                SaveEnergyTime();
            }

            // Actually Start
            StartGameLogic();
        }
        else
        {
            Debug.Log("Not Enough Energy to play!");
            // Optional: Play a sound or shake UI
        }
    }

    private void StartGameLogic()
    {
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
        float finalPoints = basePoints * scoreMultiplier;
        currentScore += finalPoints;
    }

    public void GameOver()
    {
        if (!isGameActive) return;
        isGameActive = false;
        Time.timeScale = 0f;

        if (currentScore > highScore)
        {
            highScore = (int)currentScore;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }

        if (UIController.Instance != null)
        {
            UIController.Instance.ShowGameOverScreen((int)currentScore, highScore);
        }
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}