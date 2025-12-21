using UnityEngine;

public class EnemyState : BaseState
{
    protected TestEnemyController controller;
    protected EnemyState(TestEnemyController controller)
    {
        this.controller = controller;
    }
    public override void Enter() { }

    public override void Update() { }

    public override void FixedUpdate() { }

    public override void LateUpdate() { }

    public override void Exit() { }
}
