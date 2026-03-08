using System.Collections.Generic;

public enum CardPattern
{
    None,
    Single,     // 兜底：不满足其它牌型就算 Single（按第一张图案）
    Pair,
    Triple,
    Quad,
    Rainbow     // 四种各一张
}

public class PlayContext
{
    public readonly List<CardView> selectedCards = new();
    public readonly List<CardView> normalCards = new();
    public readonly List<CardView> specialCards = new();

    public CardPattern basePattern = CardPattern.None;
    public BasicPattern activePattern;

    public bool HasBaseEffect => basePattern != CardPattern.None;
    public bool HasSpecialEffect => specialCards.Count > 0;
}