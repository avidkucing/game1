using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; // For TimeSpan
using System.Collections.Generic; // For List<>

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    [Header("HUD References")]
    public Slider healthSlider;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;

    [Header("Menu References")]
    public TextMeshProUGUI menuHighScoreText;
    public TextMeshProUGUI energyText;      // e.g., "5/5"
    public TextMeshProUGUI energyTimerText; // e.g., "00:59"

    [Header("Game Over Screen")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalHighScoreText;

    [Header("Upgrade Selection UI")]
    public GameObject upgradePanel;           // The main container for the choice UI
    public Button[] upgradeButtons;           // Exactly 3 buttons in the panel
    public TextMeshProUGUI[] upgradeNameTexts; // Text components for titles (e.g., "Fire Rate")
    public TextMeshProUGUI[] upgradeDescTexts; // Text components for rarity/level info

    // Keep track of current choices to apply the selection later
    private List<UpgradeManager.UpgradeOption> currentChoices;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void InitializeHealth(float currentHP, float maxHP)
    {
        if(healthSlider) 
        {
            healthSlider.maxValue = maxHP;
            healthSlider.value = currentHP;
        }
    }

    public void UpdateHealth(float currentHP)
    {
        if(healthSlider) healthSlider.value = currentHP;
    }

    public void UpdateScore(int score)
    {
        if(scoreText) scoreText.text = "SCORE: " + score.ToString("D6");
    }
    
    public void UpdateTime(float time)
    {
        if(timeText) timeText.text = "T+" + time.ToString("F1") + "s";
    }

    public void UpdateHighScoreText(int highScore)
    {
        if (menuHighScoreText) 
            menuHighScoreText.text = "BEST: " + highScore.ToString("D6");
    }

    // --- NEW ENERGY UI ---

    public void UpdateEnergyDisplay(int current, int max)
    {
        if (energyText) 
            energyText.text = $"ENERGY: {current}/{max}";
        
        // Hide timer if energy is full
        if (current >= max && energyTimerText != null)
        {
            energyTimerText.text = "FULL";
        }
    }

    public void UpdateEnergyTimer(TimeSpan timeRemaining)
    {
        if (energyTimerText)
        {
            // Format time as MM:SS
            energyTimerText.text = string.Format("{0:D2}:{1:D2}", timeRemaining.Minutes, timeRemaining.Seconds);
        }
    }

    // ---------------------

    public void ShowGameOverScreen(int score, int highScore)
    {
        gameOverPanel.SetActive(true);

        if(finalScoreText) finalScoreText.text = "FINAL SCORE: " + score.ToString("D6");
        
        if(finalHighScoreText)
        {
            if(score >= highScore && score > 0)
                finalHighScoreText.text = "NEW HIGH SCORE!";
            else
                finalHighScoreText.text = "BEST: " + highScore.ToString("D6");
        }
    }

    public void ShowUpgradePanel(List<UpgradeManager.UpgradeOption> choices)
    {
        currentChoices = choices;
        upgradePanel.SetActive(true);

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            if (i < choices.Count)
            {
                upgradeButtons[i].gameObject.SetActive(true);
                
                // Set text based on the upgrade data
                upgradeNameTexts[i].text = choices[i].name;
                
                // Visual feedback for Rarity
                upgradeDescTexts[i].text = choices[i].isEpic ? "<color=purple>EPIC</color>" : "Common";
                upgradeDescTexts[i].text += "\nTap to Upgrade";
            }
            else
            {
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // Linked to the onClick() event of the 3 buttons in Unity (Index 0, 1, or 2)
    public void SelectUpgrade(int index)
    {
        if (index >= currentChoices.Count) return;

        var choice = currentChoices[index];
        ApplyUpgrade(choice);

        // Resume the game
        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void ApplyUpgrade(UpgradeManager.UpgradeOption choice)
    {
        // Logic to increment levels
        if (choice.id == "FR") 
        {
            GameManager.Instance.gunScript.UpgradeWeapon();
        }
        else if (PlayerController.Instance.defenseLevels.ContainsKey(choice.id))
        {
            PlayerController.Instance.defenseLevels[choice.id]++;
            Debug.Log($"{choice.name} upgraded to Level {PlayerController.Instance.defenseLevels[choice.id]}");
        }
    }
}