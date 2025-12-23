using UnityEngine;

public class CharacterIdle : CharacterState
{
    
    public CharacterIdle(TestCharController @char) : base(@char)
    {
        RunFixedUpdate = true;
    }

    public override void Enter()
    {
        @char.PlayAnimation(Idle);
    }

    public override void Update()
    {
        ray = new Ray(@char.transform.position, Vector3.right);
        bool isHit = Physics.Raycast(ray, @char.range, EnemyMask);
        Debug.DrawRay(ray.origin, ray.direction * @char.range, Color.red);
        @char.stateMachine.ChangeState(isHit
            ? @char.stateDict[CharStateType.Attack]
            : @char.stateDict[CharStateType.Run]);
    }
}
