using System;
using UnityEngine;

public static class AbilityFactory
{
    public static IAbility Create(int abilityId, AbilityRarity rarity)
    {
        switch (abilityId)
        {
            // ===== 공용 Stat =====
            case AbilityIds.MaxHPUp: return new Ability_MaxHPUp(rarity);
            case AbilityIds.AttackUp: return new Ability_AttackUp(rarity);
            case AbilityIds.MagicAttackUp: return new Ability_MagicAttackUp(rarity);
            case AbilityIds.SkillDamageUp: return new Ability_SkillDamageUp(rarity);
            case AbilityIds.CritRateUp: return new Ability_CritRateUp(rarity);
            case AbilityIds.AttackRangeUp: return new Ability_AttackRangeUp(rarity);
            case AbilityIds.AttackSpeedUp: return new Ability_AttackSpeedUp(rarity);

            // ===== 공용 Event =====
            case AbilityIds.SkillCooldownResetOnUse: return new Ability_SkillCooldownReset(rarity);
            case AbilityIds.SkillBuffAfterUse: return new Ability_SkillBuffAfterUse(rarity);

            // ===== 딜러(근거리) =====
            case AbilityIds.CritBonusDamage: return new Ability_CritBonusDamage(rarity);
            case AbilityIds.LifeStealOnAttack: return new Ability_LifeSteal(rarity);
            case AbilityIds.BossDamageUp: return new Ability_BossDamageUp(rarity);

            // ===== 딜러(원거리) =====
            case AbilityIds.RangedAttackUp: return new Ability_RangedAttackUp(rarity);
            case AbilityIds.EveryThirdAttackBonus: return new Ability_EveryThirdAttackBonus(rarity);
            case AbilityIds.DotOnHit: return new Ability_DotOnHit(rarity);

            // ===== 탱커 =====
            case AbilityIds.DamageReduction: return new Ability_DamageReduction(rarity);
            case AbilityIds.TakeDamageForAlly: return new Ability_TakeDamageForAlly(rarity);
            case AbilityIds.StatusResistance: return new Ability_StatusResistance(rarity);
            case AbilityIds.ShieldAndHealBoost: return new Ability_ShieldAndHealBoost(rarity);

            // ===== 서포터 =====
            case AbilityIds.HealAura: return new Ability_HealAura(rarity);
            case AbilityIds.HealCanCrit: return new Ability_HealCanCrit(rarity);
            case AbilityIds.HealAndShieldIncrease: return new Ability_HealAndShieldIncrease(rarity);

            default:
                Debug.LogError($"[AbilityFactory] Unknown abilityId: {abilityId}");
                return null;
        }
    }
}
