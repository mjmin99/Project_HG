using UnityEngine;

public class CharacterState : BaseState
{
    protected TestCharController @char;
    
    protected Ray ray;
    protected static readonly int EnemyMask =  LayerMask.GetMask("Enemy");
    

    protected CharacterState(TestCharController @char)
    {
        this.@char = @char;
    }
    public override void Enter() { }

    public override void Update() { }

    public override void FixedUpdate() { }

    public override void LateUpdate() { }

    public override void Exit() { }
}

public enum CharStateType{
    Idle,
    Run,
    Attack,
    Skill,
    Hit,
    Dead,
    Rewind
}