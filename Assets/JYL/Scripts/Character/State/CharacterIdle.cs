using UnityEngine;

public class CharacterIdle : CharacterState
{
    
    public CharacterIdle(TestCharacterController character) : base(character)
    {
        RunFixedUpdate = true;
    }

    public override void Enter()
    {
        character.PlayAnimation(Idle);
    }

    public override void Update()
    {
        ray = new Ray(character.transform.position, Vector3.right);
        bool isHit = Physics.Raycast(ray, character.range, EnemyMask);
        Debug.DrawRay(ray.origin, ray.direction * character.range, Color.red);
        character.stateMachine.ChangeState(isHit
            ? character.stateDict[CharStateType.Attack]
            : character.stateDict[CharStateType.Run]);
    }
}
