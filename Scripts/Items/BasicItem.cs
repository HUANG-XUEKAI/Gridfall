using UnityEngine;
using System;

public enum ItemEffect
{
    None = 0, AddHP = 1, Bomb3x3Random = 2,
}

public enum ItemClass
{
    Consumable = 0, Appearance = 1,
}

[CreateAssetMenu(menuName = "Gridfall/Items/Basic Item")]
public class BasicItem : ScriptableObject
{
    public string itemId;
    public ItemClass itemClass;
    
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;
    
    public int quantity;
    public int maximumStack = 1;
    public int maximumQuantity = 999;

    public ItemEffect effect;
    public int effectValue = 1;
}

/*[Serializable]
public class ItemStack
{
    public BasicItem item;
    public int count;
}*/