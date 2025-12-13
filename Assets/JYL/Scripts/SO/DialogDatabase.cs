// using System.Collections.Generic;
// using UnityEngine;
//
// [CreateAssetMenu(fileName = "DialogDB", menuName = "Dialog")]
// public class DialogDatabase : ScriptableObject
// {
//     // 스크립터블 오브젝트로 관리하게 되면 사용하도록 함
//     /// <summary>
//     ///  개선안 : CSV 파일 하나에서 대화를 전부 읽어낸 것을
//     /// 청크(장소, 대화 씬) 별로 구별하여 SO로 만들어 관리함.
//     /// 
//     /// </summary>
//     
//     //[SerializeField] private string csvPath = "CSV/TestDialog";
//     private readonly Dictionary<string, Dialog> dialogs = new();
//
//     [ContextMenu("Set Database")]
//     public void SetDatabase()
//     {
//         dialogs.Clear();
//         //Dialogs = Util.ParseCsvToDialogs(csvPath);
//     }
//     
// }
