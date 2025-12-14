using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private Enemy targetEnemy;
    private float damage;

    public float speed = 8f;
    public float lifetime = 5f; // 최대 생존 시간 (무한 추적 방지)

    private float timer = 0f;

    public void Init(Transform target, float damage)
    {
        this.target = target;
        this.damage = damage;

        // Enemy 컴포넌트 미리 캐싱
        if (target != null)
        {
            targetEnemy = target.GetComponent<Enemy>();

            if (targetEnemy == null)
            {
                Debug.LogWarning("[Projectile] 타겟에 Enemy 컴포넌트 없음!");
            }
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 타임아웃 체크
        if (timer >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        // 타겟이 사라졌으면 투사체 제거
        if (target == null || targetEnemy == null)
        {
            Destroy(gameObject);
            return;
        }

        // 타겟을 향해 이동
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // 충돌 체크
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance < 0.2f)
        {
            targetEnemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}