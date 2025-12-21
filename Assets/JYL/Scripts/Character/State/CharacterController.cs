using System;
using UnityEditor;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    
    public Rigidbody2D rb;
    public BoxCollider2D boxCol;
    public Animator animator;
    // public Skill skill;
    public CharacterStats stats;

    // FSM 관리
    // public StateMachine stateMachine;

    private void Update()
    {
        // stateMachine.Update();
    }

    private void FixedUpdate()
    {
        // stateMachine.FixedUpdate();
    }

    private void LateUpdate()
    {
        // stateMachine.LateUpdate();
    }
    
    public void Init(CharacterInstance instance, CharacterModel model)
    {
        stats = instance.GetStats(model);
        animator = gameObject.GetOrAddComponent<Animator>();
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"Animation/Controller/{model.id}");
        // stateMachine.Init();
    }

    public void TakeHit()
    {
        animator.Play("Hit");
    }

    public void Attack(AttackType type)
    {
        // 애니메이션 재생
        animator.Play("Attack");
        switch (type)
        {
            case AttackType.Melee:
                // 밀리 범위 공격 수행
                break;
            case AttackType.Ranged:
                // 투사체 발사 : 별도 스크립트
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void Move()
    {
        animator.Play("Move");
        // 오른쪽으로 이동하는 로직
    }
    public void UseSkill()
    {
        animator.Play("Skill");
    }
}
