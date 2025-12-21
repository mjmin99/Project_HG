using System;
using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;
using UnityEngine;

public class TestCharacterController : MonoBehaviour, IAttackable
{
    [SerializeField] private int characterId;
    [SerializeField] private float range;
    [SerializeField] private float fireSpeed;
    [SerializeField] private float hp;
    [SerializeField] private float shield;
    [SerializeField] public AttackType atkType;
    [SerializeField] private float atk;
    [SerializeField] private float def;
    [SerializeField] private bool isPoison;

    public Rigidbody rb;
    public BoxCollider col;

    private TestBulletController bulletPrefab;
    private StateMachine stateMachine;
    private Animator animator;
    
    private float maxHp;
    
    private readonly Dictionary<CharStateType, BaseState> stateDict = new(); 
    public void Init()
    {
        rb = gameObject.GetOrAddComponent<Rigidbody>();
        col = gameObject.GetOrAddComponent<BoxCollider>();
        animator = gameObject.GetOrAddComponent<Animator>();
        maxHp = hp;
        stateMachine = new StateMachine();
        stateDict.Add(CharStateType.Idle, new CharacterIdle(this) );
        stateDict.Add(CharStateType.Run, new CharacterRun(this));
        stateDict.Add(CharStateType.Attack, new CharacterAttack(this));
        stateDict.Add(CharStateType.Skill, new CharacterSkill(this));
        stateDict.Add(CharStateType.Hit, new CharacterHit(this));
        stateDict.Add(CharStateType.Dead, new CharacterDead(this));
        stateMachine.Initialize(stateDict[CharStateType.Idle]);
        if (atkType == AttackType.Ranged)
        {
            bulletPrefab = Resources.Load<TestBulletController>("Test/TestBullet");
        }
    }

    void Update()
    {
        stateMachine.Update();
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    void LateUpdate()
    {
        stateMachine.LateUpdate();
    }

    public void Attack(TestEnemyController controller)
    {
        switch (atkType)
        {
            case AttackType.Melee:
                var info = new AttackInfo(gameObject.layer, atk, isPoison);
                controller.TakeHit(info);
                break;
            case AttackType.Ranged:
                var bullet = Instantiate(bulletPrefab, gameObject.transform);
                bullet.Init(gameObject.layer, atk, isPoison);
                bullet.FireToPosition(controller.transform.position);
                break;
            default:
                Debug.Log("어택 타입 안정해짐");
                break;
        }
    }
    
    public void TakeHit(AttackInfo attackInfo)
    {
        int damage = (int)(attackInfo.atk * (1 - def / 100));
        // 해당 데미지를 Toast UI로 표현
        if (shield > 0 && damage > 0)
        {
            int shieldDamage = (int)Mathf.Clamp(damage, 0, shield);
            shield -= shieldDamage;
            damage -= shieldDamage;
        }

        if (damage <= 0) return;
        
        hp -= damage;
        
        if (hp <= 0)
        {
            hp = 0;
            stateMachine.ChangeState(
                stateDict[CharStateType.Dead]);
        }
        
        else
        {
            stateMachine.ChangeState(
                stateDict[CharStateType.Hit]);
        }
    }

    public void Heal(float amount)
    {
        int healAmount = (int)Mathf.Clamp(amount, 0, maxHp - hp);
        if (healAmount > 0)
        {
            hp += healAmount;
            // 힐 이펙트 및 Toast UI 생성
        }
    }

    public void GetShield(float amount)
    {
        shield += amount;
        // 쉴드 증가 Toast UI 생성 
    }
    
    public void PlayAnimation(int animKey)
    {
        animator.Play(animKey);
    }
}