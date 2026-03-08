using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gridfall/Board/Pattern Database")]
public class PatternDatabase : ScriptableObject
{
    public List<BasicPattern> patterns = new();
    
    public BasicPattern GetPatternRandom()
    {
        if (patterns == null || patterns.Count == 0)
            return null;
        return patterns[Random.Range(0, patterns.Count)];
    }
}
