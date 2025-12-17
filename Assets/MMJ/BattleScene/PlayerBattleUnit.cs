// using UnityEngine;
// 
// public class PlayerBattleUnit : MonoBehaviour
// {
//     public int id;
//     public CharacterStats stats;
// 
//     public float currentHP;
//     public float attackTimer;
// 
//     public PlayerHPBar3D hpBar;
//     public float maxHP;
// 
//     public AttackType attackType;
//     public float attackRange = 1.5f;
// 
//     // 원거리 전용
//     public GameObject projectilePrefab;
//     public Transform firePoint;
// 
//     public void Init(int id, CharacterStats stats)
//     {
//         this.id = id;
//         this.stats = stats;
// 
//         maxHP = stats.hp;
//         currentHP = stats.hp;
// 
//         var model = CharacterManager.Instance.models[id];
// 
//         // CSV에서 읽은 attackType 그대로 사용
//         attackType = model.attackType;
//         attackRange = stats.attackRange;
// 
//         // 원거리 공격이면 투사체 설정
//         if (attackType == AttackType.Ranged)
//         {
//             SetupRanged();
//         }
// 
//         PlayerBattleManager.Instance.Register(this);
//         hpBar = PlayerHPBar3D.Create(this);
//     }
// 
//     private void Update()
//     {
//         AutoAttack();
//     }
// 
//     public void SetupRanged()
//     {
//         // firePoint 생성 (투사체 발사 위치)
//         GameObject fp = new GameObject("FirePoint");
//         fp.transform.SetParent(transform);
//         fp.transform.localPosition = new Vector3(0.5f, 0.8f, 0f);
//         firePoint = fp.transform;
// 
//         // 투사체 프리팹 로드
//         projectilePrefab = Resources.Load<GameObject>("Projectiles/BasicProjectile");
// 
//         if (projectilePrefab == null)
//             Debug.LogWarning($"[PlayerBattleUnit] BasicProjectile 프리팹을 찾을 수 없습니다!");
//     }
// 
//     void AutoAttack()
//     {
//         attackTimer += Time.deltaTime;
// 
//         // attackSpeed는 "초당 공격 횟수"이므로 간격은 1/attackSpeed
//         float attackInterval = 1f / stats.attackSpeed;
// 
//         if (attackTimer < attackInterval)
//             return;
// 
//         Enemy target = EnemyManager.Instance.GetClosestEnemy(transform.position);
//         if (target == null)
//             return;
// 
//         if (!IsInRange(target))
//             return;
// 
//         attackTimer = 0f;
//         DoAttack(target);
//     }
// 
//     bool IsInRange(Enemy target)
//     {
//         float dist = Vector3.Distance(transform.position, target.transform.position);
//         return dist <= attackRange;
//     }
// 
//     void DoAttack(Enemy target)
//     {
//         if (attackType == AttackType.Melee)
//         {
//             // 근거리 공격: 즉시 데미지
//             target.TakeDamage(stats.attack);
//         }
//         else
//         {
//             // 원거리 공격: 투사체 발사
//             FireProjectile(target);
//         }
//     }
// 
//     void FireProjectile(Enemy target)
//     {
//         if (projectilePrefab == null || firePoint == null)
//         {
//             Debug.LogWarning("[PlayerBattleUnit] 투사체 발사 실패: 프리팹 또는 FirePoint 없음");
//             return;
//         }
// 
//         GameObject projObj = Instantiate(
//             projectilePrefab,
//             firePoint.position,
//             Quaternion.identity
//         );
// 
//         Projectile proj = projObj.GetComponent<Projectile>();
//         proj.Init(target.transform, stats.attack);
//     }
// 
//     public void TakeDamage(float dmg)
//     {
//         currentHP -= dmg;
// 
//         DamageText.Create(transform.position + Vector3.up * 1.2f, dmg);
// 
//         if (hpBar != null)
//             hpBar.UpdateBar(currentHP / maxHP);
// 
//         if (currentHP <= 0)
//             Die();
//     }
// 
//     void Die()
//     {
//         PlayerBattleManager.Instance.Unregister(this);
// 
//         if (hpBar != null)
//             Destroy(hpBar.gameObject);
// 
//         Destroy(gameObject);
// 
//         BattleResultManager.Instance.CheckDefeat();
//     }
// }