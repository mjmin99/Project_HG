using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestEnemyController : MonoBehaviour, IAttackable
{
    private float hp = 50f;
    private float shield = 25f;
    private float def = 7f;

    [SerializeField] private TestDamageUI damageUi;
    [SerializeField] private RectTransform uiCanvas;

    private Animator animator;
    private BoxCollider col;

    public StateMachine stateMachine;
    public readonly Dictionary<CharStateType, BaseState> stateDict = new();

    public void Init()
    {
        animator = gameObject.GetOrAddComponent<Animator>();
        col = gameObject.GetComponent<BoxCollider>();
        
        animator.Play("idle");
        col.center = new Vector3(0, 0.25f, 0);
        col.size = new Vector3(0.5f, 0.5f, 0.2f);
        
        damageUi.Init(uiCanvas);
        
        stateMachine = new StateMachine();
        stateDict.Add(CharStateType.Idle, new EnemyIdle(this));
        stateDict.Add(CharStateType.Attack, new EnemyAttack(this));
        stateDict.Add(CharStateType.Hit, new EnemyHit(this));
        stateDict.Add(CharStateType.Dead, new EnemyDead(this));
        stateMachine.Initialize(stateDict[CharStateType.Idle]);
    }

    public void PlayAnimation(int key) => animator.Play(key);
    
    private void Update()
    {
        stateMachine.Update();
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
    
    public void TakeHit(AttackInfo info)
    {
        animator.Play("hit");
        if (info.layer != LayerMask.NameToLayer("Player")) return;
        
        int damage = (int)(info.atk * (1 - def / 100));
        // 해당 데미지를 Toast UI로 표현
        
        damageUi.ShowDamageEffect(damage).Forget(); // ToAsyncLazy()로 값을 받을 필요없음
        
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
            Debug.Log("에너미 죽음");
        }
        
        else
        {
            // stateMachine.ChangeState(stateDict[CharStateType.Hit]);
        }
    }
}
