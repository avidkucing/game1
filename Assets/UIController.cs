using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; // For TimeSpan

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
}