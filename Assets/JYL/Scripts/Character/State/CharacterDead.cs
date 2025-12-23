using UnityEngine;

public class CharacterDead : CharacterState
{
    private const float DEAD_RANGE = 3f;
    private const float MOVE_SPEED = 1f;
    private bool isMoving = false;

    private float deadTimer;
    private float timer;
    public CharacterDead(TestCharController @char) : base(@char)
    {
        RunFixedUpdate = true;
    }

    public override void Enter()
    {
        @char.rb.linearVelocity = Vector3.zero;
        
        @char.PlayAnimation(Dead);
        @char.animator.Update(0f);

        deadTimer = @char.animator.GetCurrentAnimatorStateInfo(0).length;
        
        @char.col.enabled = false;
        @char.isDead = true;
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

        ray = new Ray(@char.transform.position, Vector3.right);
        bool isHit = Physics.Raycast(ray, DEAD_RANGE, EnemyMask);
        Debug.DrawRay(ray.origin, ray.direction * DEAD_RANGE, Color.red);

        timer += Time.deltaTime;
        if (timer > 5f)
        {
            @char.gameObject.SetActive(false);
            return;
        }
        if(isHit || isMoving) return;
        
        @char.rb.linearVelocity = Vector3.right * MOVE_SPEED;
        isMoving = true;
    }

    public override void Exit()
    {
        @char.col.enabled = true;
        @char.rb.linearVelocity = Vector3.zero;
    }
}
