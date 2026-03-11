using System.Collections.Generic;

[System.Serializable]
public class MatchData
{
    public const int MaxHP = 99;
    public const int DefaultHP = 5;
    public const int MaxPreparedConsumableCount = 2;
    
    public int currentHP;
    public int currentScore;
    public bool isGaming;
    public bool isPausing;
    
    public List<BasicCard> handCards = new();
    public List<MatchConsumableSlot> equippedConsumables = new();

    public void ClearPreparedConsumables()
    {
        equippedConsumables.Clear();
    }

    public bool CanAddPreparedConsumable()
    {
        return equippedConsumables.Count < MaxPreparedConsumableCount;
    }

    public bool AddPreparedConsumable(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return false;
        if (!CanAddPreparedConsumable()) return false;

        equippedConsumables.Add(new MatchConsumableSlot
        {
            itemId = itemId,
            used = false
        });

        return true;
    }

    public bool RemovePreparedConsumable(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return false;

        var slot = equippedConsumables.FindLast(x => x.itemId == itemId && !x.used);
        if (slot == null) return false;

        equippedConsumables.Remove(slot);
        return true;
    }
}