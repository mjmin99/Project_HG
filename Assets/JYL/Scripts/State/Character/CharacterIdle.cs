using UnityEngine;

public class CharacterIdle : CharacterState
{
    
    public CharacterIdle(CharController controller) : base(controller)
    {
        RunFixedUpdate = true;
    }

    public override void Enter()
    {
        controller.PlayAnimation(Idle);
    }

    public override void Update()
    {
        ray = new Ray(controller.transform.position, Vector3.right);
        bool isHit = Physics.Raycast(ray, controller.stats.attackRange, EnemyMask);
        Debug.DrawRay(ray.origin, ray.direction * controller.stats.attackRange, Color.red);
        controller.stateMachine.ChangeState(isHit
            ? controller.stateDict[CharStateType.Attack]
            : controller.stateDict[CharStateType.Run]);
    }
}
