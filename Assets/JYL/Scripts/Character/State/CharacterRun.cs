using UnityEngine;

public class CharacterRun : CharacterState
{
    private const float MOVE_SPEED = 3f;
    public CharacterRun(CharController controller) : base(controller) { }

    public override void Enter()
    {
        controller.rb.linearVelocity = Vector3.right * MOVE_SPEED;
        controller.PlayAnimation(Run);
    }
    
    public override void Update()
    {
        ray = new Ray(controller.transform.position, Vector3.right);
        bool isHit = Physics.Raycast(ray, controller.stats.attackRange, EnemyMask);
        Debug.DrawRay(ray.origin, ray.direction * controller.stats.attackRange, Color.red);

        if (isHit)
        {
            controller.stateMachine.ChangeState(controller.stateDict[CharStateType.Attack]);
        }
    }

    public override void Exit()
    {
        controller.rb.linearVelocity = Vector3.zero;
    }
}
