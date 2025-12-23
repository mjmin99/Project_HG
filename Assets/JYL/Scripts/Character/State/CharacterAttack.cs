using UnityEngine;

public class CharacterAttack : CharacterState
{
    public CharacterAttack(TestCharController controller) : base(controller) { }

    public override void Enter()
    {
        controller.PlayAnimation(Attack);
        Debug.Log($"들어올 때{controller.gameObject.name}");
    }

    public override void Update()
    {
        ray = new Ray(controller.transform.position, Vector3.right);
        bool isHit = Physics.Raycast(ray, out var hitInfo,controller.range, EnemyMask);
        Debug.DrawRay(ray.origin, ray.direction * controller.range, Color.red);
        controller.hitInfo = hitInfo;
        if (isHit) return;

        controller.stateMachine.ChangeState(controller.stateDict[CharStateType.Run]);
    }
}
