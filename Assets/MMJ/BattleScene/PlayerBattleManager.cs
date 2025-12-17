// using System.Collections.Generic;
// using UnityEngine;
// 
// public class PlayerBattleManager : MonoBehaviour
// {
//     public static PlayerBattleManager Instance;
// 
//     // 현재 전투 중인 플레이어 유닛들
//     private List<PlayerBattleUnit> players = new List<PlayerBattleUnit>();
// 
//     private void Awake()
//     {
//         Instance = this;
//     }
// 
//     // 플레이어 스폰 시 자동 등록
//     public void Register(PlayerBattleUnit unit)
//     {
//         if (!players.Contains(unit))
//             players.Add(unit);
//     }
// 
//     // 플레이어 사망 시 자동 제거
//     public void Unregister(PlayerBattleUnit unit)
//     {
//         if (players.Contains(unit))
//             players.Remove(unit);
//     }
// 
//     /// <summary>
//     /// 적이 가까운 플레이어 찾을 때 호출
//     /// </summary>
//     public PlayerBattleUnit GetClosestPlayer(Vector3 fromPos)
//     {
//         float minDist = float.MaxValue;
//         PlayerBattleUnit closest = null;
// 
//         foreach (var player in players)
//         {
//             if (player == null) continue;
// 
//             float dist = Vector3.Distance(fromPos, player.transform.position);
//             if (dist < minDist)
//             {
//                 minDist = dist;
//                 closest = player;
//             }
//         }
// 
//         return closest;
//     }
// 
//     /// <summary>
//     /// 플레이어 유닛 List 반환 (필요하면)
//     /// </summary>
//     public List<PlayerBattleUnit> GetAllPlayers()
//     {
//         return players;
//     }
// }
