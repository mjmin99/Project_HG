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

        stats.hp = model.baseHP + level * 10;
        stats.attack = model.baseAttack + level * 2;
        stats.magicAttack = model.baseMagicAttack + level * 3;
        stats.defense = model.baseDefense + level;

        stats.attackSpeed = model.baseAttackSpeed;
        stats.critRate = model.baseCritRate;
        stats.critDamage = model.baseCritDamage;

        stats.attackRange = model.attackRange; // ⭐ 이 줄 추가

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
}
