// using UnityEngine;
// 
// public class BattleStageApplier : MonoBehaviour
// {
//     private void Start()
//     {
//         // 1. 선택된 스테이지 확인
//         StageId selected = StageContext.SelectedStage;
//         Debug.Log($"[BattleStageApplier] Selected Stage = {selected}");
// 
//         // 2. StageDatabase에서 데이터 가져오기
//         StageData stageData = StageDatabase.Instance.GetStage(selected);
//         if (stageData == null)
//         {
//             Debug.LogError("[BattleStageApplier] StageData 없음");
//             return;
//         }
// 
//         // 3. 웨이브 확인
//         if (stageData.waveProfile == null)
//         {
//             Debug.LogError("[BattleStageApplier] WaveProfile 없음");
//             return;
//         }
// 
//         // 4. 웨이브 주입
//         EnemyWaveSystem.Instance.SetWaves(stageData.waveProfile.waves);
// 
//         // 5. 스테이지 보상 주입
//         BattleResultManager.Instance.SetStageReward(stageData.rewardGold);
//         Debug.Log($"[BattleStageApplier] rewardGold = {stageData.rewardGold}");
// 
//         // 6. 웨이브 시작
//         EnemyWaveSystem.Instance.Begin();
// 
//         Debug.Log("[BattleStageApplier] Wave 적용 완료");
//     }
// }
