// using UnityEngine;
// 
// public class EnemySpawner : MonoBehaviour
// {
//     public GameObject enemyPrefab;
//     public float spawnInterval = 2f;
//     private float timer = 0f;
// 
//     public void Begin()
//     {
//         enabled = true;
//     }
// 
//     private void Update()
//     {
//         timer += Time.deltaTime;
//         if (timer >= spawnInterval)
//         {
//             timer = 0;
//             SpawnEnemy();
//         }
//     }
// 
//     void SpawnEnemy()
//     {
//         Instantiate(enemyPrefab, transform.position, Quaternion.identity);
//     }
// }

