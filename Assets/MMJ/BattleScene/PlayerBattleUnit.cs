using System.Security.Cryptography;
using UnityEngine;

public class PlayerBattleUnit : MonoBehaviour
{
    public int id;
    public CharacterStats stats;

    public float currentHP;
    public float attackTimer;

    public PlayerHPBar3D hpBar;
    public float maxHP;

    public AttackType attackType;
    public float attackRange = 1.5f;

    // 원거리 전용
    public GameObject projectilePrefab;
    public Transform firePoint;


    public void Init(int id, CharacterStats stats)
    {
        this.id = id;
        this.stats = stats;

        maxHP = stats.hp;
        currentHP = stats.hp;

        var model = CharacterManager.Instance.models[id];

        attackType = model.attackType;
        attackRange = model.attackRange;

        // range 기준 자동 판별 -----------------> 100 사거리를 넘기면 자동으로 원거리로 판정됨
        attackRange = stats.attackRange;
        if (attackRange > 100f)
        {
            attackType = AttackType.Ranged;
            SetupRanged();
        }
        else
        {
            attackType = AttackType.Melee;
        }

        PlayerBattleManager.Instance.Register(this);
        hpBar = PlayerHPBar3D.Create(this);
    }

    private void Update()
    {
        AutoAttack();
    }

    public void SetupRanged()
    {
        // firePoint 생성
        GameObject fp = new GameObject("FirePoint");
        fp.transform.SetParent(transform);
        fp.transform.localPosition = new Vector3(0.5f, 0.8f);
        firePoint = fp.transform;

        // 투사체 프리팹은 Resources 등에서 로드
        projectilePrefab = Resources.Load<GameObject>("Projectiles/BasicProjectile");
    }

    void AutoAttack()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer < stats.attackSpeed)
            return;

        Enemy target = EnemyManager.Instance.GetClosestEnemy(transform.position);
        if (target == null)
            return;

        if (!IsInRange(target))
            return;

        attackTimer = 0f;
        DoAttack(target);
    }

    Enemy FindTarget()
    {
        return EnemyManager.Instance.GetClosestEnemy(transform.position);
    }

    bool IsInRange(Enemy target)
    {
        float dist = Vector3.Distance(transform.position, target.transform.position);
        return dist <= attackRange;
    }

    void DoAttack(Enemy target)
    {
        if (attackType == AttackType.Melee)
            target.TakeDamage(stats.attack);
        else
            FireProjectile(target);
    }

    void FireProjectile(Enemy target)
    {
        GameObject projObj = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        Projectile proj = projObj.GetComponent<Projectile>();
        proj.Init(target.transform, stats.attack);
    }

    void DoMeleeAttack(Enemy target)
    {
        target.TakeDamage(stats.attack);
    }

    public void TakeDamage(float dmg)
    {
        currentHP -= dmg;

        DamageText.Create(transform.position + Vector3.up * 1.2f, dmg);

        if (hpBar != null)
            hpBar.UpdateBar(currentHP / maxHP);

        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        // Player 제거
        PlayerBattleManager.Instance.Unregister(this);

        if (hpBar != null)
            Destroy(hpBar.gameObject);

        Destroy(gameObject);

        BattleResultManager.Instance.CheckDefeat();
    }
}
