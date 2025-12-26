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
}
