using UnityEngine;

public class CharacterAttack : CharacterState
{
    private float timer = 0f;
    private const float RETURN_TIME = 0.2f;
    public CharacterAttack(TestCharController @char) : base(@char) { }

    public override void Enter()
    {
        @char.PlayAnimation(Attack);
        timer = 0f;
    }

    public override void Update()
    {
        ray = new Ray(@char.transform.position, Vector3.right);
        bool isHit = Physics.Raycast(ray, out var hitInfo,@char.range, EnemyMask);
        Debug.DrawRay(ray.origin, ray.direction * @char.range, Color.red);
        @char.hitInfo = hitInfo;
        if (isHit) return;
        timer += Time.deltaTime;
        if (timer >= RETURN_TIME)
        {
            @char.stateMachine.ChangeState(@char.stateDict[CharStateType.Run]);
        }
    }
}
