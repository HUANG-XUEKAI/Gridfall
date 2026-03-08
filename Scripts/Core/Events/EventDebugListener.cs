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

    private void OnCardPlayed(CardPlayedEvent e)
    {
        Debug.Log($"[Event] CardPlayed: {e.pattern}, normal={e.normalCardCount}, special={e.specialCardCount}");
    }

    private void OnHPChanged(HPChangedEvent e)
    {
        Debug.Log($"[Event] HPChanged: {e.currentHP}/{e.maxHP}, delta={e.delta}");
    }

    private void OnEffectExecuted(SpecialEffectEvent e)
    {
        Debug.Log($"[Event] SpecialEffectExecuted: {e.sourceCard?.displayName}, effect={e.effectName}");
    }

    private void OnGameOver(GameOverEvent e)
    {
        Debug.Log($"[Event] GameOver: finalHP={e.finalHP}");
    }

    private void OnBoardResolved(BoardResolvedEvent e)
    {
        Debug.Log($"[Event] BoardResolved: pattern={e.pattern}, cells={e.clearedCellCount}, lines={e.clearedLineCount}");
    }
}