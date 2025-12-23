// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// 
// public class BattleResultUI : MonoBehaviour
// {
//     public static BattleResultUI Instance;
// 
//     [SerializeField] GameObject panel;
//     [SerializeField] TMP_Text titleText;
//     [SerializeField] TMP_Text rewardText;
//     [SerializeField] Button btnOK;
// 
//     private void Awake()
//     {
//         Instance = this;
//         panel.SetActive(false);
// 
//         btnOK.onClick.RemoveAllListeners();
//     }
// 
//     public void ShowVictory(int goldReward)
//     {
//         panel.SetActive(true);
//         titleText.text = "VICTORY!";
//         titleText.color = Color.yellow;
// 
//         rewardText.text = $"+ {goldReward} Gold";
// 
//         btnOK.onClick.RemoveAllListeners();
//         btnOK.onClick.AddListener(() => OnVictoryConfirm(goldReward));
//     }
// 
//     public void ShowDefeat()
//     {
//         panel.SetActive(true);
//         titleText.text = "DEFEAT...";
//         titleText.color = Color.red;
// 
//         rewardText.text = "No Rewards";
// 
//         btnOK.onClick.RemoveAllListeners();
//         btnOK.onClick.AddListener(OnDefeatConfirm);
//     }
// 
//     private void OnVictoryConfirm(int goldReward)
//     {
//         // 1) 보상 지급
//         SaveManager.Instance.AddGold(goldReward);
// 
//         // 2) 스테이지 진행도 갱신
//         StageId cleared = StageContext.SelectedStage;
//         var data = SaveManager.Instance.CurrentData;
// 
//         // 클리어한 스테이지가 현재 기록보다 앞서면 업데이트
//         if (cleared.world > data.clearedWorld ||
//             (cleared.world == data.clearedWorld && cleared.stage > data.clearedStage))
//         {
//             data.clearedWorld = cleared.world;
//             data.clearedStage = cleared.stage;
// 
//             Debug.Log($"[BattleResultUI] 스테이지 진행도 업데이트: {cleared}");
//         }
// 
//         // 3) 저장
//         SaveManager.Instance.SaveCurrentUser();
// 
//         // 4) 메인씬 이동
//         SceneChanger.Instance.LoadScene("MainScene");
//     }
// 
//     private void OnDefeatConfirm()
//     {
//         SceneChanger.Instance.LoadScene("MainScene");
//     }
// }