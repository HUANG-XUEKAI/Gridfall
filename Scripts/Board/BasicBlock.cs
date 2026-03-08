using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Gridfall/Board/Basic Block")]
public class BasicBlock : ScriptableObject
{
    public string id;
    [NonSerialized] 
    public BasicPattern pattern;
    public int baseScore = 1;
}
