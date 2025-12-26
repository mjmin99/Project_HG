using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class EnemyController : MonoBehaviour, IAttackable
{
    public Enemy enemyInfo;

    private float maxHp;
    public float curHp;


    private float atk;
    private float mAtk;
    private float atkSpeed;
    private float atkRange;
    private float defense;
    private AttackType atkType;
    
    private float curShield;

    [SerializeField] private TestDamageUI damageUi;
    [SerializeField] private RectTransform uiCanvas;

    public Animator animator;
    public BoxCollider col;

    public RaycastHit hitInfo;
    public StateMachine stateMachine;
    public readonly Dictionary<CharStateType, BaseState> stateDict = new();
    private const float HIT_COOLDOWN = 3f;
    private float hitCoolTimer;
    public ReactiveProperty<bool> isDead;

    public void Init(Enemy info)
    {
        animator = gameObject.GetOrAddComponent<Animator>();
        col = gameObject.GetComponent<BoxCollider>();

        animator.Play("Idle");
        col.center = new Vector3(0, 0.25f, 0);
        col.size = new Vector3(0.5f, 0.5f, 0.2f);

        damageUi.Init(uiCanvas);

        stateMachine = new StateMachine();
        stateDict.Add(CharStateType.Idle, new EnemyIdle(this));
        stateDict.Add(CharStateType.Attack, new EnemyAttack(this));
        stateDict.Add(CharStateType.Hit, new EnemyHit(this));
        stateDict.Add(CharStateType.Dead, new EnemyDead(this));
        stateMachine.Initialize(stateDict[CharStateType.Idle]);

        // 스텟설정
        maxHp = info.maxHP;
        curHp = info.maxHP;
        atk = info.attack;
        mAtk = info.magicAttack;
        atkSpeed = info.attackSpeed;
        atkRange = info.attackRange;
        defense = info.defense;
        atkType = info.attackType;

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
        var attackInfo = new AttackInfo(LayerMask.NameToLayer("Enemy"), atk);
        hitInfo.collider.GetComponent<IAttackable>().TakeHit(attackInfo);
    }

    public void TakeHit(AttackInfo info)
    {
        if (info.layer != LayerMask.NameToLayer("Player")) return;

        int damage = (int)(info.atk * (1 - defense / 100));

        // 해당 데미지를 Toast UI로 표현
        damageUi.ShowDamageEffect(damage).Forget(); // ToAsyncLazy()로 값을 받을 필요없음

        if (curShield > 0 && damage > 0)
        {
            int shieldDamage = (int)Mathf.Clamp(damage, 0, curShield);
            curShield -= shieldDamage;
            damage -= shieldDamage;
        }

        if (damage <= 0) return;

        curHp = damage;

        if (curHp <= 0)
        {
            curHp = 0;
            stateMachine.ChangeState(stateDict[CharStateType.Dead]);
            isDead.Value = true;
        }

        else if (hitCoolTimer <= 0f)
        {
            stateMachine.ChangeState(stateDict[CharStateType.Hit]);
            hitCoolTimer = HIT_COOLDOWN;
        }
    }
}
