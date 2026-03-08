using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private ScoreConfig scoreConfig;
    
    private MatchDataCenter MDC => MatchDataCenter.Instance;
    private AccountDataCenter ADC => AccountDataCenter.Instance;

    private void OnEnable()
    {
        GameEvents.BoardResolved += OnBoardResolved;
        GameEvents.GameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameEvents.BoardResolved -= OnBoardResolved;
        GameEvents.GameOver -= OnGameOver;
    }
    
    private void OnBoardResolved(BoardResolvedEvent e)
    {
        if (scoreConfig == null) return;
        if (e.pattern == CardPattern.None) return;
        if (e.clearedCellCount <= 0) return;

        int multiplier = scoreConfig.GetPatternMultiplier(e.pattern);
        if (multiplier <= 0) return;

        int amount = 0; // 待改
        // TODO : 计分逻辑改成：每个格子BasicBlock的基础分 * multiplier

        MDC.AddScore(amount, "消除得分（Temp）");
    }

    private void OnGameOver(GameOverEvent e)
    {
        if (MDC.CurrentMatch.currentScore > ADC.Profile.bestScore)
        {
            ADC.Profile.bestScore = MDC.CurrentMatch.currentScore;
            Debug.Log($"新纪录：{ADC.Profile.bestScore}");
        }

        ADC.Progress.totalPlayCount++;
        Debug.Log("Game Over!");
    }
}