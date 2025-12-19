using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class TestCharacterController : MonoBehaviour
{
    [SerializeField] private int characterId;
    [SerializeField] private float range;
    [SerializeField] private float hp;
    [SerializeField] private AttackType atkType;
    [SerializeField] private float atk;
    [SerializeField] private float def;

    private StateMachine stateMachine;
    public Rigidbody rb;
    public BoxCollider col;
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
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer($"Enemy"))
        {
            var info = collision.gameObject.GetComponent<EnemyAttackInfo>();
            TakeHit(info);
        }
    }

    private void TakeHit(EnemyAttackInfo attackInfo)
    {
        hp -= attackInfo.atk * ((100 - def) / 100);
    }
    public void PlayAnimation(int animKey)
    {
        animator.Play(animKey);
    }
}
