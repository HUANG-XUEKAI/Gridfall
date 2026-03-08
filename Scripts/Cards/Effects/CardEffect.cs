using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    public abstract void Execute(CardEffectContext context);
}

public class CardEffectContext
{
    public BoardManager board;
    //public GameManager gameManager;
    public Vector2Int targetCell;
}