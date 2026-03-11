using System.Collections.Generic;

[System.Serializable]
public class MatchData
{
    public const int MaxHP = 99;
    public const int DefaultHP = 5;
    
    public int currentHP;
    public int currentScore;
    public bool isGaming;
    public bool isPausing;
    
    public List<BasicCard> handCards = new();
    public List<MatchConsumableSlot> equippedConsumables = new();

    public void ClearPreparedConsumables()
    {
        
    }

    public void CanAddPreparedConsumable()
    {
        
    }

    public void AddPreparedConsumable(string itemId)
    {
        
    }

    public void RemovePreparedConsumable(string itemId)
    {
        
    }
}