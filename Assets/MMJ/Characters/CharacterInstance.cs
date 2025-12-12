using System;
using UnityEngine;

[Serializable]
public class CharacterInstance
{
    public int id;
    public bool isOwned = false;

    public int level = 1;
    public int exp = 0;

    public int star = 1;   // 승급(1~5)
    public int shard = 0;  // 조각(중복 보상)

    public CharacterStats GetStats(CharacterModel model)
    {
        CharacterStats stats = new CharacterStats();

        // 기본 스탯 + 레벨 성장
        stats.hp = model.baseHP + (level - 1) * 5;
        stats.attack = model.baseAttack + (level - 1) * 2;
        stats.magicAttack = model.baseMagicAttack + (level - 1) * 2;
        stats.defense = model.baseDefense + (level - 1) * 1;

        // 그대로 적용되는 스탯
        stats.attackSpeed = model.baseAttackSpeed;
        stats.critRate = model.baseCritRate;
        stats.critDamage = model.baseCritDamage;
        stats.attackRange = model.attackRange;

        // 승급 보정
        stats.hp += star * 10;
        stats.attack += star * 5;

        return stats;
    }
}
