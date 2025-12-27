using UnityEngine;

// 공용 스텟 어빌리티 
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
        float rate = AbilityTiers.StatRate(rarity, 0.10f, 0.20f, 0.35f);
        stats.hp *= (1f + rate);
    }
}

public class Ability_AttackUp : AbilityBase, IStatModifierAbility
{
    private readonly AbilityRarity rarity;
    public Ability_AttackUp(AbilityRarity r) { rarity = r; }
    public override int AbilityId => AbilityIds.AttackUp;
    public override string Name => "공격력 증가";
    public override AbilityRarity Rarity => rarity;
    public override AbilityScope Scope => AbilityScope.Common;

    public void ModifyStats(ref CharacterStats stats, ICombatActor owner)
    {
        float rate = AbilityTiers.StatRate(rarity, 0.10f, 0.20f, 0.35f);
        stats.attack *= (1f + rate);
    }
}

public class Ability_MagicAttackUp : AbilityBase, IStatModifierAbility
{
    private readonly AbilityRarity rarity;
    public Ability_MagicAttackUp(AbilityRarity r) { rarity = r; }
    public override int AbilityId => AbilityIds.MagicAttackUp;
    public override string Name => "마법공격력 증가";
    public override AbilityRarity Rarity => rarity;
    public override AbilityScope Scope => AbilityScope.Common;

    public void ModifyStats(ref CharacterStats stats, ICombatActor owner)
    {
        float rate = AbilityTiers.StatRate(rarity, 0.10f, 0.20f, 0.35f);
        stats.magicAttack *= (1f + rate);
    }
}

// 잠시 막아둠
// public class Ability_SkillDamageUp : AbilityBase, IStatModifierAbility
// {
//     private readonly AbilityRarity rarity;
//     public Ability_SkillDamageUp(AbilityRarity r) { rarity = r; }
//     public override int AbilityId => AbilityIds.SkillDamageUp;
//     public override string Name => "스킬 피해량 증가";
//     public override AbilityRarity Rarity => rarity;
//     public override AbilityScope Scope => AbilityScope.Common;
// 
//     public void ModifyStats(ref CharacterStats stats, ICombatActor owner)
//     {
//         float rate = AbilityTiers.StatRate(rarity, 0.08f, 0.16f, 0.28f);
//         stats.skillDamageMultiplier *= (1f + rate);
//     }
// }

public class Ability_CritRateUp : AbilityBase, IStatModifierAbility
{
    private readonly AbilityRarity rarity;
    public Ability_CritRateUp(AbilityRarity r) { rarity = r; }
    public override int AbilityId => AbilityIds.CritRateUp;
    public override string Name => "치명타 확률 증가";
    public override AbilityRarity Rarity => rarity;
    public override AbilityScope Scope => AbilityScope.Common;

    public void ModifyStats(ref CharacterStats stats, ICombatActor owner)
    {
        float add = AbilityTiers.StatRate(rarity, 0.03f, 0.06f, 0.10f);
        stats.critRate = Mathf.Clamp01(stats.critRate + add);
    }
}

public class Ability_AttackRangeUp : AbilityBase, IStatModifierAbility
{
    private readonly AbilityRarity rarity;
    public Ability_AttackRangeUp(AbilityRarity r) { rarity = r; }
    public override int AbilityId => AbilityIds.AttackRangeUp;
    public override string Name => "공격 사거리 증가";
    public override AbilityRarity Rarity => rarity;
    public override AbilityScope Scope => AbilityScope.Common;

    public void ModifyStats(ref CharacterStats stats, ICombatActor owner)
    {
        float rate = AbilityTiers.StatRate(rarity, 0.10f, 0.18f, 0.30f);
        stats.attackRange *= (1f + rate);
    }
}

public class Ability_AttackSpeedUp : AbilityBase, IStatModifierAbility
{
    private readonly AbilityRarity rarity;
    public Ability_AttackSpeedUp(AbilityRarity r) { rarity = r; }
    public override int AbilityId => AbilityIds.AttackSpeedUp;
    public override string Name => "공격 속도 증가";
    public override AbilityRarity Rarity => rarity;
    public override AbilityScope Scope => AbilityScope.Common;

    public void ModifyStats(ref CharacterStats stats, ICombatActor owner)
    {
        float rate = AbilityTiers.StatRate(rarity, 0.08f, 0.15f, 0.25f);
        stats.attackSpeed *= (1f + rate);
    }
}

// 임시로 막아둠
// 딜러(원거리) 전용: 공격력 추가 증가
//public class Ability_RangedAttackUp : AbilityBase, IStatModifierAbility
//{
//    private readonly AbilityRarity rarity;
//    public Ability_RangedAttackUp(AbilityRarity r) { rarity = r; }
//    public override int AbilityId => AbilityIds.RangedAttackUp;
//    public override string Name => "원거리 공격력 추가 증가";
//    public override AbilityRarity Rarity => rarity;
//    public override AbilityScope Scope => AbilityScope.Dealer_Ranged;
//
//    public void ModifyStats(ref CharacterStats stats, ICombatActor owner)
//    {
//        float rate = AbilityTiers.StatRate(rarity, 0.12f, 0.24f, 0.40f);
//        stats.attack *= (1f + rate);
//    }
//}

public class Ability_CritDamageUp : AbilityBase, IStatModifierAbility
{
    private readonly AbilityRarity rarity;

    public Ability_CritDamageUp(AbilityRarity r)
    {
        rarity = r;
    }

    public override int AbilityId => AbilityIds.CritDamageUp;
    public override string Name => "치명타 데미지 증가";
    public override AbilityRarity Rarity => rarity;
    public override AbilityScope Scope => AbilityScope.Common;

    public void ModifyStats(ref CharacterStats stats, ICombatActor owner)
    {
        float bonus = AbilityTiers.StatRate(
           rarity,
           0.15f, // Tier1
           0.30f, // Tier2
           0.50f  // Tier3
       );

        stats.critDamage += bonus;
    }
}
public class Ability_DefenseUp : AbilityBase, IStatModifierAbility
{
    private readonly AbilityRarity rarity;

    public Ability_DefenseUp(AbilityRarity r)
    {
        rarity = r;
    }

    public override int AbilityId => AbilityIds.DefenseUp;
    public override string Name => "방어력 증가";
    public override AbilityRarity Rarity => rarity;
    public override AbilityScope Scope => AbilityScope.Common;

    public void ModifyStats(ref CharacterStats stats, ICombatActor owner)
    {
        float rate = AbilityTiers.StatRate(
            rarity,
            0.10f, // Tier1
            0.20f, // Tier2
            0.35f  // Tier3
        );

        stats.defense *= (1f + rate);
    }
}

