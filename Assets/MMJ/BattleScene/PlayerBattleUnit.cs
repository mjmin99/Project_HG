using UnityEngine;

public class PlayerBattleUnit : MonoBehaviour
{
    public int id;
    public CharacterStats stats;

    public float currentHP;
    public float attackTimer;

    public PlayerHPBar3D hpBar;
    public float maxHP;

    public void Init(int id, CharacterStats stats)
    {
        this.id = id;
        this.stats = stats;

        maxHP = stats.hp;
        currentHP = stats.hp;

        // Player 등록
        PlayerBattleManager.Instance.Register(this);

        // HPBar 생성 추가!
        hpBar = PlayerHPBar3D.Create(this);
    }

    private void Update()
    {
        AutoAttack();
    }

    void AutoAttack()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= stats.attackSpeed)
        {
            attackTimer = 0f;

            var target = EnemyManager.Instance.GetClosestEnemy(transform.position);
            Debug.Log("AutoAttack target = " + target);
            if (target != null)
                target.TakeDamage(stats.attack);
        }

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
