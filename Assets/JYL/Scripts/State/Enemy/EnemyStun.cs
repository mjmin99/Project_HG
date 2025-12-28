using UnityEngine;

public class EnemyStun : EnemyState
{
    public EnemyStun(EnemyController controller) : base(controller) { }
    public override void Enter()
    {
        controller.PlayAnimation(Hit);
    }

    public override void Update()
    {
        controller.stunTime -=  Time.deltaTime;
        if(controller.stunTime <= 0)
            controller.ChangeState(CharStateType.Idle);
    }
}
