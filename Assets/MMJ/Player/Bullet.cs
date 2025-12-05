using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // z+ 방향(또는 x+)으로 직진
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
