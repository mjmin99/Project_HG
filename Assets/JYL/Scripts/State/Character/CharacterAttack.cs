using UnityEngine;

public class CharacterAttack : CharacterState
{
    public CharacterAttack(CharController controller) : base(controller) { }

    public override void Enter()
    {
        controller.PlayAnimation(Attack);
    }

    public override void Update()
    {
        ray = new Ray(controller.transform.position, Vector3.right);
        bool isHit = Physics.Raycast(ray, out var hitInfo,controller.stats.attackRange, EnemyMask);
        Debug.DrawRay(ray.origin, ray.direction * controller.stats.attackRange, Color.red);
        controller.hitInfo = hitInfo;
        if (isHit) return;

        controller.stateMachine.ChangeState(controller.stateDict[CharStateType.Run]);
    }
}
