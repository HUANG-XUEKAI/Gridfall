using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private ScoreConfig scoreConfig;

    public int CurrentScore { get; private set; }

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

    public void ResetScore()
    {
        CurrentScore = 0;
        RaiseScoreChanged(0, "Reset");
    }

    private void OnBoardResolved(BoardResolvedEvent e)
    {
        if (scoreConfig == null) return;
        if (e.pattern == CardPattern.None) return;
        if (e.clearedCellCount <= 0) return;

        int multiplier = scoreConfig.GetPatternMultiplier(e.pattern);
        if (multiplier <= 0) return;

        int add = e.clearedCellCount * scoreConfig.baseCellScore * multiplier;

        CurrentScore += add;
        RaiseScoreChanged(add, $"{e.pattern} x {e.clearedCellCount}");
    }

    private void OnGameOver(GameOverEvent e)
    {
        Debug.Log($"[ScoreSystem] Final Score = {CurrentScore}");
    }

    private void RaiseScoreChanged(int delta, string reason)
    {
        GameEvents.RaiseScoreChanged(new ScoreChangedEvent
        {
            currentScore = CurrentScore,
            delta = delta,
            reason = reason
        });
    }
}