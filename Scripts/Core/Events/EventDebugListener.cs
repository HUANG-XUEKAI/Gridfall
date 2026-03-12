using UnityEngine;

public class EventDebugListener : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.CardPlayed += OnCardPlayed;
        GameEvents.HPChanged += OnHPChanged;
        GameEvents.EffectExecuted += OnEffectExecuted;
        GameEvents.GameOver += OnGameOver;
        GameEvents.BoardResolved += OnBoardResolved;
    }

    private void OnDisable()
    {
        GameEvents.CardPlayed -= OnCardPlayed;
        GameEvents.HPChanged -= OnHPChanged;
        GameEvents.EffectExecuted -= OnEffectExecuted;
        GameEvents.GameOver -= OnGameOver;
        GameEvents.BoardResolved -= OnBoardResolved;
    }

    private void OnCardPlayed(GameEvents.CardPlayedEvent e)
    {
        Debug.Log($"[Event] CardPlayed: {e.pattern}, normal={e.normalCardCount}, special={e.specialCardCount}");
    }

    private void OnHPChanged(GameEvents.HPChangedEvent e)
    {
        Debug.Log($"[Event] HPChanged: {e.currentHP}, delta={e.delta}");
    }

    private void OnEffectExecuted(GameEvents.SpecialEffectEvent e)
    {
        Debug.Log($"[Event] SpecialEffectExecuted: {e.sourceCard?.displayName}, effect={e.effectName}");
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