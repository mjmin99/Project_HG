// using UnityEngine;
// using UnityEngine.UI;
//
// public class BattleOptionPanel : UIPanel
// {
//     [Header("Buttons")]
//     [SerializeField] private Button btnClose;
//     [SerializeField] private Button btnRetry; 
//     [SerializeField] private Button btnGiveup;
//     [SerializeField] private Button btnOption;
//     protected override void Awake()
//     {
//         base.Awake();
//
//         btnClose.onClick.AddListener(() =>
//         {
//             UIManager.Instance.CloseTop();
//         });
//
//         btnRetry.onClick.AddListener(() =>
//         {
//             RetryBattle();
//         });
//
//         btnGiveup.onClick.AddListener(() =>
//         {
//             GiveUpBattle();
//         });
//
//         btnOption.onClick.AddListener(() =>
//         {
//             OpenOption();
//         });
//     }
//
//     private void OpenOption()
//     {
//         UIManager.Instance.OpenUI<OptionPanel>("OptionPanel");
//     }
//
//     public override void OnOpen()
//     {
//         base.OnOpen();
//         PauseBattle();
//     }
//
//     public override void OnClose()
//     {
//         ResumeBattle();
//         base.OnClose();
//     }
//
//     private void PauseBattle()
//     {
//         Time.timeScale = 0f;
//     }
//
//     private void ResumeBattle()
//     {
//         Time.timeScale = 1f;
//     }
//
//     private void RetryBattle()
//     {
//         Time.timeScale = 1f;
//         // BattleManager.Instance.Restart();
//     }
//
//     private void GiveUpBattle()
//     {
//         Time.timeScale = 1f;
//         // BattleManager.Instance.GiveUp();
//     }
//
//
// }
