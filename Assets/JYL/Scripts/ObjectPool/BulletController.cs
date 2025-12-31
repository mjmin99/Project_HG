using UnityEngine;

public class BulletController : PooledObject
{
    private float fireSpeed;
    private Rigidbody rb;
    private BoxCollider col;
    private AttackInfo info;
    private Animator animator;
    public bool isInit;

    public void Init(LayerMask attacker, float firePower, float fireSpeed = 2f)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = null;
        gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
        
        rb = gameObject.GetOrAddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        col = gameObject.GetOrAddComponent<BoxCollider>();
        col.isTrigger = true;
        col.enabled = false;
        
        rb.isKinematic = false;
        rb.useGravity = false;
        
        info = new AttackInfo(attacker, firePower, false);
        animator = gameObject.GetOrAddComponent<Animator>();
        isInit = true;
        this.fireSpeed = fireSpeed;
    }

    public void FireToPosition(Vector3 pos, bool isCritical)
    {
        info.isCritical =  isCritical;
        rb.linearVelocity = Vector3.zero;
        gameObject.SetActive(true);
        animator.Play("Fire");
        animator.Update(0f);
        col.enabled = true;
        var targetVec = pos + Vector3.up * 0.25f - transform.position;
        targetVec.Normalize();
        rb.AddForce(targetVec * fireSpeed, ForceMode.Impulse);
        ReturnToPool();
    }
    
    void OnTriggerEnter(Collider other)
    {
        var comp = other.gameObject.GetComponent<IAttackable>();
        comp.TakeHit(info);
        ReturnToPoolNow();
    }
}
