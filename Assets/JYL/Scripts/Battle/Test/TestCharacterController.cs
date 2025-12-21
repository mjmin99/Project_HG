using System.Collections.Generic;
using UnityEngine;

public class TestCharacterController : MonoBehaviour, IAttackable
{
    [SerializeField] private int characterId;
    [SerializeField] public float range;
    [SerializeField] private float fireSpeed;
    [SerializeField] private float hp;
    [SerializeField] private float shield;
    [SerializeField] public AttackType atkType;
    [SerializeField] private float atk;
    [SerializeField] private float def;
    [SerializeField] private bool isPoison;

    public Animator animator;
    public Rigidbody rb;
    public BoxCollider col;
    public StateMachine stateMachine;

    public Collider[] rayHit;
    public readonly Dictionary<CharStateType, BaseState> stateDict = new(); 

    private TestBulletController bulletPrefab;
    
    private TimeRecorder timeRecorder;
    
    private float maxRecordTime;
    private float maxHp;
    
    
    
    public void Init(float recordTime)
    {
        rb = gameObject.GetOrAddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
        
        col = gameObject.GetOrAddComponent<BoxCollider>();
        col.isTrigger = true;

        animator = gameObject.GetOrAddComponent<Animator>();
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"Test/{characterId}_AnimController"); 
        
        maxHp = hp;
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
        
        if (atkType == AttackType.Ranged)
        {
            bulletPrefab = Resources.Load<TestBulletController>("Test/TestBullet");
        }
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
        if (stateMachine.CurrentState != stateDict[CharStateType.Rewind])
        {
            RecordTime();
        }
    }
    
    private void LateUpdate()
    {
        stateMachine.LateUpdate();
    }

    private void RecordTime()
    {
        timeRecorder.Record(new TestTimeInfo(transform, hp, shield));
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
        if (attackInfo.layer != LayerMask.NameToLayer("Enemy")) return;
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

    public void SetHp(float value)
    {
        hp = value;
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