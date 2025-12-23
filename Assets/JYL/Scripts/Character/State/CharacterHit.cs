using UnityEngine;

public class CharacterHit : CharacterState
{
    private const float HIT_TIMER = 0.2f;
    private float timer;
    
    public CharacterHit(TestCharController @char) : base(@char)
    {
        RunFixedUpdate = true;
    }

    public override void Enter()
    {
        @char.PlayAnimation(Hit);
        timer = 0f;
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer > HIT_TIMER)
        {
            @char.stateMachine
                .ChangeState(@char.stateDict[CharStateType.Idle]);
        }
    }

    public override void Exit()
    {
        
    }
}
