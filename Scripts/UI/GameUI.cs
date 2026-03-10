using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI diamondText;
    
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hpText;
    
    private AccountDataCenter ADC => AccountDataCenter.Instance;
    
    private void OnEnable()
    {
        GameEvents.ProfileChanged += OnProfileChanged;
        
        GameEvents.HPChanged += OnHPChanged;
        GameEvents.ScoreChanged += OnScoreChanged;
        GameEvents.GameStarted += OnGameStarted;
    }

    private void OnDisable()
    {
        GameEvents.ProfileChanged -= OnProfileChanged;
        
        GameEvents.HPChanged -= OnHPChanged;
        GameEvents.ScoreChanged -= OnScoreChanged;
        GameEvents.GameStarted -= OnGameStarted;
    }

    private void Start()
    {
        RefreshEnergyText(ADC.Profile.energy);
        RefreshGoldText(ADC.Profile.gold);
        RefreshDiamondText(ADC.Profile.diamond);
        
        RefreshScoreText(0);
        RefreshHPText(MatchData.DefaultHP);
    }

    private void OnGameStarted(GameStartedEvent e)
    {
        RefreshHPText(e.defaultHP);
        RefreshScoreText(e.defaultScore);
    }
    
    private void OnProfileChanged(ProfileChangedEvent e)
    {
        RefreshEnergyText(e.energy);
        RefreshGoldText(e.gold);
        RefreshDiamondText(e.diamond);
    }

    private void OnHPChanged(HPChangedEvent e)
    {
        RefreshHPText(e.currentHP);
    }

    private void OnScoreChanged(ScoreChangedEvent e)
    {
        RefreshScoreText(e.currentScore);
    }

    private void RefreshEnergyText(int energy)
    {
        if (energyText != null)
            energyText.text = $"E: {energy}";
    }

    private void RefreshGoldText(int gold)
    {
        if (goldText != null)
            goldText.text = $"G: {gold}";
    }

    private void RefreshDiamondText(int diamond)
    {
        if (diamondText != null)
            diamondText.text = $"D: {diamond}";
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
