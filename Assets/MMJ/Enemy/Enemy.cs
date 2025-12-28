using System;
using UnityEngine;

[Serializable]
public class Enemy
{
    public int id;
    public string enemyName;

    public float attack;
    public float magicAttack;
    public float maxHP;
    public float attackSpeed;

    public float attackRange;

    public AttackType attackType;
    public float defense;

    // 프리팹 로드 경로 Resources/Prefabs/Enemies/{name}.prefab
    public string PrefabPath =>
        $"Prefabs/Enemies/{enemyName}";
}
