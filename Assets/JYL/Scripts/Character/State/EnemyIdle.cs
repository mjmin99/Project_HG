using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class EnemyIdle : EnemyState
{
    public EnemyIdle(TestEnemyController controller) : base(controller) { }

    public override void Enter()
    {
        controller.PlayAnimation(Idle);
    }

    public override void Update()
    {
        
    }
    
    public override void Exit()
    {
        
    }
}
