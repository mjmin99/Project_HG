using UnityEngine;

public class CharacterRun : CharacterState
{
    private const float MOVE_SPEED = 3f;
    public CharacterRun(TestCharacterController character) : base(character) { }

    public override void Enter()
    {
        character.rb.linearVelocity = Vector3.right * MOVE_SPEED;
        character.PlayAnimation(Run);
    }
    
    public override void Update()
    {
        ray = new Ray(character.transform.position, Vector3.right);
        bool isHit = Physics.Raycast(ray, character.range, EnemyMask);
        Debug.DrawRay(ray.origin, ray.direction * character.range, Color.red);

        if (isHit)
        {
            character.stateMachine.ChangeState(character.stateDict[CharStateType.Attack]);
        }
    }

    public override void Exit()
    {
        character.rb.linearVelocity = Vector3.zero;
    }
}
