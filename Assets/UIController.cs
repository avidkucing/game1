using UnityEngine;
using UnityEngine.UI;
using TMPro; // If using TextMeshPro

public class UIController : MonoBehaviour
{
    // Singleton instance so other scripts can easily find this
    public static UIController Instance;

    [Header("UI References")]
    public Slider healthSlider;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Call this when the game starts to set max values
    public void InitializeHealth(float currentHP, float maxHP)
    {
        healthSlider.maxValue = maxHP;
        healthSlider.value = currentHP;
    }

    // Call this whenever the player takes damage
    public void UpdateHealth(float currentHP)
    {
        healthSlider.value = currentHP;
    }

    // Call this to update the score display
    public void UpdateScore(int score)
    {
        scoreText.text = "SCORE: " + score.ToString("D6"); // "D6" makes it 000500
    }
    
    // We can update time in the GameLoop later, but here is the setup
    public void UpdateTime(float time)
    {
        // Simple format for seconds
        timeText.text = "T+" + time.ToString("F1") + "s";
    }

    [Header("Game Over Screen")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    // Call this method when the player dies
    public void ShowGameOverScreen(int score)
    {
        // 1. Activate the panel
        gameOverPanel.SetActive(true);

        // 2. Set the score text
        finalScoreText.text = "FINAL SCORE: " + score.ToString("D6");
        
        // Optional: Hide the HUD (Health/Timer) so it looks cleaner
        // healthSlider.gameObject.SetActive(false); 
    }
}