using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class TestCharacterController : MonoBehaviour
{
    [SerializeField] private int characterId;
    [SerializeField] private float range;
    [SerializeField] private float hp;
    [SerializeField] private float shield;
    [SerializeField] public AttackType atkType;
    [SerializeField] private float atk;
    [SerializeField] private float def;

    public Rigidbody rb;
    public BoxCollider col;
    
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer($"Enemy")) return;
        var info = collision.gameObject.GetComponent<EnemyAttackInfo>();
        TakeHit(info);
    }

    private void TakeHit(EnemyAttackInfo attackInfo)
    {
        int damage = (int)(attackInfo.atk * (1 - def / 100));
        // 해당 데미지를 Toast UI로 표현
        if (shield > 0 && damage > 0)
        {
            int shieldDamage = (int)Mathf.Clamp(damage, 0, shield);
            shield -= shieldDamage;
            damage -= shieldDamage;
        }

        if (damage > 0)
        {
            hp -= damage;
        }
    }

    public void Heal(float hp)
    {
        this.hp = Mathf.Clamp(this.hp + hp, 0, maxHp);
        // 힐 이펙트 생성
    }

    public void GetShield(float amount)
    {
        shield += amount;
    }
    
    public void PlayAnimation(int animKey)
    {
        animator.Play(animKey);
    }
}