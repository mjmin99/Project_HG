using UnityEngine;

public class CharacterDead : CharacterState
{
    private const float DEAD_RANGE = 3f;
    private const float MOVE_SPEED = 1f;
    private bool isMoving = false;

    private float deadTimer;
    private float timer;
    public CharacterDead(TestCharacterController character) : base(character)
    {
        RunFixedUpdate = true;
    }

    public override void Enter()
    {
        character.rb.linearVelocity = Vector3.zero;
        
        character.PlayAnimation(Dead);
        character.animator.Update(0f);

        deadTimer = character.animator.GetCurrentAnimatorStateInfo(0).length;
        
        character.col.enabled = false;
        character.isDead = true;
        isMoving = false;

        timer = 0f;
    }

    public override void Update()
    {
        if (timer < deadTimer)
        {
            timer += Time.deltaTime;
            return;
        }

        ray = new Ray(character.transform.position, Vector3.right);
        bool isHit = Physics.Raycast(ray, DEAD_RANGE, EnemyMask);
        Debug.DrawRay(ray.origin, ray.direction * DEAD_RANGE, Color.red);

        timer += Time.deltaTime;
        if (timer > 5f)
        {
            character.gameObject.SetActive(false);
            return;
        }
        if(isHit || isMoving) return;
        
        character.rb.linearVelocity = Vector3.right * MOVE_SPEED;
        isMoving = true;
    }

    public override void Exit()
    {
        character.col.enabled = true;
        character.rb.linearVelocity = Vector3.zero;
    }
}
