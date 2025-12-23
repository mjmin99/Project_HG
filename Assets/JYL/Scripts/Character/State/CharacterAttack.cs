using UnityEngine;

public class CharacterAttack : CharacterState
{
    private float timer = 0f;
    private const float RETURN_TIME = 0.2f;
    public CharacterAttack(TestCharacterController character) : base(character) { }

    public override void Enter()
    {
        character.PlayAnimation(Attack);
        timer = 0f;
    }

    public override void Update()
    {
        ray = new Ray(character.transform.position, Vector3.right);
        bool isHit = Physics.Raycast(ray, out var hitInfo,character.range, EnemyMask);
        Debug.DrawRay(ray.origin, ray.direction * character.range, Color.red);
        character.hitInfo = hitInfo;
        if (isHit) return;
        timer += Time.deltaTime;
        if (timer >= RETURN_TIME)
        {
            character.stateMachine.ChangeState(character.stateDict[CharStateType.Run]);
        }
    }
}
