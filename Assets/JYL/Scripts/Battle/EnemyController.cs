using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

public class EnemyController : MonoBehaviour, IAttackable
{
    public Enemy enemyInfo;

    private float maxHp;
    public ReactiveProperty<float> curHp = new();
    
    private AttackType atkType;
    
    private float curShield;

    private DamageUI damageUi;
    private EnemyHpPresenter hpBar;

    public Animator animator;
    public BoxCollider col;

    public RaycastHit hitInfo;
    public StateMachine stateMachine;
    public readonly Dictionary<CharStateType, BaseState> stateDict = new();
    private const float HIT_COOLDOWN = 3f;
    private float hitCoolTimer;
    public ReactiveProperty<bool> isDead = new();

    private const string ANIM_CONT_PATH = "Battle/Enemy/Controllers/";

    public float stunTime;
    
    public bool[] skillDropHp = new bool[4];

    public void Init(Enemy info, DamageUI damageUI, RectTransform uiCanvas = null)
    {
        // 스프라이트 렌더러 추가
        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.flipX = true;
        
        // 애니메이터 설정
        animator = gameObject.AddComponent<Animator>();
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(ANIM_CONT_PATH + info.enemyName);
        animator.Play("Idle");
        
        // 콜라이더 설정
        col = gameObject.GetOrAddComponent<BoxCollider>();
        col.center = new Vector3(0, 0.25f, 0);
        col.size = new Vector3(0.5f, 0.5f, 0.2f);

        // 상태 설정
        stateMachine = new StateMachine();
        stateDict.Add(CharStateType.Idle, new EnemyIdle(this));
        stateDict.Add(CharStateType.Attack, new EnemyAttack(this));
        stateDict.Add(CharStateType.Hit, new EnemyHit(this));
        stateDict.Add(CharStateType.Dead, new EnemyDead(this));
        stateDict.Add(CharStateType.Stun, new EnemyStun(this));
        stateMachine.Initialize(stateDict[CharStateType.Idle]);

        // 스텟설정
        enemyInfo = info;
        maxHp = info.maxHP;
        curHp.Value = info.maxHP;
        
        // UI 설정
        damageUi = damageUI;
        
        if (uiCanvas)
        {
            SetEnemyUI(uiCanvas);
        }
        else
        {
            SetEnemyUI();
        }
    }

    // UI 설정
    private void SetEnemyUI(RectTransform uiCanvas = null)
    {
        EnemyHpPresenter hpUi;
        if (uiCanvas) // 보스일 경우, UI 캔버스를 넣음
        {
            hpUi = Resources.Load<BossHpPresenter>($"UI/Battle/BossHpPanel");
            hpBar = Instantiate(hpUi, uiCanvas);
        }
        else // 보스가 아닌 일반 에너미의 경우, 데미지 출력 WorldSpace 캔버스를 사용함
        {
            var enemyCanvas = damageUi.CheckChildCanvas(transform);
            hpUi = Resources.Load<EnemyHpPresenter>($"UI/Battle/EnemyHpPanel"); 
            hpBar = Instantiate(hpUi, enemyCanvas);
        }
        
        hpBar.Init(enemyInfo.enemyName, maxHp);
        
        curHp.Subscribe(hpBar.UpdateUI).AddTo(this); //UniRx로 구독
    }

    public void PlayAnimation(int key) => animator.Play(key);

    private void Update()
    {
        stateMachine.Update();
        if (hitCoolTimer > 0f) hitCoolTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    private void LateUpdate()
    {
        stateMachine.LateUpdate();
    }

    public void ChangeState(CharStateType state) => stateMachine.ChangeState(stateDict[state]);

    public void Attack()
    {
        var attackInfo = new AttackInfo(LayerMask.NameToLayer("Enemy"), enemyInfo.attack);
        hitInfo.collider.GetComponent<IAttackable>().TakeHit(attackInfo);
    }

    public void TakeHit(AttackInfo info)
    {
        if (info.layer != LayerMask.NameToLayer("Player")) return;

        int damage = stunTime > 0
            ? (int)(info.atk * (1 - enemyInfo.defense / 200))
            : (int)(info.atk * (1 - enemyInfo.defense / 100));

        // 해당 데미지를 Toast UI로 표현
        damageUi.ShowDamageEffect(damage, transform, false).Forget(); // ToAsyncLazy()로 값을 받을 필요없음. Forget()으로 가비지 없앨 수 있음

        if (curShield > 0 && damage > 0)
        {
            int shieldDamage = (int)Mathf.Clamp(damage, 0, curShield);
            curShield -= shieldDamage;
            damage -= shieldDamage;
        }

        if (damage <= 0) return;

        curHp.Value -= damage;

        if (curHp.Value <= 0)
        {
            curHp.Value = 0;
            stateMachine.ChangeState(stateDict[CharStateType.Dead]);
            isDead.Value = true;
        }

        else if (hitCoolTimer <= 0f && stunTime <= 0f)
        {
            stateMachine.ChangeState(stateDict[CharStateType.Hit]);
            hitCoolTimer = HIT_COOLDOWN;
        }
    }

    public void GetStun(float stunTime)
    {
        // TODO: 스턴 효과 애니메이션 재생 필요
        this.stunTime = stunTime;
        stateMachine.ChangeState(stateDict[CharStateType.Stun]);
    }

    public float GetHpPercent()
    {
        return curHp.Value / maxHp ;
    }
}
