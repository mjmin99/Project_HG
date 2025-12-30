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
            .Subscribe(_ => RestartStage())
            .AddTo(this);
        returnButton
            .OnClickAsObservable()
            .Subscribe(_ => ReturnToBase())
            .AddTo(this);
    }

    private static void RestartStage()
    {
        Manager.Game.IsBattle = false;
        Manager.Game.IsGameClear = false;
        Manager.Game.IsGameOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("BattleScene");
    }

    private static void ReturnToBase()
    {
        Manager.Game.IsBattle = false;
        Manager.Game.IsGameClear = false;
        Manager.Game.IsGameOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
    }
}
