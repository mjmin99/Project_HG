using UnityEngine;

public class CharacterAttack : CharacterState
{
    public CharacterAttack(TestCharacterController character) : base(character)
    {
        RunFixedUpdate = true;
    }

    public override void Enter()
    {
        character.PlayAnimation(Attack);
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Update()
    {
        ray = new Ray(character.transform.position, Vector3.right);
        bool isHit = Physics.Raycast(ray, character.range, EnemyMask);
        Debug.DrawRay(ray.origin, ray.direction * character.range, Color.red);
        
        if (!isHit)
        {
            character.stateMachine.ChangeState(character.stateDict[CharStateType.Run]);
        }
    }

    public override void Exit()
    {
        
    }
}
