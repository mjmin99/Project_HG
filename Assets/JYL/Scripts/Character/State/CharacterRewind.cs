using UnityEngine;

public class CharacterRewind : CharacterState
{
    private TestTimeInfo startInfo;
    private TestTimeInfo targetInfo;
    private float lerpTime;

    private const float REWIND_SPEED = 1.5f;

    public CharacterRewind(TestCharacterController character) : base(character)
    {
        this.character = character;
        RunFixedUpdate = true;
    }

    public override void Enter()
    {
        character.rb.isKinematic = true;
        character.col.enabled = false;

        character.PlayAnimation(Rewind);
        character.animator.speed = 1f * REWIND_SPEED;

        startInfo = new TestTimeInfo(character.transform, 0, 0);
        
        targetInfo = character.HasHistory() ? character.PopHistory() : startInfo;

        lerpTime = 0f;
    }

    public override void Update()
    {
        
        if (!character.HasHistory() && lerpTime >= 1f)
        {
            character.FinishRewind();
            return;
        }
        
        // 분자 합이 분모만큼 되면 1 이상됨
        // rewind speed로 배속 조절 됨
        lerpTime += (Time.deltaTime * REWIND_SPEED) / Time.fixedDeltaTime; 

        if (lerpTime >= 1f)
        {
            if (character.HasHistory())
            {
                startInfo = targetInfo;
                targetInfo = character.PopHistory();

                lerpTime -= 1f;
            }
            else
            {
                lerpTime = 1f;
            }
        }
        
        character.transform.position = Vector3.Lerp(startInfo.position, targetInfo.position, lerpTime);
        character.transform.rotation = Quaternion.Lerp(startInfo.rotation, targetInfo.rotation, lerpTime);
        
    }

    public override void FixedUpdate()
    {
        character.SetHp(targetInfo.hp);
        character.SetShield(targetInfo.shield);
        
    }

    public override void Exit()
    {
        character.rb.isKinematic = false;
        character.col.enabled = true;
        character.animator.speed = 1f;
    }
}
