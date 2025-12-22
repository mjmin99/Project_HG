using UnityEngine;

public class EnemyHit : EnemyState
{
    public EnemyHit(TestEnemyController controller) : base(controller) { }

    public override void Enter()
    {
        controller.PlayAnimation(Hit);
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        
    }
}
