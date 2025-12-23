using UnityEngine;

public class CharacterDead : CharacterState
{
    private const float DEAD_RANGE = 3f;
    private const float MOVE_SPEED = 1f;
    private bool isMoving = false;

    private float deadTimer;
    private float timer;
    public CharacterDead(TestCharController controller) : base(controller)
    {
        RunFixedUpdate = true;
    }

    public override void Enter()
    {
        controller.rb.linearVelocity = Vector3.zero;
        
        controller.PlayAnimation(Dead);
        controller.animator.Update(0f);

        deadTimer = controller.animator.GetCurrentAnimatorStateInfo(0).length;
        
        controller.col.enabled = false;
        controller.isDead = true;
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

        ray = new Ray(controller.transform.position, Vector3.right);
        bool isHit = Physics.Raycast(ray, DEAD_RANGE, EnemyMask);
        Debug.DrawRay(ray.origin, ray.direction * DEAD_RANGE, Color.red);

        timer += Time.deltaTime;
        if (timer > 5f)
        {
            controller.gameObject.SetActive(false);
            return;
        }
        if(isHit || isMoving) return;
        
        controller.rb.linearVelocity = Vector3.right * MOVE_SPEED;
        isMoving = true;
    }

    public override void Exit()
    {
        controller.col.enabled = true;
        controller.rb.linearVelocity = Vector3.zero;
    }
}
