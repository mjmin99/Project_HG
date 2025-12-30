using UnityEngine;

public class LazerController : PooledObject
{
    private float fireSpeed = 1f;
    private Rigidbody rb;
    private BoxCollider col;
    private AttackInfo info;
    private Animator animator;
    public bool isInit;
    
    public void Init(LayerMask attacker, float firePower, float playSpeed = 1f)
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        gameObject.GetComponent<SpriteRenderer>().sprite = null;
        
        rb = gameObject.GetOrAddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        col = gameObject.GetOrAddComponent<BoxCollider>();
        col.isTrigger = true;
        col.enabled = false;
        
        rb.isKinematic = false;
        rb.useGravity = false;
        
        info = new AttackInfo(attacker, firePower);
        animator = gameObject.GetOrAddComponent<Animator>();
        isInit = true;
        fireSpeed = playSpeed;
    }
    
    public void InitiateLazer()
    {
        gameObject.SetActive(true);
        animator.speed = fireSpeed;
        animator.Play("Fire");
        animator.Update(0f);
        var counter = animator.GetCurrentAnimatorStateInfo(0).length;
        ReturnToPool(counter);
    }
    public void OnLazer() // 애니메이션 이벤트를 통해 수행 됨
    {
        col.enabled = true;
    }
    
    void OnTriggerEnter(Collider other)
    {
        var comp = other.gameObject.GetComponent<IAttackable>();
        comp.TakeHit(info);
    }
}
