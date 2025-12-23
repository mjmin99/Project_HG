// using UnityEngine;
// 
// 
// // 테스트용 EnemyController
// // - 전투 로직 없음
// // - Enemy(Model) 주입 구조 검증용
// // - 전투 구현 시 교체 예정
// public class TestEnemyController : MonoBehaviour
// {
//     private Enemy model;
//     // EnemyManager에서 호출
//     // Enemy(Model) 데이터를 주입받음
//     public void Init(Enemy enemyModel)
//     {
//         model = enemyModel;
// 
//         // 디버그 확인용 로그
//         Debug.Log(
//             $"[EnemyController:Test] Init → " +
//             $"ID={model.id}, Name={model.name}, " +
//             $"HP={model.maxHP}, ATK={model.attack}, " +
//             $"MATK={model.magicAttack}, " +
//             $"Range={model.attackRange}, " +
//             $"Type={model.attackType}"
//         );
// 
//         // 보기 편하게 오브젝트 이름 변경
//         gameObject.name = $"Enemy_{model.name}";
//     }
// 
//     private void Start()
//     {
//         // 테스트용: 씬에서 눈에 보이게 살짝 랜덤 이동
//         Vector3 offset = new Vector3(
//             Random.Range(-1.5f, 1.5f),
//             0f,
//             Random.Range(-1.5f, 1.5f)
//         );
// 
//         transform.position += offset;
//     }
// 
//     private void OnDestroy()
//     {
//         if (model != null)
//         {
//             Debug.Log($"[EnemyController:Test] Destroyed → {model.name}");
//         }
//     }
// }
