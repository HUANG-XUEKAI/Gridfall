using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gridfall/Cards/Card Database")]
public class CardDatabase : ScriptableObject
{
    [SerializeField] private PatternDatabase patternDatabase;
    [SerializeField] private BasicCard normalCardTemplate;
    [SerializeField] private List<SpecialCard> specialCards = new();
    
    [SerializeField] private bool allowSpecialCards = true;
    [SerializeField, Range(0f, 1f)] private float specialDrawChance = 0.15f;
    
    private BasicCard[] normalCards;
    public bool AllowSpecialCards => allowSpecialCards;

    public void InitializeNormalCards()
    {
        if (patternDatabase == null || 
            patternDatabase.patterns == null || 
            patternDatabase.patterns.Count == 0)
        {
            normalCards = null;
            return;
        }

        normalCards = new BasicCard[patternDatabase.patterns.Count];

        for (int i = 0; i < patternDatabase.patterns.Count; i++)
        {
            var runtimeCard = Instantiate(normalCardTemplate);
            runtimeCard.pattern = patternDatabase.patterns[i];
            normalCards[i] = runtimeCard;
        }
    }
    
    public BasicCard GetNormalCardRandom()
    {
        bool cardArrayIsEmpty = (normalCards == null || normalCards.Length == 0);
        
        if (cardArrayIsEmpty)
            InitializeNormalCards();
        if (cardArrayIsEmpty)
            return null;
        
        return normalCards[Random.Range(0, normalCards.Length)];
    }
    
    public SpecialCard GetSpecialCardRandom()
    {
        bool trySpecial = allowSpecialCards
                          && specialCards.Count > 0
                          && Random.value < specialDrawChance;
        
        if (trySpecial)
            return specialCards[Random.Range(0, specialCards.Count)];
        
        return null;
    }
}