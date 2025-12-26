using UnityEngine;

public class EnemyState : BaseState
{
    protected static readonly int PlayerMask = LayerMask.GetMask("Player");
    
    protected EnemyController controller;
    protected EnemyState(EnemyController controller)
    {
        this.controller = controller;
    }
    public override void Enter() { }

    public override void Update() { }

    public override void FixedUpdate() { }

    public override void LateUpdate() { }

    public override void Exit() { }
}
