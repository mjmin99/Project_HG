using UnityEngine;

public class CharacterRewind : CharacterState
{
    private TestTimeInfo startInfo;
    private TestTimeInfo targetInfo;
    private float lerpTime;

    private const float REWIND_SPEED = 1.5f;

    public CharacterRewind(TestCharController @char) : base(@char)
    {
        this.@char = @char;
        RunFixedUpdate = true;
    }

    public override void Enter()
    {
        @char.isRewinding = true;
        @char.rb.isKinematic = true;
        @char.col.enabled = false;

        if(!@char.isDead) @char.PlayAnimation(Rewind);
        
        @char.animator.speed = 1f * REWIND_SPEED;

        startInfo = new TestTimeInfo(@char.transform.position, 0, 0);
        
        targetInfo = @char.HasHistory() ? @char.PopHistory() : startInfo;

        lerpTime = 0f;
    }

    public override void Update()
    {
        // 기록이 없으면 종료
        if (!@char.HasHistory() && lerpTime >= 1f)
        {
            @char.FinishRewind();
            return;
        }
        
        // 분자 합이 분모만큼 되면 1 이상됨
        // rewind speed로 배속 조절 됨
        lerpTime += (Time.deltaTime * REWIND_SPEED) / Time.fixedDeltaTime; 

        if (lerpTime >= 1f)
        {
            if (@char.HasHistory())
            {
                startInfo = targetInfo;
                targetInfo = @char.PopHistory();
                if (@char.isDead && targetInfo.hp > 0)
                {
                    @char.isDead = false;
                    @char.PlayAnimation(Rewind);
                }

                lerpTime -= 1f;
            }
            else
            {
                lerpTime = 1f;
            }
        }
        
        @char.transform.position = Vector3.Lerp(startInfo.position, targetInfo.position, lerpTime);
    }

    public override void FixedUpdate()
    {
        @char.SetHp(targetInfo.hp);
        @char.SetShield(targetInfo.shield);
        
    }

    public override void Exit()
    {
        @char.isRewinding = false;
        @char.rb.isKinematic = false;
        @char.col.enabled = true;
        @char.animator.speed = 1f;
    }
}
