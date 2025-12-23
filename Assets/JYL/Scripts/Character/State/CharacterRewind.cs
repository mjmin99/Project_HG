using UnityEngine;

public class CharacterRewind : CharacterState
{
    private TestTimeInfo startInfo;
    private TestTimeInfo targetInfo;
    private float lerpTime;

    private const float REWIND_SPEED = 1.5f;

    public CharacterRewind(TestCharController controller) : base(controller)
    {
        this.controller = controller;
        RunFixedUpdate = true;
    }

    public override void Enter()
    {
        controller.isRewinding = true;
        controller.rb.isKinematic = true;
        controller.col.enabled = false;

        if(!controller.isDead) controller.PlayAnimation(Rewind);
        
        controller.animator.speed = 1f * REWIND_SPEED;

        startInfo = new TestTimeInfo(controller.transform.position, 0, 0);
        
        targetInfo = controller.HasHistory() ? controller.PopHistory() : startInfo;

        lerpTime = 0f;
    }

    public override void Update()
    {
        // 기록이 없으면 종료
        if (!controller.HasHistory() && lerpTime >= 1f)
        {
            controller.FinishRewind();
            return;
        }
        
        // 분자 합이 분모만큼 되면 1 이상됨
        // rewind speed로 배속 조절 됨
        lerpTime += (Time.deltaTime * REWIND_SPEED) / Time.fixedDeltaTime; 

        if (lerpTime >= 1f)
        {
            if (controller.HasHistory())
            {
                startInfo = targetInfo;
                targetInfo = controller.PopHistory();
                if (controller.isDead && targetInfo.hp > 0)
                {
                    controller.isDead = false;
                    controller.PlayAnimation(Rewind);
                }

                lerpTime -= 1f;
            }
            else
            {
                lerpTime = 1f;
            }
        }
        
        controller.transform.position = Vector3.Lerp(startInfo.position, targetInfo.position, lerpTime);
    }

    public override void FixedUpdate()
    {
        controller.SetHp(targetInfo.hp);
        controller.SetShield(targetInfo.shield);
        
    }

    public override void Exit()
    {
        controller.isRewinding = false;
        controller.rb.isKinematic = false;
        controller.col.enabled = true;
        controller.animator.speed = 1f;
    }
}
