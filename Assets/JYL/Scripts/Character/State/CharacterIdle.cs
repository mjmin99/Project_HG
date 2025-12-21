using UnityEngine;

public class CharacterIdle : CharacterState
{
    private const float DELAY_TIMER = 0.5f;
    
    private float timer = 0f;
    
    public CharacterIdle(TestCharacterController character) : base(character)
    {
        RunFixedUpdate = true;
    }

    public override void Enter()
    {
        character.PlayAnimation(Idle);
        timer = 0f;
    }

    public override void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
    }

    public override void Update()
    {
        if (timer < DELAY_TIMER) return;

        timer = 0f;
        ray = new Ray(character.transform.position, Vector3.right);
        bool isHit = Physics.Raycast(ray, character.range, EnemyMask);
        Debug.DrawRay(ray.origin, ray.direction * character.range, Color.red);
        character.stateMachine.ChangeState(isHit
            ? character.stateDict[CharStateType.Attack]
            : character.stateDict[CharStateType.Run]);
    }

    public override void Exit()
    {
        
    }
    
}
