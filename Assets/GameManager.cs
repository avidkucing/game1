using System; 
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // For tracking consumed components

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float scoreMultiplier = 1.0f;

    [Header("Luck & Economy")]
    public int luckLevel = 0; // Current Luck (Lv 0-5)
    public int dropsInCurrentRun = 0; 
    public float baseDropFloor = 0.0001f; // 0.01% base floor

    [Header("Synthesis System")]
    // Tracks which components (FR, PR, etc.) are consumed by an Evolution
    public HashSet<string> consumedComponents = new HashSet<string>();

    [Header("Energy System")]
    public int maxEnergy = 5;
    public int currentEnergy;
    public int energyCost = 1;
    public float rechargeDuration = 60f; 
    
    private DateTime nextEnergyTime;

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
        LoadEnergy();
        ShowMainMenu();
    }

    void Update()
    {
        HandleEnergyRestoration();

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

    // Helper to get the dynamic chance for EnemyController
    public float GetCurrentDropChance()
    {
        // Decay reduction: Each Luck point reduces decay by 10%
        float currentDecay = 0.5f * (1f - (luckLevel * 0.1f)); 
        
        // Floor buff: Each point increases floor by 20% compound
        float currentFloor = baseDropFloor * Mathf.Pow(1.2f, luckLevel);
        
        // Multiplicative decay based on total drops so far
        float calculatedChance = Mathf.Pow(currentDecay, dropsInCurrentRun);
        
        return Mathf.Max(calculatedChance, currentFloor);
    }

    // --- NEW: SYNTHESIS LOGIC ---

    public bool IsComponentAvailable(string componentID)
    {
        // Returns false if component was already used in an Evolution
        return !consumedComponents.Contains(componentID);
    }

    public void ConsumeComponents(string id1, string id2)
    {
        // Mark base components as locked after synthesis
        consumedComponents.Add(id1);
        consumedComponents.Add(id2);
        Debug.Log($"Synthesis Complete: {id1} and {id2} are now consumed.");
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
            UpdateEnergyUI();
            return;
        }

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

    // --- UPDATED GAME START ---

    public void TryStartGame()
    {
        if (currentEnergy >= energyCost)
        {
            currentEnergy -= energyCost;
            PlayerPrefs.SetInt(EnergyKey, currentEnergy);
            
            if (currentEnergy == maxEnergy - energyCost)
            {
                nextEnergyTime = DateTime.Now.AddSeconds(rechargeDuration);
                SaveEnergyTime();
            }

            // Reset run-specific Luck stats
            dropsInCurrentRun = 0;
            consumedComponents.Clear();

            StartGameLogic();
        }
        else
        {
            Debug.Log("Not Enough Energy to play!");
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