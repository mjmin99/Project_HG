using System;
using UnityEngine;

[System.Serializable]
public struct CharacterStats
{
    public AttackType atkType;
    public float hp;
    public float attack;
    public float magicAttack;
    public float defense;

    public float attackSpeed;
    public float critRate;
    public float critDamage;

    public float attackRange;
    public float skillDamageMultiplier; // 어빌리티 스킬 데미지 배율
}
