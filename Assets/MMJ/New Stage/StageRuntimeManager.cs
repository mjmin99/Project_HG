// using UnityEngine;
//
// public class StageRuntimeManager : MonoBehaviour
// {
//     public static StageRuntimeManager Instance { get; private set; }
//
//     public StageDataSO CurrentStage { get; private set; }
//
//     private void Awake()
//     {
//         if (Instance != null)
//         {
//             Destroy(gameObject);
//             return;
//         }
//         Instance = this;
//         DontDestroyOnLoad(gameObject);
//     }
//
//     public void SetStage(StageDataSO stage)
//     {
//         CurrentStage = stage;
//     }
//
//     public void ClearStageData()
//     {
//         CurrentStage = null;
//     }
// }
