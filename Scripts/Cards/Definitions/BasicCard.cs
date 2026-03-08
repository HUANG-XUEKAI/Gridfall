using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Gridfall/Cards/Basic Card")]
public class BasicCard : ScriptableObject
{
    public string cardId;
    public string displayName;
    [NonSerialized]
    public BasicPattern pattern;
}