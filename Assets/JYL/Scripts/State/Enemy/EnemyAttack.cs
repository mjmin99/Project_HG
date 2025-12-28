using UnityEngine;

public class EnemyAttack : EnemyState
{
    private Ray ray;
    public EnemyAttack(EnemyController controller) : base(controller) { }

    public override void Enter()
    {
        controller.PlayAnimation(Attack);
    }

    public override void Update()
    {
        ray = new Ray(controller.transform.position, Vector3.left);
        bool isHit = Physics.Raycast(ray, out var hitInfo, controller.enemyInfo.attackRange, PlayerMask);
        Debug.DrawRay(ray.origin + Vector3.up * 0.25f, ray.direction * controller.enemyInfo.attackRange, Color.red);
        controller.hitInfo = hitInfo;
        if (isHit) return;
        controller.stateMachine.ChangeState(controller.stateDict[CharStateType.Idle]);
    }

    public override void Exit()
    {
        
    }
}
