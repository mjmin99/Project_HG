using UnityEngine;

public class Ability_MaxHPUp : AbilityBase, IStatModifierAbility
{
    private readonly AbilityRarity rarity;

    public Ability_MaxHPUp(AbilityRarity rarity)
    {
        this.rarity = rarity;
    }

    public override int AbilityId => AbilityIds.MaxHPUp;
    public override string Name => "최대 체력 증가";
    public override AbilityRarity Rarity => rarity;
    public override AbilityScope Scope => AbilityScope.Common;

    public void ModifyStats(ref CharacterStats stats, ICombatActor owner)
    {
        float rate = GetIncreaseRate();
        stats.hp *= (1f + rate);
    }

    private float GetIncreaseRate()
    {
        return rarity switch
        {
            AbilityRarity.Tier1 => 0.10f, // +10%
            AbilityRarity.Tier2 => 0.20f, // +20%
            AbilityRarity.Tier3 => 0.35f, // +35%
            _ => 0f
        };
    }
}
