using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float fireSpeed = 3f;
    private Rigidbody rb;
    private BoxCollider col;
    private AttackInfo info;
    private Animator animator;

    public void Init(LayerMask attacker, float firePower)
    {
        rb = gameObject.GetOrAddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        col = gameObject.GetOrAddComponent<BoxCollider>();
        col.isTrigger = true;
        col.enabled = false;
        
        gameObject.transform.position += Vector3.up * 0.25f;
        
        rb.isKinematic = false;
        rb.useGravity = false;
        
        info = new AttackInfo(attacker, firePower);
        animator = gameObject.GetOrAddComponent<Animator>();
        animator.Play("Fire");
    }

    public void FireToPosition(Vector3 pos)
    {
        col.enabled = true;
        var targetVec = pos + Vector3.up * 0.25f - transform.position;
        targetVec.Normalize();
        rb.AddForce(targetVec * fireSpeed, ForceMode.Impulse);
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
