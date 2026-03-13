using UnityEngine;

public class EventDebugListener : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.CardPlayed += OnCardPlayed;
        GameEvents.HPChanged += OnHPChanged;
        GameEvents.GameOver += OnGameOver;
        GameEvents.BoardResolved += OnBoardResolved;
    }

    private void OnDisable()
    {
        GameEvents.CardPlayed -= OnCardPlayed;
        GameEvents.HPChanged -= OnHPChanged;
        GameEvents.GameOver -= OnGameOver;
        GameEvents.BoardResolved -= OnBoardResolved;
    }

    private void OnCardPlayed(GameEvents.CardPlayedEvent e)
    {
        Debug.Log($"[Event] CardPlayed: {e.pattern}");
    }

    private void OnHPChanged(GameEvents.ChangeData e)
    {
        Debug.Log($"[Event] HPChanged: {e.currValue}, delta={e.delta}");
    }
    
    private void OnGameOver(GameEvents.GameOverEvent e)
    {
        Debug.Log($"[Event] GameOver: finalScore={e.finalScore}");
    }

    private void OnBoardResolved(GameEvents.BoardResolvedEvent e)
    {
        Debug.Log($"[Event] BoardResolved: pattern={e.pattern}, cells={e.clearedCellCount}, lines={e.clearedLineCount}");
    }
}