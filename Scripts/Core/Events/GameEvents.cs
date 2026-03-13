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
    
    public static event Action<HPChangedEvent> HPChanged;
    public static void RaiseHPChanged(HPChangedEvent e) => HPChanged?.Invoke(e);
    
    public static event Action<GameStartedEvent> GameStarted; 
    public static void RaiseGameStared(GameStartedEvent e) => GameStarted?.Invoke(e);   
    
    public static event Action<GameOverEvent> GameOver;
    public static void RaiseGameOver(GameOverEvent e) => GameOver?.Invoke(e);
    
    public static event Action<ScoreChangedEvent> ScoreChanged;
    public static void RaiseScoreChanged(ScoreChangedEvent e) => ScoreChanged?.Invoke(e);
    
    public static event Action<bool> GamePaused;
    public static void RaiseGamePaused(bool isPaused) => GamePaused?.Invoke(isPaused);
    
    public static event Action<ProfileChangedEvent> ProfileChanged;
    public static void RaiseProfileChanged(ProfileChangedEvent e) => ProfileChanged?.Invoke(e);
    
    public static event Action InventoryItemsChanged;
    public static void RaiseInventoryItemsChanged() => InventoryItemsChanged?.Invoke();
    
    public static event Action CarriedItemsChanged;
    public static void RaiseCarriedItemsChanged()
        => CarriedItemsChanged?.Invoke();
    
    public class GameStartedEvent
    {
        public int defaultHP = MatchData.DefaultHP;
        public int defaultScore = 0;
        // TODO : 本局所携带的道具
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

    public class HPChangedEvent
    {
        public int currentHP;
        public int delta;
    }
    
    public class GameOverEvent
    {
        public int finalScore;
    }

    public class ScoreChangedEvent
    {
        public int currentScore;
        public int delta;
        public string reason;
    }

    public class ProfileChangedEvent
    {
        public int energy;
        public int gold;
        public int diamond;
    }
}