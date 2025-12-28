using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterInstance
{
    public int id;
    public bool isOwned = false;

    public int level = 1;
    public int exp = 0;

    public int shard = 0;  // 조각(중복 보상)

    // 어빌리티
    // 이전에 쓰던 거 public List<AbilityInstance> abilities = new List<AbilityInstance>();
    // 바꾼이유 : 슬롯 재설정을 위해서
    public List<AbilitySlot> abilitySlots = new();

    public CharacterStats GetStats(CharacterModel model)
    {
        CharacterStats stats = new CharacterStats();

        stats.atkType = model.attackType;
        stats.hp = model.baseHP + level * 10;
        stats.attack = model.baseAttack + level * 2;
        stats.magicAttack = model.baseMagicAttack + level * 3;
        stats.defense = model.baseDefense + level;

        stats.attackSpeed = model.baseAttackSpeed;
        stats.critRate = model.baseCritRate;
        stats.critDamage = model.baseCritDamage;

        stats.attackRange = model.attackRange;

        ApplyAbilityStatModifiers(ref stats);
        // ---------------------------------------------- 추가 중
        var setBonuses = GetAbilitySetBonuses();

        foreach (var pair in setBonuses)
        {
            switch (pair.Key)
            {
                case StatType.HP:
                    stats.hp *= (1f + pair.Value);
                    break;

                case StatType.Attack:
                    stats.attack *= (1f + pair.Value);
                    break;

                case StatType.MagicAttack:
                    stats.magicAttack *= (1f + pair.Value);
                    break;

                case StatType.Defense:
                    stats.defense *= (1f + pair.Value);
                    break;

                case StatType.AttackSpeed:
                    stats.attackSpeed *= (1f + pair.Value);
                    break;

                case StatType.CritRate:
                    stats.critRate *= (1f + pair.Value);
                    break;

                case StatType.CritDamage:
                    stats.critDamage *= (1f + pair.Value);
                    break;

                case StatType.AttackRange:
                    stats.attackRange *= (1f + pair.Value);
                    break;
            }
        }

        return stats;
    }

    public int GetUnlockedAbilitySlotCount(CharacterModel model)
    {
        if (model == null) return 0;

        // 시작 1개 + 레벨업당 1개
        int unlocked = 1 + (level - 1);

        // 최대치는 rarity
        return Mathf.Clamp(unlocked, 1, model.MaxAbilitySlotCount);
    }

    public void SyncAbilitySlots(CharacterModel model)
    {
        int unlockedCount = GetUnlockedAbilitySlotCount(model);

        while (abilitySlots.Count < unlockedCount)
        {
            abilitySlots.Add(new AbilitySlot
            {
                ability = null,
                isLocked = false
            });
        }
    }

    private void ApplyAbilityStatModifiers(ref CharacterStats stats)
    {
        if (abilitySlots == null)
            return;

        foreach (var slot in abilitySlots)
        {
            if (slot.ability == null)
                continue;

            var ability = slot.ability;

            switch (ability.abilityId)
            {
                // ===== 기본 스탯 =====

                case AbilityIds.MaxHPUp:
                    stats.hp += AbilityTiers.Value(
                        ability.rarity,
                        50,    // Tier1
                        100,   // Tier2
                        200    // Tier3
                    );
                    break;

                case AbilityIds.AttackUp:
                    stats.attack += AbilityTiers.Value(
                        ability.rarity,
                        5,
                        10,
                        20
                    );
                    break;

                case AbilityIds.MagicAttackUp:
                    stats.magicAttack += AbilityTiers.Value(
                        ability.rarity,
                        6,
                        12,
                        25
                    );
                    break;

                case AbilityIds.DefenseUp:
                    stats.defense += AbilityTiers.Value(
                        ability.rarity,
                        5,
                        10,
                        20
                    );
                    break;

                case AbilityIds.AttackSpeedUp:
                    stats.attackSpeed += AbilityTiers.Value(
                        ability.rarity,
                        0.05f,
                        0.10f,
                        0.20f
                    );
                    break;

                case AbilityIds.AttackRangeUp:
                    stats.attackRange += AbilityTiers.Value(
                        ability.rarity,
                        0.5f,
                        1.0f,
                        1.5f
                    );
                    break;

                case AbilityIds.CritRateUp:
                    stats.critRate += AbilityTiers.Value(
                        ability.rarity,
                        0.05f,
                        0.10f,
                        0.15f
                    );
                    break;

                case AbilityIds.CritDamageUp:
                    stats.critDamage += AbilityTiers.Value(
                        ability.rarity,
                        0.15f,
                        0.30f,
                        0.50f
                    );
                    break;

                // ===== 안전장치 =====
                default:
                    // 아직 스탯화 안 된 어빌리티
                    break;
            }
        }
    
    }
    // ------------------------------------------------------ 추가 중
    public Dictionary<int, int> GetAbilityCounts()
    {
        var dict = new Dictionary<int, int>();

        foreach (var slot in abilitySlots)
        {
            if (slot.ability == null)
                continue;

            int id = slot.ability.abilityId;

            if (!dict.ContainsKey(id))
                dict[id] = 0;

            dict[id]++;
        }

        return dict;
    }

    private static StatType GetStatTypeByAbility(int abilityId)
    {
        return abilityId switch
        {
            AbilityIds.MaxHPUp => StatType.HP,
            AbilityIds.AttackUp => StatType.Attack,
            AbilityIds.MagicAttackUp => StatType.MagicAttack,
            AbilityIds.AttackSpeedUp => StatType.AttackSpeed,
            AbilityIds.DefenseUp => StatType.Defense,
            AbilityIds.CritRateUp => StatType.CritRate,
            AbilityIds.CritDamageUp => StatType.CritDamage,
            AbilityIds.AttackRangeUp => StatType.AttackRange,

            _ => StatType.None
        };
    }

    public enum StatType
    {
        None,
        HP,
        Attack,
        MagicAttack,
        Defense,
        AttackSpeed,
        CritRate,
        CritDamage,
        AttackRange
    }

    private static float GetSetBonusRate(int count)
    {
        return count switch
        {
            >= 5 => 0.35f, // 35%
            >= 4 => 0.20f,
            >= 3 => 0.10f,
            >= 2 => 0.05f,
            _ => 0f
        };
    }

    public Dictionary<StatType, float> GetAbilitySetBonuses()
    {
        var result = new Dictionary<StatType, float>();
        var counts = GetAbilityCounts();

        foreach (var pair in counts)
        {
            int abilityId = pair.Key;
            int count = pair.Value;

            if (count < 2)
                continue;

            StatType statType = GetStatTypeByAbility(abilityId);
            if (statType == StatType.None)
                continue;

            float bonusRate = GetSetBonusRate(count);
            if (bonusRate <= 0f)
                continue;

            if (!result.ContainsKey(statType))
                result[statType] = 0f;

            result[statType] += bonusRate;
        }

        return result;
    }

    public HashSet<int> GetSetBonusAbilityIds()
    {
        var result = new HashSet<int>();
        var counts = GetAbilityCounts();

        foreach (var pair in counts)
        {
            if (pair.Value >= 2)
                result.Add(pair.Key);
        }

        return result;
    }
}
