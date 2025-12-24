using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharController : MonoBehaviour, IAttackable
{
    public CharacterStats stats;
    public AttackType atkType;
    public Animator animator;
    public Rigidbody rb;
    public BoxCollider col;
    public StateMachine stateMachine;
    public readonly Dictionary<CharStateType, BaseState> stateDict = new();
    
    
    // 컨트롤러 전용 스탯
    private float maxHp;
    public float curHp { get; private set; }
    public float shield;
    public bool isRewinding;
    public bool isDead;
    
    public RaycastHit hitInfo; // 어택 시 사용하는 정보
    
    private BulletController bulletPrefab;
    private TimeRecorder timeRecorder;
    private float maxRecordTime;
    private const float HIT_COOLDOWN = 3f;
    private float hitTimer;
    
    public void Init(int characterId, float recordTime)
    {
        stats = Manager.Character.GetStats(characterId);
        rb = gameObject.GetOrAddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
        
        col = gameObject.GetOrAddComponent<BoxCollider>();
        col.isTrigger = true;
        col.center = new Vector3(0f, 0.25f, 0f);
        col.size = new Vector3(0.5f, 0.5f, 0.2f);
        
        animator = gameObject.GetOrAddComponent<Animator>();
        animator.runtimeAnimatorController 
            = Resources.Load<RuntimeAnimatorController>(
                $"Animation/Character/Controller/{characterId}");
        
        maxHp = stats.hp;
        curHp = maxHp;
        maxRecordTime =  recordTime;
        
        timeRecorder = new TimeRecorder(maxRecordTime, Time.fixedDeltaTime);
        
        stateMachine = new StateMachine();
        stateDict.Add(CharStateType.Idle, new CharacterIdle(this) );
        stateDict.Add(CharStateType.Run, new CharacterRun(this));
        stateDict.Add(CharStateType.Attack, new CharacterAttack(this));
        stateDict.Add(CharStateType.Skill, new CharacterSkill(this));
        stateDict.Add(CharStateType.Hit, new CharacterHit(this));
        stateDict.Add(CharStateType.Dead, new CharacterDead(this));
        stateDict.Add(CharStateType.Rewind, new CharacterRewind(this));
        
        stateMachine.Initialize(stateDict[CharStateType.Idle]);

        if (stats.atkType != AttackType.Melee)
        {
            bulletPrefab 
                = Resources.Load<BulletController>(
                    $"Prefab/Bullet/Character/{characterId}");
        }
        isRewinding = false;

    }
    private void Update()
    {
        stateMachine.Update();
        if (hitTimer > 0f) hitTimer += Time.deltaTime;
    }
    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
        
        if (!isRewinding) 
            timeRecorder.Record(transform.position, curHp, shield);
    }
    
    private void LateUpdate()
    {
        stateMachine.LateUpdate();
    }
        
    
    public bool HasHistory() => timeRecorder.HasHistory();
    public TestTimeInfo PopHistory() => timeRecorder.Pop();
    public TestTimeInfo PeekHistory() => timeRecorder.Peek();
    public void RewindTime()
    {
        stateMachine.ChangeState(stateDict[CharStateType.Rewind]);
    }
    public void FinishRewind()
    {
        timeRecorder.Clear();
        stateMachine.ChangeState(stateDict[CharStateType.Idle]);
    }
    
    public void Attack()
    {
        if (hitInfo.collider == null) return;
        
        switch (atkType)
        {
            case AttackType.Melee:
                var info = new AttackInfo(gameObject.layer, stats.attack);
                hitInfo.collider.GetComponent<IAttackable>().TakeHit(info);
                break;
            case AttackType.Ranged:
                var bullet = Instantiate(bulletPrefab, gameObject.transform);
                bullet.Init(gameObject.layer, stats.attack);
                bullet.FireToPosition(hitInfo.transform.position);
                break;
            case AttackType.Lazer:
                // OnTriggerEnter로 알아서 처리됨
            default:
                Debug.Log("어택 타입 안정해짐");
                break;
        }
    }
    
    public void TakeHit(AttackInfo attackInfo)
    {
        if (attackInfo.layer != LayerMask.NameToLayer("Enemy")) return;
        int damage = (int)(attackInfo.atk * (1 - stats.defense / 100));
        // 해당 데미지를 Toast UI로 표현
        
        if (shield > 0 && damage > 0)
        {
            int shieldDamage = (int)Mathf.Clamp(damage, 0, shield);
            shield -= shieldDamage;
            damage -= shieldDamage;
        }
        if (damage <= 0) return;
        
        curHp -= damage;
        
        if (curHp <= 0)
        {
            curHp = 0;
            stateMachine.ChangeState(
                stateDict[CharStateType.Dead]);
        }
        
        else
        {
            if (hitTimer > 0) return;
            hitTimer = HIT_COOLDOWN;
            stateMachine.ChangeState(stateDict[CharStateType.Hit]);
        }
    }
    public void Heal(float amount)
    {
        int healAmount = (int)Mathf.Clamp(amount, 0, maxHp - curHp);
        if (healAmount > 0)
        {
            curHp += healAmount;
            // 힐 이펙트 및 Toast UI 생성
        }
    }
    public void SetHp(float value)
    {
        curHp = value;
    }
    public void SetShield(float value)
    {
        shield = value;
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
