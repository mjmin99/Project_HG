// using System.Collections.Generic;
// using UnityEngine;
//
// // SO 기반 전체 스테이지 데이터베이스
// // 런타임 읽기 전용
//
// [CreateAssetMenu(menuName = "Stage/Stage Database", fileName = "StageDatabaseSO")]
// public class StageDatabaseSO : ScriptableObject
// {
//     [SerializeField] private List<StageDataSO> stages = new();
//
//     // (world-stage) → StageData 캐시
//     //private Dictionary<string, StageDataSO> cache;
//     //private Dictionary<StageDataSO, bool> isClear;
//
//     // 스테이지 조회
//     public bool TryGet(int world, int stage, out StageDataSO data)
//     {
//         if (cache == null) BuildCache();
//         return cache.TryGetValue(StageKeyUtil.ToKey(world, stage), out data);
//     }
//     
//     // 런타임 조회용 캐시 생성
//     private void BuildCache()
//     {
//         cache = new Dictionary<string, StageDataSO>(stages.Count);
//         isClear = new Dictionary<StageDataSO, bool>(stages.Count);
//         foreach (var s in stages)
//         {
//             if (s == null) continue;
//             cache[StageKeyUtil.ToKey(s.world, s.stage)] = s;
//             isClear[s] = Manager.Save
//         }
//     }
//
//     // 에디터 전용 => 프로젝트 내 모든 StageDataSO를 자동 수집
// #if UNITY_EDITOR
//     [ContextMenu("Auto Refresh (Find All StageDataSO)")]
//     private void AutoRefresh()
//     {
//         stages.Clear();
//
//         string[] guids = UnityEditor.AssetDatabase.FindAssets("t:StageDataSO");
//         foreach (var guid in guids)
//         {
//             string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
//             var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<StageDataSO>(path);
//             if (asset != null) stages.Add(asset);
//         }
//
//         UnityEditor.EditorUtility.SetDirty(this);
//         UnityEditor.AssetDatabase.SaveAssets();
//         Debug.Log($"[StageDatabaseSO] Refreshed. Count={stages.Count}");
//     }
// #endif
// }
