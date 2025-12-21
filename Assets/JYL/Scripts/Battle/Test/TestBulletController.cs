using UnityEngine;

public class TestBulletController : MonoBehaviour
{
    [SerializeField] private float fireSpeed;
    private Rigidbody rb;
    private BoxCollider col;
    private AttackInfo info;

    public void Init(LayerMask attacker, float firePower, bool isPoison)
    {
        rb = gameObject.GetOrAddComponent<Rigidbody>();
        col = gameObject.GetOrAddComponent<BoxCollider>();
        rb.isKinematic = true;
        rb.useGravity = false;
        info = new AttackInfo(attacker, firePower, isPoison);
    }

    public void FireToPosition(Vector3 pos)
    {
        var targetVec = pos - transform.position;
        targetVec.Normalize();
        rb.AddForce(targetVec * fireSpeed, ForceMode.Impulse);
    }
    void OnCollisionEnter(Collision collision)
    {
        var comp = collision.gameObject.GetComponent<IAttackable>();
        comp.TakeHit(info);
    }
}
