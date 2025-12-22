using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestEnemyController : MonoBehaviour, IAttackable
{
    private float hp = 1000f;
    private float shield = 100f;
    private float def = 7f;
    private const float HIT_TIMER = 0.5f;
    private float timer;

    [SerializeField] private TestDamageUI damageUi;
    [SerializeField] private RectTransform uiCanvas;

    private Animator animator;
    private BoxCollider col;
    
    public void Awake()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<BoxCollider>();
        
        animator.Play("idle");
        col.center = new Vector3(0, 0.25f, 0);
        col.size = new Vector3(0.5f, 0.5f, 0.2f);
        
        damageUi.Init(uiCanvas);
    }

    public void FixedUpdate()
    {
        if (timer > HIT_TIMER)
        {
            timer = 0f;
            animator.Play("idle");
        }
        else
        {
            timer += Time.fixedDeltaTime;
        }
    }
    public void TakeHit(AttackInfo info)
    {
        animator.Play("hit");
        if (info.layer != LayerMask.NameToLayer("Player")) return;
        
        int damage = (int)(info.atk * (1 - def / 100));
        // 해당 데미지를 Toast UI로 표현
        
        damageUi.ShowDamageEffect(damage).Forget(); // ToAsyncLazy()로 값을 받을 필요없음
        
        if (shield > 0 && damage > 0)
        {
            int shieldDamage = (int)Mathf.Clamp(damage, 0, shield);
            shield -= shieldDamage;
            damage -= shieldDamage;
        }

        if (damage <= 0) return;
        
        hp -= damage;
        
        if (hp <= 0)
        {
            hp = 0;
            Debug.Log("에너미 죽음");
        }
        
        else
        {
            // stateMachine.ChangeState(stateDict[CharStateType.Hit]);
        }
    }
}
