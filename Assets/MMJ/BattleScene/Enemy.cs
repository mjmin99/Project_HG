// using UnityEngine;
// 
// public class Enemy : MonoBehaviour
// {
//     public float maxHP = 50f;
//     public float currentHP;
// 
//     public float moveSpeed = 2f;
//     public float attack = 5f;
//     public float attackSpeed = 1f;
//     public float attackRange = 1.3f;
// 
//     private float atkTimer = 0f;
//     private Transform targetPlayer;
// 
//     public HPBar3D hpBar;
// 
//     private void Start()
//     {
//         currentHP = maxHP;
//         EnemyManager.Instance.Register(this);
// 
//         hpBar = HPBar3D.Create(this);
//     }
// 
//     private void Update()
//     {
//         UpdateTarget();
// 
//         if (targetPlayer == null)
//             return;
// 
//         float dist = Vector3.Distance(transform.position, targetPlayer.position);
// 
//         if (dist > attackRange)
//         {
//             MoveTowardsPlayer();
//         }
//         else
//         {
//             TryAttack();
//         }
//     }
// 
//     void UpdateTarget()
//     {
//         var playerUnit = PlayerBattleManager.Instance.GetClosestPlayer(transform.position);
//         if (playerUnit != null)
//             targetPlayer = playerUnit.transform;
//         else
//             targetPlayer = null;
//     }
// 
//     void MoveTowardsPlayer()
//     {
//         if (targetPlayer == null) return;
// 
//         transform.position = Vector3.MoveTowards(
//             transform.position,
//             targetPlayer.position,
//             moveSpeed * Time.deltaTime);
//     }
// 
//     void TryAttack()
//     {
//         atkTimer += Time.deltaTime;
//         if (atkTimer >= attackSpeed)
//         {
//             atkTimer = 0f;
// 
//             var playerUnit = PlayerBattleManager.Instance.GetClosestPlayer(transform.position);
//             if (playerUnit != null)
//                 playerUnit.TakeDamage(attack);
//         }
//     }
// 
//     public void TakeDamage(float dmg)
//     {
//         currentHP -= dmg;
// 
//         DamageText.Create(transform.position + Vector3.up * 1.2f, dmg);
// 
//         // HP바 업데이트
//         if (hpBar != null)
//             hpBar.UpdateBar(currentHP / maxHP);
// 
//         if (currentHP <= 0)
//             Die();
//     }
// 
//     void Die()
//     {
//         EnemyManager.Instance.Unregister(this);
//         Destroy(gameObject);
//     }
// }
