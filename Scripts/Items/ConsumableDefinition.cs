using UnityEngine;

public enum ConsumableEffectType
{
    None = 0,
    AddHP = 1,
    Bomb3x3Random = 2,
}

[CreateAssetMenu(menuName = "Gridfall/Items/Consumable Definition")]
public class ConsumableDefinition : ScriptableObject
{
    public string itemId;
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;

    public ConsumableEffectType effectType;
    public int effectValue = 1;
}