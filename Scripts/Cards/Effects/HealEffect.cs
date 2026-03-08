using UnityEngine;

[CreateAssetMenu(menuName = "Gridfall/Card Effects/Heal Effect")]
public class HealEffect : CardEffect
{
    [SerializeField] private int healAmount = 1;

    public override void Execute(CardEffectContext context)
    {
        if (context?.gameManager == null) 
            return;
        context.gameManager.AddHP(healAmount);
        Debug.Log($"HealEffect: +{healAmount} HP");
    }
}