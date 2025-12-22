using UnityEngine;

public class EnemyAttack : EnemyState
{
    public EnemyAttack(TestEnemyController controller) : base(controller) { }

    public override void Enter()
    {
        controller.PlayAnimation(Attack);
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        
    }
}
