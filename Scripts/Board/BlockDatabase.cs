using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gridfall/Board/Block Database")]
public class BlockDatabase : ScriptableObject
{
    [SerializeField] private PatternDatabase patternDatabase;
    [SerializeField] private BasicBlock normalBlockTemplate;
    
    [SerializeField] private List<BasicBlock> specialBlocks = new();
    [SerializeField] private bool allowSpecialBlocks = false;
    [SerializeField, Range(0f, 1f)] private float specialBlockChance = 0.15f;
    
    private BasicBlock[] normalBlocks;
    
    public bool AllowSpecialBlocks => allowSpecialBlocks;
    
    public void InitializeNormalBlocks()
    {
        if (patternDatabase == null || 
            patternDatabase.patterns == null || 
            patternDatabase.patterns.Count == 0)
        {
            normalBlocks = null;
            return;
        }

        normalBlocks = new BasicBlock[patternDatabase.patterns.Count];

        for (int i = 0; i < patternDatabase.patterns.Count; i++)
        {
            var runtimeBlock = Instantiate(normalBlockTemplate);
            runtimeBlock.pattern = patternDatabase.patterns[i];
            normalBlocks[i] = runtimeBlock;
        }
    }
    
    public BasicBlock GetNormalBlockRandom()
    {
        bool blockArrayIsEmpty = (normalBlocks == null || normalBlocks.Length == 0);
        
        if (blockArrayIsEmpty)
            InitializeNormalBlocks();
        if (blockArrayIsEmpty)
            return null;

        return normalBlocks[Random.Range(0, normalBlocks.Length)];
    }

    public BasicBlock GetSpecialBlockRandom()
    {
        bool trySpecial = allowSpecialBlocks
                          && specialBlocks.Count > 0
                          && Random.value < specialBlockChance;
        
        if (trySpecial) 
            return specialBlocks[Random.Range(0, specialBlocks.Count)];
        
        return null;
    }
}
