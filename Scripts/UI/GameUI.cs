using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hpText;

    private void OnEnable()
    {
        GameEvents.HPChanged += OnHPChanged;
        GameEvents.ScoreChanged += OnScoreChanged;
    }

    private void OnDisable()
    {
        GameEvents.HPChanged -= OnHPChanged;
        GameEvents.ScoreChanged -= OnScoreChanged;
    }

    private void Start()
    {
        RefreshScoreText(0);
        RefreshHPText(MatchData.DefaultHP);
    }

    private void OnHPChanged(HPChangedEvent e)
    {
        RefreshHPText(e.currentHP);
    }

    private void OnScoreChanged(ScoreChangedEvent e)
    {
        RefreshScoreText(e.currentScore);
    }

    private void RefreshScoreText(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }
    
    private void RefreshHPText(int hp)
    {
        if (hpText) 
            hpText.text = $"HP: {hp}";
    }
}
