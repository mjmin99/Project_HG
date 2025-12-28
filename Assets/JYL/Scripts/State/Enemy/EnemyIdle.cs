using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class EnemyIdle : EnemyState
{
    private Ray ray;
    public EnemyIdle(EnemyController controller) : base(controller) { }

    public override void Enter()
    {
        controller.PlayAnimation(Idle);
    }

    public override void Update()
    {
        ray = new Ray(controller.transform.position, Vector3.left);
        if (Physics.Raycast(ray, out var hit, 0.5f, PlayerMask))
        {
            controller.hitInfo = hit;
            controller.ChangeState(CharStateType.Attack);
        }
    }
}
