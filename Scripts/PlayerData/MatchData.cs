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
    public List<ItemState> carriedItems = new();

    public void ClearCarriedItems()
    {
        carriedItems.Clear();
    }

    public bool CanCarryMore()
    {
        return carriedItems.Count < MaxPreparedConsumableCount;
    }

    public bool AddCarriedItem(BasicItem item)
    {
        if (item == null) return false;
        if (!CanCarryMore()) return false;

        carriedItems.Add(new ItemState
        {
            item = item,
            used = false
        });

        return true;
    }

    public bool RemoveCarriedItem(string itemId)
    {
        /*if (string.IsNullOrEmpty(itemId)) return false;

        var slot = carriedItems.FindLast(x => x.item == itemId && !x.used);
        if (slot == null) return false;

        carriedItems.Remove(slot);
        return true;*/

        return true;
        // 重写
    }
}