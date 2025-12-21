public class Ability_CritBonusDamage : AbilityBase, IEventAbility
{
    private readonly AbilityRarity rarity;
    public Ability_CritBonusDamage(AbilityRarity r) { rarity = r; }
    public override int AbilityId => AbilityIds.CritBonusDamage;
    public override string Name => "치명타 시 추가 피해";
    public override AbilityRarity Rarity => rarity;
    public override AbilityScope Scope => AbilityScope.Dealer_Melee;

    public void OnEvent(ref AbilityContext ctx)
    {
        // 전투 코드가 "치명타 공격"에도 Attack 이벤트를 동일하게 쏘고 isCritical=true로 준다고 가정
        if (ctx.eventType != BattleEventType.Attack) return;
        if (!ctx.isCritical) return;
        if (ctx.amount <= 0f) return;

        float bonus = AbilityTiers.StatRate(rarity, 0.15f, 0.30f, 0.50f); // 추가 피해 비율
        ctx.amount *= (1f + bonus);
    }
}

public class Ability_LifeSteal : AbilityBase, IEventAbility
{
    private readonly AbilityRarity rarity;
    public Ability_LifeSteal(AbilityRarity r) { rarity = r; }
    public override int AbilityId => AbilityIds.LifeStealOnAttack;
    public override string Name => "공격 시 흡혈";
    public override AbilityRarity Rarity => rarity;
    public override AbilityScope Scope => AbilityScope.Dealer_Melee;

    public void OnEvent(ref AbilityContext ctx)
    {
        if (ctx.eventType != BattleEventType.Attack) return;
        if (ctx.amount <= 0f) return;

        float rate = AbilityTiers.StatRate(rarity, 0.03f, 0.06f, 0.08f);
        float heal = ctx.amount * rate;
        ctx.owner.Heal(heal);
    }
}

public class Ability_BossDamageUp : AbilityBase, IEventAbility
{
    private readonly AbilityRarity rarity;
    public Ability_BossDamageUp(AbilityRarity r) { rarity = r; }
    public override int AbilityId => AbilityIds.BossDamageUp;
    public override string Name => "보스에게 가하는 피해 증가";
    public override AbilityRarity Rarity => rarity;
    public override AbilityScope Scope => AbilityScope.Dealer_Melee; // 원하면 Dealer 공통으로 바꿔도 됨

    public void OnEvent(ref AbilityContext ctx)
    {
        if (ctx.eventType != BattleEventType.Attack) return;
        if (!ctx.targetIsBoss) return;

        float rate = AbilityTiers.StatRate(rarity, 0.10f, 0.20f, 0.35f);
        ctx.amount *= (1f + rate);
    }
}
