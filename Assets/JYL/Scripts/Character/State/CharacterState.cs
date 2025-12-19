using UnityEngine;

public class CharacterState : BaseState
{
    protected static readonly int Idle = Animator.StringToHash("Idle");
    protected static readonly int Run = Animator.StringToHash("Run");
    protected static readonly int Jump = Animator.StringToHash("Jump");
    protected static readonly int Attack = Animator.StringToHash("Attack");
    protected static readonly int Hit = Animator.StringToHash("Hit");
    protected static readonly int Dead = Animator.StringToHash("Dead");

    protected TestCharacterController character;
    
    public override void Enter() { }

    public override void Update() { }

    public override void FixedUpdate() { }

    public override void LateUpdate() { }

    public override void Exit() { }
}
