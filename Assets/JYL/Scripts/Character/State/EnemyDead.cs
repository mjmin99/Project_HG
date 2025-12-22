using UnityEngine;

public class EnemyDead : EnemyState
{
    public EnemyDead(TestEnemyController controller) : base(controller) { }

    public override void Enter()
    {
        controller.PlayAnimation(Dead);
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        
    }
}
