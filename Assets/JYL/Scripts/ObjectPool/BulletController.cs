using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float fireSpeed;
    private Rigidbody rb;
    private BoxCollider col;
    private AttackInfo info;

    public void Init(LayerMask attacker, float firePower)
    {
        rb = gameObject.GetOrAddComponent<Rigidbody>();
        col = gameObject.GetOrAddComponent<BoxCollider>();
        col.isTrigger = true;
        gameObject.transform.position += Vector3.up * 0.25f;
        rb.isKinematic = false;
        rb.useGravity = false;
        info = new AttackInfo(attacker, firePower);
    }

    public void FireToPosition(Vector3 pos)
    {
        var targetVec = pos + Vector3.up * 0.25f - transform.position;
        targetVec.Normalize();
        rb.AddForce(targetVec * fireSpeed, ForceMode.Impulse);
    }

    public void LazerFire()
    {
        // TODO : 레이저 발사 구현 및 테스트
    }
    
    void OnTriggerEnter(Collider other)
    {
        var comp = other.gameObject.GetComponent<IAttackable>();
        comp.TakeHit(info);
    }
}
