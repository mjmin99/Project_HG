using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleOptionPanel : UIPanel
{
    [Header("Buttons")]
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnRetry; 
    [SerializeField] private Button btnGiveUp;
    [SerializeField] private Button btnOption;
    [SerializeField] private TMP_Text gameOverText;
    
    protected override void Awake()
    {
        base.Awake();

        btnClose.onClick.AddListener(() =>
        {
            UIManager.Instance.CloseTop();
        });

        btnRetry.OnClickAsObservable().Subscribe(_ => RetryBattle().Forget());

        btnGiveUp.OnClickAsObservable().Subscribe(_ => GiveUpBattle().Forget());

        btnOption.onClick.AddListener(OpenOption);
        
        btnOption.gameObject.SetActive(!Manager.Game.IsGameOver);
        gameOverText.gameObject.SetActive(Manager.Game.IsGameOver);
        btnClose.gameObject.SetActive(!Manager.Game.IsGameOver);
    }

    private void OpenOption()
    {
        UIManager.Instance.OpenUI<OptionPanel>("OptionPanel");
    }

    public override void OnOpen()
    {
        base.OnOpen();
        PauseBattle();
    }

    public override void OnClose()
    {
        ResumeBattle();
        base.OnClose();
    }

    private void PauseBattle() => Time.timeScale = 0f;

    private void ResumeBattle() => Time.timeScale = 1f;

    private async UniTaskVoid RetryBattle()
    {
        Debug.Log("스테이지 재시작");
        Time.timeScale = 1f;
        UIManager.Instance.CloseTop();
        await UniTask.WhenAll(Manager.Game.tasks);
        SceneManager.LoadScene("BattleScene");
        Manager.Game.IsGameOver = false;
        Manager.Game.IsGameClear = false;
    }

    private async UniTaskVoid GiveUpBattle() // 현재 스테이지 포기. 씬전환
    {
        Debug.Log("스테이지 포기");
        Time.timeScale = 1f;
        UIManager.Instance.CloseTop();
        await UniTask.WhenAll(Manager.Game.tasks);
        DOTween.KillAll();
        Manager.Game.ClearCharacters();
        Manager.Game.IsBattle = false;
        Manager.Game.IsGameOver = false;
        Manager.Game.IsGameClear = false;
        SceneManager.LoadScene("MainScene");
    }
}
