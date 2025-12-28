using UnityEngine;

public class CharacterState : BaseState
{
    protected CharController controller;
    
    protected Ray ray;
    protected static readonly int EnemyMask =  LayerMask.GetMask("Enemy");
    

    protected CharacterState(CharController controller)
    {
        this.controller = controller;
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
    Rewind,
    Stun
}