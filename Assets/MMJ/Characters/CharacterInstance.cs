using System;
using UnityEngine;

[Serializable]
public class CharacterInstance
{
    public int id;
    public bool isOwned = false;

    public int level = 1;
    public int exp = 0;

    public int shard = 0;  // 조각(중복 보상)

    public CharacterStats GetStats(CharacterModel model)
    {
        CharacterStats stats = new()
        {
            maxHp = model.baseHP + level * 10,
            attack = model.baseAttack + level * 2,
            magicAttack = model.baseMagicAttack + level * 3,
            defense = model.baseDefense + level,
            attackSpeed = model.baseAttackSpeed,
            critRate = model.baseCritRate,
            critDamage = model.baseCritDamage,
            attackRange = model.attackRange // ⭐ 이 줄 추가
        };

        return stats;
    }
}
