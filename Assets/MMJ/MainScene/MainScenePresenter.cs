using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScenePresenter : MonoBehaviour
{
    public Button shopButton;
    // public TestStageSelectUIController testStageSelectUI;
    public Button expeditionButton;
    public Button OptionButton;
    private void Awake()
    {
        shopButton.onClick.AddListener(GoToShop);
        expeditionButton.onClick.AddListener(OnClickExpedition);
        OptionButton.onClick.AddListener(OnClickOption);
    }

    private void Start()
    {
        _ = CheckFirstBase();
        UpdateGoldUI();

        Debug.Log("<color=lime>MainScene 시작</color>");

        // 보유 캐릭터 로그 찍어보기
        foreach (var pair in Manager.Character.instances)
        {
            var inst = pair.Value;
            var model = Manager.Character.models[inst.id];
            string ownedStr = inst.isOwned ? "보유" : "미보유";
            // Debug.Log($"캐릭터 id={inst.id}, name={model.characterName}, 상태={ownedStr}, 레벨={inst.level}");
        }

        // 여기서 PartyUI.Initialize() 같은거 호출하면 됨
        Manager.Audio.SwapClip(AudioClipType.BGM, "MainBGM").Forget();
    }

    // 게임 첫 시작 시 재생하는 다이얼로그
    private async UniTask CheckFirstBase()
    {
        if (!Manager.Dialog.CheckDialogCondition(DialogCondition.IsFirstRun))
        {
            await Manager.Dialog.StartDialog(DialogKey.Prologue);
            Manager.Dialog.MarkDialogCondition(DialogCondition.IsFirstRun);
        }
        if (!Manager.Dialog.CheckDialogCondition(DialogCondition.IsFirstBase))
        {
            await Manager.Dialog.StartDialog(DialogKey.Scene1);
            Manager.Dialog.MarkDialogCondition(DialogCondition.IsFirstBase);
        }
        // 여기서 기본 캐릭터 지급
        Manager.Character.GiveCharacter(9);
        Manager.Character.GiveCharacter(10);
        Manager.Character.GiveCharacter(11);
    }

    public void UpdateGoldUI()
    {
        int gold = Manager.Save.CurrentData.gold;
    }

    public void GoToShop() // 배틀씬 버튼 처럼 나중에 버튼에 직접 달아서 움직이게 역할 주는 것도 괜찮을듯
    {
        UIManager.Instance.OpenUI<UIPanel>("ShopPanel");
        // 또는 SceneChanger.Instance.LoadScene("ShopScene");
    }

    public void OnClickExpedition()
    {
        UIManager.Instance.OpenUI<StageSelectPanel>("StageSelectPanel");
    }

    public void OnClickOption()
    {
        UIManager.Instance.OpenUI<OptionPanel>("OptionPanel");
    }
}
