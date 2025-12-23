using UnityEngine;

public class CharacterRun : CharacterState
{
    private const float MOVE_SPEED = 3f;
    public CharacterRun(TestCharController @char) : base(@char) { }

    public override void Enter()
    {
        @char.rb.linearVelocity = Vector3.right * MOVE_SPEED;
        @char.PlayAnimation(Run);
    }
    
    public override void Update()
    {
        ray = new Ray(@char.transform.position, Vector3.right);
        bool isHit = Physics.Raycast(ray, @char.range, EnemyMask);
        Debug.DrawRay(ray.origin, ray.direction * @char.range, Color.red);

        if (isHit)
        {
            @char.stateMachine.ChangeState(@char.stateDict[CharStateType.Attack]);
        }
    }

    public override void Exit()
    {
        @char.rb.linearVelocity = Vector3.zero;
    }
}
