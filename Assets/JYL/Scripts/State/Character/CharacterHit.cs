using UnityEngine;

public class CharacterHit : CharacterState
{
    private const float HIT_TIMER = 0.2f;
    private float timer;
    
    public CharacterHit(CharController controller) : base(controller)
    {
        RunFixedUpdate = true;
    }

    public override void Enter()
    {
        controller.PlayAnimation(Hit);
        timer = 0f;
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer > HIT_TIMER)
        {
            controller.stateMachine
                .ChangeState(controller.stateDict[CharStateType.Idle]);
        }
    }

    public override void Exit()
    {
        
    }
}
