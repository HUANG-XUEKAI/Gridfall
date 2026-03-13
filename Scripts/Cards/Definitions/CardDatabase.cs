using UnityEngine;

[CreateAssetMenu(menuName = "Gridfall/Cards/Card Database")]
public class CardDatabase : ScriptableObject
{
    [SerializeField] private PatternDatabase patternDatabase;
    [SerializeField] private BasicCard basicCardTemplate;
    
    private BasicCard[] basicCards;
    
    public void InitializeBasicCards()
    {
        if (patternDatabase == null || 
            patternDatabase.patterns == null || 
            patternDatabase.patterns.Count == 0)
        {
            basicCards = null;
            return;
        }

        basicCards = new BasicCard[patternDatabase.patterns.Count];

        for (int i = 0; i < patternDatabase.patterns.Count; i++)
        {
            var runtimeCard = Instantiate(basicCardTemplate);
            runtimeCard.pattern = patternDatabase.patterns[i];
            basicCards[i] = runtimeCard;
        }
    }
    
    public BasicCard GetCardRandom()
    {
        bool cardArrayIsEmpty = (basicCards == null || basicCards.Length == 0);
        
        if (cardArrayIsEmpty)
            InitializeBasicCards();
        if (cardArrayIsEmpty)
            return null;
        
        return basicCards[Random.Range(0, basicCards.Length)];
    }
}