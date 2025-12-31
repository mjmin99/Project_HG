using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageClearPanel : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private TMP_Text clearTimeText;
    [SerializeField] private TMP_Text rewardText;

    public void Init(float record)
    {
        Time.timeScale = 0f;
        var stageData = Manager.Game.GetStageData();
        rewardText.SetText($"{stageData.rewardGold} G");
        float minutes = record / 60;
        float seconds = record % 60;
        clearTimeText.SetText($"{minutes:00} : {seconds:00}");
        restartButton
            .OnClickAsObservable()
            .Subscribe(_ => RestartStage().Forget())
            .AddTo(this);
        returnButton
            .OnClickAsObservable()
            .Subscribe(_ => ReturnToBase().Forget())
            .AddTo(this);
    }

    private static async UniTask RestartStage()
    {
        Manager.Game.IsBattle = false;
        Manager.Game.IsGameClear = false;
        Manager.Game.IsGameOver = false;
        Time.timeScale = 1f;
        
        await UniTask.WhenAll(Manager.Game.tasks);
        
        DOTween.KillAll();
        SceneManager.LoadScene("BattleScene");
    }

    private static async UniTask ReturnToBase()
    {
        Manager.Game.IsBattle = false;
        Manager.Game.IsGameClear = false;
        Manager.Game.IsGameOver = false;
        Time.timeScale = 1f;
        
        await UniTask.WhenAll(Manager.Game.tasks);
        
        DOTween.KillAll();
        await SceneManager.LoadSceneAsync("MainScene");
        if (Manager.Game.GetStageData().stage != 5) return;
        await CheckBossDeadDialog();
    }
    private static async UniTask CheckBossDeadDialog()
    {
        DialogCondition condition;
        switch(Manager.Game.GetStageData().world)
        {
            case 1:
                condition = DialogCondition.WorldBoss1Down;
                if (!Manager.Dialog.CheckDialogCondition(condition))
                {
                    await Manager.Dialog.StartDialog(DialogKey.Scene4);
                }
                break;
            case 5:
                condition = DialogCondition.WorldBoss5Down;
                if (!Manager.Dialog.CheckDialogCondition(condition))
                {
                    await Manager.Dialog.StartDialog(DialogKey.Scene13);
                }
                break;
            default:
                return;
        }
        Manager.Dialog.MarkDialogCondition(condition);
    }
}
