using UnityEngine;

public class Ability_StatusResistance : AbilityBase, IEventAbility
{
    private readonly AbilityRarity rarity;
    public Ability_StatusResistance(AbilityRarity r) { rarity = r; }
    public override int AbilityId => AbilityIds.StatusResistance;
    public override string Name => "상태이상 저항 확률 증가";
    public override AbilityRarity Rarity => rarity;
    public override AbilityScope Scope => AbilityScope.Tanker;

    public void OnEvent(ref AbilityContext ctx)
    {
        if (ctx.eventType != BattleEventType.StatusApplyAttempt) return;

        float chance = AbilityTiers.Chance(rarity, 0.15f, 0.25f, 0.40f);
        if (Random.value <= chance)
            ctx.cancel = true;
    }
}
