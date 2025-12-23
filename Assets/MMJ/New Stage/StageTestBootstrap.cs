// using UnityEngine;
//
// public class StageTestBootstrap : MonoBehaviour
// {
//     [Header("Stage")]
//     public StageDatabaseSO stageDatabase;
//     public int testWorld = 1;
//     public int testStage = 1;
//
//     [Header("Fake Result (for test)")]
//     public bool autoClearAfterStart = true;
//     public float fakeClearDelay = 5f;
//
//     private void Start()
//     {
//         Debug.Log("<color=cyan>[Test] StageTestBootstrap Start</color>");
//
//         // 1. 스테이지 데이터 조회
//         if (!stageDatabase.TryGet(testWorld, testStage, out var stageData))
//         {
//             Debug.LogError("[Test] StageData를 찾을 수 없습니다!");
//             return;
//         }
//
//         Debug.Log($"[Test] Stage Loaded: W{stageData.world}-S{stageData.stage}");
//         Debug.Log($"[Test] Waves Count: {stageData.waves.Count}");
//
//         // 2. (전투팀 API 가정) 웨이브 전달
//         // 실제 전투 시스템에 맞게 연결
//         DebugApplyWaves(stageData);
//
//         // 3. 테스트용 자동 클리어
//         if (autoClearAfterStart)
//         {
//             Invoke(nameof(FakeClear), fakeClearDelay);
//         }
//     }
//
//     void DebugApplyWaves(StageDataSO stageData)
//     {
//         for (int w = 0; w < stageData.waves.Count; w++)
//         {
//             var wave = stageData.waves[w];
//             Debug.Log($"[Test] Wave {w + 1} / waitBeforeWave={wave.waitBeforeWave}");
//
//             foreach (var spawn in wave.spawns)
//             {
//                 Debug.Log($"Spawn monsterId={spawn.id}, count={spawn.count}, interval={spawn.spawnInterval}");
//             }
//         }
//
//         // ⚠️ 실제 전투팀 코드 연결 시 여기에 SetWaves 호출
//         // EnemyWaveSystem.Instance.SetWaves(...)
//     }
//
//     void FakeClear()
//     {
//         Debug.Log("<color=lime>[Test] Fake Clear Triggered</color>");
//
//         var service = new StageProgressService(
//             SaveManager.Instance.CurrentData.stageProgress
//         );
//
//         // 테스트용 결과 값
//         service.ApplyClearResult(
//             testWorld,
//             testStage,
//             clearTimeMs: 123456,
//             score: 9999,
//             stars: 3
//         );
//
//         SaveManager.Instance.SaveCurrentUser();
//     }
// }
