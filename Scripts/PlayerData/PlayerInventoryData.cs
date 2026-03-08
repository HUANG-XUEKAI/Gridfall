using System.Collections.Generic;

[System.Serializable]
public class PlayerInventoryData
{
    public List<string> ownedItemIds = new();
    public List<string> ownedCardSkinIds = new();
    public List<string> ownedBoardSkinIds = new();
}