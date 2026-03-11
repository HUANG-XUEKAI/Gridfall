using System.Collections.Generic;

[System.Serializable]
public class PlayerInventoryData
{
    public List<InventoryItemStack> consumables = new();
    public List<string> ownedCardSkinIds = new();
    public List<string> ownedBoardSkinIds = new();
}