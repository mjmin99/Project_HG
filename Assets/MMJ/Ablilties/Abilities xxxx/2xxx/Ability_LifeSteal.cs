using UnityEngine;

public class Ability_LifeSteal : AbilityBase, IEventAbility
{
    private readonly AbilityRarity rarity;

    public Ability_LifeSteal(AbilityRarity rarity)
    {
        this.rarity = rarity;
    }

    public override int AbilityId => AbilityIds.LifeStealOnAttack;
    public override string Name => "공격 시 흡혈";
    public override AbilityRarity Rarity => rarity;
    public override AbilityScope Scope => AbilityScope.Dealer_Melee;

    public void OnEvent(ref AbilityContext ctx)
    {
        if (ctx.eventType != BattleEventType.Attack)
            return;

        if (ctx.amount <= 0f)
            return;

        float rate = GetLifeStealRate();

        float healAmount = ctx.amount * rate;

        // Tier 3: 치명타 시 흡혈 2배
        if (rarity == AbilityRarity.Tier3 && ctx.isCritical)
        {
            healAmount *= 2f;
        }

        ctx.owner.Heal(healAmount);
    }

    private float GetLifeStealRate()
    {
        return rarity switch
        {
            AbilityRarity.Tier1 => 0.03f, // 3%
            AbilityRarity.Tier2 => 0.06f, // 6%
            AbilityRarity.Tier3 => 0.06f, // Tier3는 효과 추가
            _ => 0f
        };
    }
}
