// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
//
// public class StagePageUI : MonoBehaviour
// {
//     [Header("UI")]
//     [SerializeField] private TMP_Text label;
//     [SerializeField] private Button button;
//
//     [Header("Optional Lock Visual")]
//     [SerializeField] private GameObject lockIcon;
//     [SerializeField] private Image dim;
//
//     private StageId stageId;
//     private bool canEnter;
//
//     public void Init(StageId id)
//     {
//         stageId = id;
//         canEnter = StageProgressUtil.CanEnter(id.world, id.stage);
//
//         label.text = id.ToString();
//
//         button.interactable = canEnter;
//
//         if (lockIcon != null)
//             lockIcon.SetActive(!canEnter);
//
//         if (dim != null)
//             dim.enabled = !canEnter;
//
//         button.onClick.RemoveAllListeners();
//         if (canEnter)
//         {
//             button.onClick.AddListener(OnClickStage);
//         }
//     }
//
//     void OnClickStage()
//     {
//         StageContext.SelectedStage = stageId;
//         SceneChanger.Instance.LoadScene("BattleScene");
//     }
// }
