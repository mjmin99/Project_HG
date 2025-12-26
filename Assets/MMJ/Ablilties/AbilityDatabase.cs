using System.Collections.Generic;
using UnityEngine;

// AbilityDatabase는 오직 뽑을 후보를 제공
public static class AbilityDatabase
{
    // ===== 내부 데이터 =====

    private static readonly List<int> CommonAbilities = new()
    {
        AbilityIds.MaxHPUp,
        AbilityIds.AttackUp,
        AbilityIds.MagicAttackUp,
        // AbilityIds.SkillDamageUp,
        AbilityIds.CritRateUp,
        AbilityIds.AttackRangeUp,
        AbilityIds.AttackSpeedUp,
        // AbilityIds.SkillCooldownResetOnUse,
        // AbilityIds.SkillBuffAfterUse,
        AbilityIds.CritDamageUp,
        AbilityIds.DefenseUp,
    };

    // private static readonly List<int> DealerMeleeAbilities = new()
    // {
    //     
    //     AbilityIds.LifeStealOnAttack,
    //     AbilityIds.BossDamageUp,
    // };

    // private static readonly List<int> DealerRangedAbilities = new()
    // {
    //     AbilityIds.RangedAttackUp,
    //     AbilityIds.EveryThirdAttackBonus,
    //     AbilityIds.DotOnHit,
    // };

    // private static readonly List<int> TankAbilities = new()
    // {
    //     AbilityIds.DamageReduction,
    //     AbilityIds.TakeDamageForAlly,
    //     AbilityIds.StatusResistance,
    //     AbilityIds.ShieldAndHealBoost,
    // };
    
    // private static readonly List<int> SupportAbilities = new()
    // {
    //     AbilityIds.HealAura,
    //     AbilityIds.HealCanCrit,
    //     AbilityIds.HealAndShieldIncrease,
    // };

    // ===== 공개 API =====

    public static List<AbilityInstance> GetPoolFor(CharacterModel model)
    {
        var pool = new List<AbilityInstance>();

        // 1️⃣ 공용 어빌리티
        AddAbilities(pool, CommonAbilities);

        // 2️⃣ 역할 / 공격타입별
        // switch (model.role)
        // {
        //     case CharacterRole.Tank:
        //         AddAbilities(pool, TankAbilities);
        //         break;
        // 
        //     case CharacterRole.Dealer:
        //         if (model.attackType == AttackType.Melee)
        //             AddAbilities(pool, DealerMeleeAbilities);
        //         else
        //             AddAbilities(pool, DealerRangedAbilities);
        //         break;
        // 
        //     case CharacterRole.Healer:
        //         AddAbilities(pool, SupportAbilities);
        //         break;
        // }

        return pool;
    }

    // ===== 내부 헬퍼 =====

    private static void AddAbilities(List<AbilityInstance> pool, List<int> ids)
    {
        foreach (var id in ids)
        {
            pool.Add(new AbilityInstance(id, RollRarity()));
        }
    }

    private static AbilityRarity RollRarity()
    {
        float r = Random.value;

        // 확률 테이블 (조절 포인트)
        if (r < 0.60f) return AbilityRarity.Tier1;
        if (r < 0.90f) return AbilityRarity.Tier2;
        return AbilityRarity.Tier3;
    }
}
