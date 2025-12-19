using UnityEngine;

public abstract class BaseState
{
    protected static readonly int Idle = Animator.StringToHash("Idle");
    protected static readonly int Run = Animator.StringToHash("Run");
    protected static readonly int Attack = Animator.StringToHash("Attack");
    protected static readonly int Hit = Animator.StringToHash("Hit");
    protected static readonly int Dead = Animator.StringToHash("Dead");
    protected static readonly int Skill = Animator.StringToHash("Skill");


    public bool RunFixedUpdate { get; protected set; } = false;
    public bool RunLateUpdate { get; protected set; } = false;

    public abstract void Enter();
    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void LateUpdate();
    public abstract void Exit();
}
