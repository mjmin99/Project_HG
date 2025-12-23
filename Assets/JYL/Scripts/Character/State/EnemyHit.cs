using UnityEngine;

public class EnemyHit : EnemyState
{
    private const float HIT_TIMER = 0.2f;
    private float timer;
    public EnemyHit(TestEnemyController controller) : base(controller) { }

    public override void Enter()
    {
        timer = 0f;
        controller.PlayAnimation(Hit);
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer > HIT_TIMER)
        {
            controller.ChangeState(CharStateType.Idle);
        }
    }

    public override void Exit()
    {
        
    }
}
