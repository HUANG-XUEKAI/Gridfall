using System;

public static class GameEvents
{
    public static event Action<GameFlowState, GameFlowState> OnStateChanged;
    public static void RaiseStateChanged(GameFlowState oldState, GameFlowState newState) 
        => OnStateChanged?.Invoke(oldState, newState);
    
    public static event Action<CardPlayedEvent> CardPlayed;
    public static void RaiseCardPlayed(CardPlayedEvent e) => CardPlayed?.Invoke(e);
    
    public static event Action<BoardResolvedEvent> BoardResolved;
    public static void RaiseBoardResolved(BoardResolvedEvent e) => BoardResolved?.Invoke(e);
    
    public static event Action GameStarted; 
    public static void RaiseGameStared() => GameStarted?.Invoke();   
    
    public static event Action<GameOverEvent> GameOver;
    public static void RaiseGameOver(GameOverEvent e) => GameOver?.Invoke(e);
    
    public static event Action<bool> GamePaused;
    public static void RaiseGamePaused(bool isPaused) => GamePaused?.Invoke(isPaused);
    
    public static event Action<ChangeData> HPChanged;
    public static void RaiseHPChanged(ChangeData e) => HPChanged?.Invoke(e);
    
    public static event Action<ChangeData> ScoreChanged;
    public static void RaiseScoreChanged(ChangeData e) => ScoreChanged?.Invoke(e);
    
    public static event Action<ChangeData> EnergyChanged;
    public static void RaiseEnergyChanged(ChangeData e) => EnergyChanged?.Invoke(e);
    
    public static event Action<ChangeData> GoldChanged;
    public static void RaiseGoldChanged(ChangeData e) => GoldChanged?.Invoke(e);
    
    public static event Action<ChangeData> DiamondChanged;
    public static void RaiseDiamondChanged(ChangeData e) => DiamondChanged?.Invoke(e);
    
    public static event Action InventoryItemsChanged;
    public static void RaiseInventoryItemsChanged() => InventoryItemsChanged?.Invoke();
    
    public static event Action CarriedItemsChanged;
    public static void RaiseCarriedItemsChanged() => CarriedItemsChanged?.Invoke();
    
    public class ChangeData { 
        public int currValue;
        public int delta;
        public string reason;
    }
    
    public class CardPlayedEvent
    {
        public CardPattern pattern;
        public BasicPattern activeShape;
        public int cardCount;
    }

    public class BoardResolvedEvent
    {
        public CardPattern pattern;
        public int clearedCellCount;
        public int clearedLineCount;
        public int clearedBaseScore;
    }
    
    public class GameOverEvent
    {
        public int finalScore;
    }
}