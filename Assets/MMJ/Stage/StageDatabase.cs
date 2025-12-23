// using UnityEngine;
// using System.Collections.Generic;
//
// public class StageDatabase : MonoBehaviour
// {
//     public static StageDatabase Instance;
//
//     Dictionary<string, StageData> stageMap = new();
//
//     private void Awake()
//     {
//         Instance = this;
//         LoadFromCSV();
//     }
//
//     void LoadFromCSV()
//     {
//         TextAsset csv = Resources.Load<TextAsset>("Data/StageConfig");
//         string[] lines = csv.text.Split('\n');
//
//         for (int i = 1; i < lines.Length; i++)
//         {
//             if (string.IsNullOrWhiteSpace(lines[i])) continue;
//
//             string[] cols = lines[i].Split(',');
//
//             int world = int.Parse(cols[0]);
//             int stage = int.Parse(cols[1]);
//             int reward = int.Parse(cols[2]);
//             string waveProfileName = cols[3].Trim();
//
//             WaveProfileSO waveProfile =
//                 Resources.Load<WaveProfileSO>($"Data/Waves/{waveProfileName}");
//
//             StageData data = new StageData
//             {
//                 id = new StageId(world, stage),
//                 rewardGold = reward,
//                 waveProfile = waveProfile
//             };
//
//             stageMap.Add($"{world}-{stage}", data);
//         }
//     }
//
//     public StageData GetStage(StageId id)
//     {
//         stageMap.TryGetValue(id.ToString(), out var data);
//         return data;
//     }
// }
