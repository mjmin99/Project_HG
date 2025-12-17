// using UnityEngine;
// using System.Collections;
// 
// public class BattleResultManager : MonoBehaviour
// {
//     public static BattleResultManager Instance;
// 
//     private int stageRewardGold;
// 
//     private void Awake()
//     {
//         Instance = this;
//     }
// 
//     public void CheckVictory()
//     {
//         // 승리 조건:
//         // EnemyManager에 남은 적이 없고,
//         // 더 이상 웨이브 없음
//         if (EnemyWaveSystem.Instance.IsAllWavesCleared &&
//             EnemyManager.Instance.GetAllEnemies().Count == 0)
//         {
//             ShowVictory();
//         }
//     }
// 
//     public void CheckDefeat()
//     {
//         // 패배 조건:
//         if (PlayerBattleManager.Instance.GetAllPlayers().Count == 0)
//         {
//             ShowDefeat();
//         }
//     }
// 
//     void ShowVictory()
//     {
//         if (BattleResultUI.Instance == null)
//         {
//             Debug.LogError("BattleResultUI.Instance == null");
//             return;
//         }
// 
//         BattleResultUI.Instance.ShowVictory(stageRewardGold);
//     }
// 
//     void ShowDefeat()
//     {
//         Debug.Log("전투 패배!");
//         BattleResultUI.Instance.ShowDefeat();
//     }
// 
//     public void SetStageReward(int gold)
//     {
//         stageRewardGold = gold;
//     }
// 
// }
