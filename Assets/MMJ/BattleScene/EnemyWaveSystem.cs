// using UnityEngine;
// using System.Collections;
// 
// public class EnemyWaveSystem : MonoBehaviour
// {
//     public static EnemyWaveSystem Instance;
// 
//     [Header("Wave Settings")]
//     public WaveInfo[] waves;
//     public Transform spawnPoint;
// 
//     public bool IsAllWavesCleared { get; private set; } = false;
// 
//     private int currentWave = 0;
//     private Coroutine waveRoutine;
// 
//     private void Awake()
//     {
//         Instance = this;
// 
//         if (spawnPoint == null)
//         {
//             Debug.LogError("[EnemyWaveSystem] spawnPoint가 설정되지 않았습니다!");
//         }
//     }
// 
//     public void SetWaves(WaveInfo[] newWaves)
//     {
//         waves = newWaves;
//         Debug.Log($"[EnemyWaveSystem] 웨이브 설정: {newWaves?.Length ?? 0}개");
//     }
// 
//     public void Begin()
//     {
//         if (waves == null || waves.Length == 0)
//         {
//             Debug.LogError("[EnemyWaveSystem] waves가 설정되지 않았습니다!");
//             return;
//         }
// 
//         if (spawnPoint == null)
//         {
//             Debug.LogError("[EnemyWaveSystem] spawnPoint가 null입니다!");
//             return;
//         }
// 
//         if (waveRoutine != null)
//         {
//             StopCoroutine(waveRoutine);
//             waveRoutine = null;
//         }
// 
//         IsAllWavesCleared = false;
//         currentWave = 0;
// 
//         waveRoutine = StartCoroutine(RunWaves());
//     }
// 
//     private IEnumerator RunWaves()
//     {
//         yield return new WaitForSeconds(1f);
// 
//         while (currentWave < waves.Length)
//         {
//             Debug.Log($"[EnemyWaveSystem] Wave {currentWave + 1}/{waves.Length} 시작");
// 
//             yield return StartCoroutine(PlayWave(waves[currentWave]));
// 
//             currentWave++;
//         }
// 
//         IsAllWavesCleared = true;
//         Debug.Log("[EnemyWaveSystem] 모든 웨이브 클리어!");
// 
//         BattleResultManager.Instance.CheckVictory();
//     }
// 
//     private IEnumerator PlayWave(WaveInfo wave)
//     {
//         if (wave.enemies == null || wave.enemies.Length == 0)
//         {
//             Debug.LogWarning("[EnemyWaveSystem] 빈 웨이브 감지");
//             yield break;
//         }
// 
//         foreach (var enemyInfo in wave.enemies)
//         {
//             if (enemyInfo.enemyPrefab == null)
//             {
//                 Debug.LogWarning("[EnemyWaveSystem] enemyPrefab이 null입니다. 스킵합니다.");
//                 continue;
//             }
// 
//             for (int i = 0; i < enemyInfo.count; i++)
//             {
//                 SpawnEnemy(enemyInfo.enemyPrefab);
//                 yield return new WaitForSeconds(enemyInfo.spawnInterval);
//             }
//         }
// 
//         // 현재 웨이브 적이 모두 죽을 때까지 대기
//         while (EnemyManager.Instance.GetAllEnemies().Count > 0)
//             yield return null;
// 
//         Debug.Log($"[EnemyWaveSystem] Wave {currentWave + 1} 완료");
// 
//         yield return new WaitForSeconds(wave.delayAfterWave);
//     }
// 
//     private void SpawnEnemy(GameObject prefab)
//     {
//         if (prefab == null)
//         {
//             Debug.LogError("[EnemyWaveSystem] enemyPrefab이 null입니다!");
//             return;
//         }
// 
//         if (spawnPoint == null)
//         {
//             Debug.LogError("[EnemyWaveSystem] spawnPoint가 null입니다!");
//             return;
//         }
// 
//         GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
//         Debug.Log($"[EnemyWaveSystem] 적 생성: {prefab.name}");
//     }
// }