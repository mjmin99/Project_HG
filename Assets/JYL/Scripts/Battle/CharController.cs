using System;
using System.Collections.Generic;
using UniRx;
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

    // 해싱
    private const string CONTROLLER_PATH = "Battle/Characters/Controllers/";
    private const string BULLET_PATH = "Battle/Characters/Bullets/";
    private const string SKILL_PATH = "Skill/";
    
    // 컨트롤러 전용 스탯
    private float maxHp;
    private float curHp;
    public float shield;
    public bool isRewinding;
    
    public ReactiveProperty<bool> isDead;
    
    public RaycastHit hitInfo; // 어택 시 사용하는 정보
    
    private BulletController bulletPrefab;
    private TimeRecorder timeRecorder;
    private Skill skill;
    
    private float maxRecordTime;
    private const float HIT_COOLDOWN = 3f;
    private float hitTimer;
    
    public void Init(int characterId, CharacterStats charStats, float recordTime)
    {
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
                CONTROLLER_PATH + characterId);
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
        
        isRewinding = false;

        // 스텟 설정
        stats = charStats;
        maxHp = charStats.hp;
        curHp = maxHp;
        
        if (charStats.atkType != AttackType.Melee)
        {
            bulletPrefab 
                = Resources.Load<BulletController>(
                    BULLET_PATH+characterId);
        }

        var inst = Manager.Character.instances[characterId];
        // 스킬 정보 가져오기
        skill = Resources.Load<Skill>(SKILL_PATH + inst.skillType);
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
        
    // TimeRecord 관련 함수
    public bool HasHistory() => timeRecorder.HasHistory(); // 기록이 있는지 확인
    public TestTimeInfo PopHistory() => timeRecorder.Pop(); // 기록을 꺼냄
    public TestTimeInfo PeekHistory() => timeRecorder.Peek(); // 다음 기록 확인
    
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
                // 힐러일 경우 전체 힐
                if (stats.role == CharacterRole.Healer)
                {
                    foreach (var c in Manager.Game.Characters)
                    {
                        c.Heal(stats.magicAttack);
                    }
                    break;
                }
                var bullet = Instantiate(bulletPrefab, gameObject.transform);
                bullet.Init(gameObject.layer, stats.magicAttack);
                bullet.FireToPosition(hitInfo.transform.position);
                break;
            case AttackType.Lazer:
                // OnTriggerEnter로 알아서 처리됨
                bullet = Instantiate(bulletPrefab, gameObject.transform);
                bullet.Init(gameObject.layer,stats.magicAttack);
                break;
            default:
                Debug.Log("어택 타입 안정해짐");
                break;
        }
    }
    // 스킬 아이콘 클릭으로 사용. 사용에 성공 시 true 반환
    // 배틀 매니저에서, isGameOver일 경우 조작 막아야 함
    public bool UseSkill() 
    {
        var go = Instantiate(skill,transform);
        go.transform.position = transform.position + Vector3.up * 0.25f;
        go.Init();
        go.SkillEffect();
        
        switch (skill.skillType)
        {
            case SkillType.StrongAttack:
                if (hitInfo.collider == null) return false;
                var info = new AttackInfo(gameObject.layer, stats.attack * 5f);
                hitInfo.collider.GetComponent<IAttackable>().TakeHit(info);
                break;
            case SkillType.Parrying:
                
                break;
            case SkillType.AllHeal:
                foreach (var c in Manager.Game.Characters)
                {
                    c.Heal(stats.magicAttack);
                }
                break;
        }

        return true;
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
            // TODO : 힐 이펙트 및 Toast UI 생성
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