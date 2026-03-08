using UnityEngine;

[CreateAssetMenu(menuName = "Gridfall/Card Effects/Heal Effect")]
public class HealEffect : CardEffect
{
    [SerializeField] private int healAmount = 1;

    public override void Execute(CardEffectContext context)
    {
        MatchDataCenter.Instance.AddHP(healAmount);
        Debug.Log($"HealEffect: +{healAmount} HP");
    }
}