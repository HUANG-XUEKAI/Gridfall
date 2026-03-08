using UnityEngine;

[CreateAssetMenu(menuName = "Gridfall/Score/Score Config")]
public class ScoreConfig : ScriptableObject
{
    /*[Header("Base Score")]
    public int baseCellScore = 1;*/

    [Header("Pattern Multiplier")]
    public int singleMultiplier = 1;
    public int pairMultiplier = 2;
    public int tripleMultiplier = 3;
    public int quadMultiplier = 4;
    public int rainbowMultiplier = 4;

    public int GetPatternMultiplier(CardPattern pattern)
    {
        return pattern switch
        {
            CardPattern.Single => singleMultiplier,
            CardPattern.Pair => pairMultiplier,
            CardPattern.Triple => tripleMultiplier,
            CardPattern.Quad => quadMultiplier,
            CardPattern.Rainbow => rainbowMultiplier,
            _ => 0
        };
    }
}