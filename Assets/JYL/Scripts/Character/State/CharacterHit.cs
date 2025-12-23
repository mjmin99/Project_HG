using UnityEngine;

public class CharacterHit : CharacterState
{
    private const float HIT_TIMER = 0.2f;
    private float timer;
    
    public CharacterHit(TestCharacterController character) : base(character)
    {
        RunFixedUpdate = true;
    }

    public override void Enter()
    {
        character.PlayAnimation(Hit);
        timer = 0f;
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer > HIT_TIMER)
        {
            character.stateMachine
                .ChangeState(character.stateDict[CharStateType.Idle]);
        }
    }

    public override void Exit()
    {
        
    }
}
