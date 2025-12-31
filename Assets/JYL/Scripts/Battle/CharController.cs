using System.Collections.Generic;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEditor.UIElements;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharController : MonoBehaviour, IAttackable
{
    public int characterId;
    public CharacterStats stats;
    public Animator animator;
    public Rigidbody rb;
    public BoxCollider col;
    public StateMachine stateMachine;
    public readonly Dictionary<CharStateType, BaseState> stateDict = new();
    public Skill skillPrefab;

    // 해싱
    private const string CONTROLLER_PATH = "Battle/Characters/Controllers/";
    private const string BULLET_PATH = "Battle/Characters/Bullets/";
    private const string LAZER_PATH = "Battle/Characters/Lazer/";
    private const string SKILL_PATH = "Skill/";
    
    // 컨트롤러 전용 스탯
    public float maxHp;
    public ReactiveProperty<float> curHp = new();
    public float shield;
    public bool isRewinding;
    
    public ReactiveProperty<bool> isDead = new();
    
    public RaycastHit hitInfo; // 어택 시 사용하는 정보

    private AttackInfo atkInfo;
    private TimeRecorder timeRecorder;
    private Parrying parry;
    private readonly Stack<Skill> skillPool = new();

    private DamageUI damageUi;
    private ObjectPool bulletPool;
    
    private float maxRecordTime;
    private const float HIT_COOLDOWN = 3f;
    private float hitTimer;

    private readonly Vector3 lazerVec = Vector3.up * 0.25f - Vector3.back * 0.05f + Vector3.right * 0.2f;
    private readonly Vector3 bulletVec = Vector3.up * 0.2f + Vector3.right * 0.1f;
    
    public void Init(int characterID, CharacterStats charStats, float recordTime, DamageUI damageUI)
    {
        characterId = characterID;
        gameObject.AddComponent<SpriteRenderer>();
        
        damageUi = damageUI;
        
        rb = gameObject.GetOrAddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
        
        col = gameObject.GetOrAddComponent<BoxCollider>();
        col.isTrigger = true;
        col.center = new Vector3(0f, 0.25f, 0f);
        col.size = new Vector3(0.5f, 0.5f, 0.2f);
        
        var inst = Manager.Character.instances[characterID];
        var model = Manager.Character.models[characterID];
        
        animator = gameObject.GetOrAddComponent<Animator>();
        animator.runtimeAnimatorController 
            = Resources.Load<RuntimeAnimatorController>(
                CONTROLLER_PATH + model.characterName);
        maxRecordTime =  recordTime;
        
        timeRecorder = new TimeRecorder(maxRecordTime, Time.fixedDeltaTime);
        
        stateMachine = new StateMachine();
        stateDict.Add(CharStateType.Idle, new CharacterIdle(this) );
        stateDict.Add(CharStateType.Run, new CharacterRun(this));
        stateDict.Add(CharStateType.Attack, new CharacterAttack(this));
        // stateDict.Add(CharStateType.Skill, new CharacterSkill(this));
        stateDict.Add(CharStateType.Hit, new CharacterHit(this));
        stateDict.Add(CharStateType.Dead, new CharacterDead(this));
        stateDict.Add(CharStateType.Rewind, new CharacterRewind(this));
        
        stateMachine.Initialize(stateDict[CharStateType.Idle]);
        
        isRewinding = false;

        // 스텟 설정
        stats = charStats;
        maxHp = charStats.hp;
        curHp.Value = maxHp;
        
        // 원거리나 레이저 타입일 경우 오브젝트 풀 생성
        if (charStats.atkType == AttackType.Ranged)
        {
            var bulletPrefab 
                = Resources.Load<BulletController>(
                    BULLET_PATH+model.characterName);
            if (bulletPrefab == null)
            {
                bulletPrefab = Resources.Load<BulletController>(BULLET_PATH + "TestBullet");
            }
            var go = new GameObject($"BulletPool_{model.characterName}");
            go.transform.SetParent(transform);
            bulletPool = go.AddComponent<ObjectPool>();
            bulletPool.CreatePool(bulletPrefab);
        }
        else if(charStats.atkType == AttackType.Lazer)
        {
            var lazerPrefab 
                = Resources.Load<LazerController>(
                    LAZER_PATH+model.characterName);
            var go = new GameObject($"LazerPool_{model.characterName}");
            go.transform.SetParent(transform);
            bulletPool = go.AddComponent<ObjectPool>();
            bulletPool.CreatePool(lazerPrefab);
        }

        SkillType type;
        // 스킬 정보 가져오기
        switch (model.role)
        {
            case CharacterRole.Dealer:
                type = SkillType.StrongAttack;
                break;
            case CharacterRole.Healer:
                type = SkillType.AllHeal;
                break;
            case CharacterRole.Tank:
                type = SkillType.Parrying;
                break;
            default:
                type = SkillType.StrongAttack;
                break;
        }
        skillPrefab = Resources.Load<Skill>(SKILL_PATH + type);
        // AttackInfo 초기화
        atkInfo = new AttackInfo(gameObject.layer, stats.attack, false);
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
            timeRecorder.Record(transform.position, curHp.Value, shield);
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
        // 치명타 판단
        float crit = Random.Range(0f, 1f);
        bool isCritical = stats.critRate > crit;
        // 치명타 고려 데미지 판단은 각 어택타입에 따라 달라짐
        
        switch (stats.atkType)
        {
            case AttackType.Melee:
                float attackDamage = isCritical ? stats.critDamage * stats.attack : stats.attack;
                atkInfo.atk = attackDamage;
                atkInfo.isCritical = isCritical;
                hitInfo.collider.GetComponent<IAttackable>().TakeHit(atkInfo);
                Manager.Audio.PlaySfx("MeleeAttack");
                break;
            case AttackType.Ranged:
                attackDamage = isCritical ? stats.critDamage * stats.attack : stats.attack;
                // TODO: 힐러일 경우 전체 힐. 이펙트 재생 필요함
                if (stats.role == CharacterRole.Healer)
                {
                    foreach (var c in Manager.Game.Characters)
                    {
                        c.Heal(stats.magicAttack);
                    }
                    break;
                }
                var bullet = bulletPool.GetObject() as BulletController;
                if (bullet != null)
                {
                    bullet.transform.position = transform.position + bulletVec;
                    
                    if (!bullet.isInit) bullet.Init(gameObject.layer, attackDamage);
                    bullet.FireToPosition(hitInfo.transform.position, isCritical);
                    Manager.Audio.PlaySfx("BulletAttack");
                }
                break;
            case AttackType.Lazer:
                float magicAttack = isCritical ? stats.critDamage * stats.magicAttack : stats.magicAttack;
                // OnTriggerEnter로 알아서 처리됨
                var lazer = bulletPool.GetObject() as LazerController;
                if (lazer != null)
                {
                    lazer.transform.position = transform.position + lazerVec;

                    if (!lazer.isInit) lazer.Init(gameObject.layer, magicAttack);
                    
                    lazer.InitiateLazer(isCritical);
                    Manager.Audio.PlaySfx("Lazer");
                }
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
        Skill newSkill;
        if (skillPool.Count <= 0)
        {
            newSkill = Instantiate(skillPrefab,transform);
            newSkill.Init(skillPool);
        }
        else
        {
            newSkill =  skillPool.Pop();
        }
        
        switch (skillPrefab.skillType)
        {
            case SkillType.StrongAttack:
                newSkill.transform.position = transform.position + Vector3.right * 0.25f;
                if (hitInfo.collider == null) return false;
                var info = new AttackInfo(gameObject.layer, stats.attack * 5f, false);
                hitInfo.collider.GetComponent<IAttackable>().TakeHit(info);
                newSkill.SkillEffect();
                break;
            case SkillType.Parrying:
                parry = newSkill as Parrying;
                if (parry != null)
                {
                    parry.transform.position = transform.position + Vector3.right * 0.25f;
                    parry.SkillEffect();
                }
                break;
            case SkillType.AllHeal:
                newSkill.SkillEffect();
                newSkill.transform.position = transform.position;
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
        if (parry != null)
        {
            if (parry.isParrying)
            {
                parry.SuccessParry();
                hitInfo.collider.GetComponent<EnemyController>().GetStun(parry.stunTime);
                return;
            }
        }
        int damage = (int)(attackInfo.atk * (1 - stats.defense / 100));
        
        // 해당 데미지를 Toast UI로 표현
        damageUi.ShowDamageEffect(damage, transform, true, false).Forget();
        
        if (shield > 0 && damage > 0)
        {
            int shieldDamage = (int)Mathf.Clamp(damage, 0, shield);
            shield -= shieldDamage;
            damage -= shieldDamage;
        }
        if (damage <= 0) return;
        
        curHp.Value -= damage;
        
        if (curHp.Value <= 0)
        {
            curHp.Value = 0;
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
        if (isDead.Value) return;
        
        int healAmount = (int)Mathf.Clamp(amount, 0, maxHp - curHp.Value);
        if (healAmount > 0)
        {
            curHp.Value += healAmount;
            // TODO : 힐 이펙트 및 Toast UI 생성
            damageUi.ShowHealEffect(healAmount, transform).Forget();
        }
    }
    
    public void SetHp(float value)
    {
        curHp.Value = value;
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