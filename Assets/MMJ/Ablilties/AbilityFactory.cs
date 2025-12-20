using System;
using UnityEngine;

public static class AbilityFactory
{
    public static IAbility Create(int abilityId, AbilityRarity rarity)
    {
        switch (abilityId)
        {
            // ===== 공용 =====
            case AbilityIds.MaxHPUp:
                return new Ability_MaxHPUp(rarity);

            // case AbilityIds.AttackUp: 
            //     return new Ability_AttackUp(rarity); 아직 미구현 ㅋ

            case AbilityIds.SkillCooldownResetOnUse:
                return new Ability_SkillCooldownReset(rarity);

            // ===== 딜러 근거리 =====
            case AbilityIds.LifeStealOnAttack:
                return new Ability_LifeSteal(rarity);

            // ===== 예외 =====
            default:
                Debug.LogError($"[AbilityFactory] Unknown AbilityId: {abilityId}");
                return null;
        }
    }
}
