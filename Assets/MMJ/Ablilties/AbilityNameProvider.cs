using System.Collections.Generic;

// 디테일 패널에서 어빌리티의 이름을 제공해주는 용도 
public static class AbilityNameProvider
{
    private static readonly Dictionary<int, string> names = new()
    {
        // 공용
        { AbilityIds.MaxHPUp, "최대 체력 증가" },
        { AbilityIds.AttackUp, "공격력 증가" },
        { AbilityIds.MagicAttackUp, "마법 공격력 증가" },
        { AbilityIds.SkillDamageUp, "스킬 피해량 증가" },
        { AbilityIds.CritRateUp, "치명타 확률 증가" },
        { AbilityIds.AttackRangeUp, "공격 사거리 증가" },
        { AbilityIds.AttackSpeedUp, "공격 속도 증가" },
        { AbilityIds.SkillCooldownResetOnUse, "스킬 사용 시 쿨타임 초기화" },
        { AbilityIds.SkillBuffAfterUse, "스킬 사용 후 능력치 증가" },

        // 딜러
        { AbilityIds.CritBonusDamage, "치명타 시 추가 피해" },
        { AbilityIds.LifeStealOnAttack, "공격 시 흡혈" },
        { AbilityIds.BossDamageUp, "보스에게 가하는 피해 증가" },

        // 딜러(원거리)
        { AbilityIds.RangedAttackUp, "원거리 공격력 추가 증가" },
        { AbilityIds.EveryThirdAttackBonus, "매 3번째 공격 추가 피해" },
        { AbilityIds.DotOnHit, "공격 시 도트 피해 부여" },

        // 탱커
        { AbilityIds.DamageReduction, "받는 피해 감소" },
        { AbilityIds.TakeDamageForAlly, "파티원 대신 피해 받기" },
        { AbilityIds.StatusResistance, "상태이상 저항 확률 증가" },
        { AbilityIds.ShieldAndHealBoost, "보호막 및 회복량 증가" },

        // 서포터
        { AbilityIds.HealAura, "아군 지속 회복 오라" },
        { AbilityIds.HealCanCrit, "회복/보호막에 치명타 적용" },
        { AbilityIds.HealAndShieldIncrease, "회복 및 보호막 증가량 추가 증가" },
    };

    public static string GetName(int abilityId)
    {
        return names.TryGetValue(abilityId, out var name)
            ? name
            // : $"Unknown Ability ({abilityId})"; 
            : $"어빌리티 없음!";
    }
}
