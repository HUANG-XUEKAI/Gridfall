using System;

public static class GameEvents
{
    public static event Action<CardPlayedEvent>    CardPlayed;
    public static event Action<BoardResolvedEvent> BoardResolved;
    public static event Action<HPChangedEvent>     HPChanged;
    public static event Action<SpecialEffectEvent> EffectExecuted;
    public static event Action<GameOverEvent>      GameOver;
    public static event Action<ScoreChangedEvent>  ScoreChanged;

    public static void RaiseCardPlayed(CardPlayedEvent e)        => CardPlayed?.Invoke(e);
    public static void RaiseBoardResolved(BoardResolvedEvent e)  => BoardResolved?.Invoke(e);
    public static void RaiseHPChanged(HPChangedEvent e)          => HPChanged?.Invoke(e);
    public static void RaiseEffectExecuted(SpecialEffectEvent e) => EffectExecuted?.Invoke(e);
    public static void RaiseGameOver(GameOverEvent e)            => GameOver?.Invoke(e);
    public static void RaiseScoreChanged(ScoreChangedEvent e)    => ScoreChanged?.Invoke(e);
}

public class CardPlayedEvent
{
    public CardPattern pattern;
    public BasicPattern activeShape;
    public int normalCardCount;
    public int specialCardCount;
}

public class BoardResolvedEvent
{
    public CardPattern pattern;
    public int clearedCellCount;
    public int clearedLineCount;
}

public class HPChangedEvent
{
    public int currentHP;
    public int maxHP;
    public int delta;
}

public class SpecialEffectEvent
{
    public BasicCard sourceCard;
    public string effectName;
}

public class GameOverEvent
{
    public int finalHP;
}

public class ScoreChangedEvent
{
    public int currentScore;
    public int delta;
    public string reason;
}