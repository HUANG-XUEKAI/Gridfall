using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Gridfall/Items/Basic Item")]
public class BasicItem : ScriptableObject
{
    public string itemId;
    public ItemType itemType;
    
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;

    public ConsumableEffectType effectType;
    public int effectValue = 1;
}

[Serializable]
public class ItemStack
{
    public BasicItem item;
    public int count;
}

[Serializable]
public class ItemState
{
    public BasicItem item;
    public bool used;
}

public enum ConsumableEffectType
{
    None = 0,
    AddHP = 1,
    Bomb3x3Random = 2,
}

public enum ItemType
{
    Consumable = 0,
    Appearance = 1,
}