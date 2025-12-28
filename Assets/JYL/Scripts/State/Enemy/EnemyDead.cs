using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class EnemyDead : EnemyState
{
    private float timer;
    private float deathTime;
    public EnemyDead(EnemyController controller) : base(controller) { }

    public override void Enter()
    {
        controller.PlayAnimation(Dead);
        controller.animator.Update(0f); // 애니메이션 적용을 다음 Update가 아닌, 현재 적용
        controller.col.enabled = false;
        deathTime = controller.animator.GetCurrentAnimatorStateInfo(0).length; // 애니메이션 클립의 재생 길이를 가져옴
    }

    public override void Update()
    {
        timer +=  Time.deltaTime;
        
        if (timer < deathTime) return;
        
        controller.ChangeState(CharStateType.Idle);
        controller.gameObject.SetActive(false);
    }

    public override void Exit()
    {
        timer = 0;
    }
}
