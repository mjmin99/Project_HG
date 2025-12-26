using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 상점 메인 패널
/// - UIManager 기반 UIPanel
/// - 캐릭터 가챠 전용 (1 / 10 / 100)
/// </summary>
public class ShopPanel : UIPanel
{
    [Header("UI")]
    [SerializeField] private TMP_Text goldText;

    [Header("Draw Buttons")]
    [SerializeField] private Button draw1Button;
    [SerializeField] private Button draw10Button;
    [SerializeField] private Button draw100Button;

    [Header("Close")]
    [SerializeField] private Button closeButton;

    [Header("DEV")]
    [SerializeField] private Button devAddGoldButton; // [DEV] +10000 Gold

    private GachaService gacha;

    #region Life Cycle

    public override void OnOpen()
    {
        // 도메인 서비스 생성
        gacha = new GachaService(
            Manager.Save,
            Manager.Character
        );

        // 버튼 바인딩
        BindButtons();

        // 초기 UI 갱신
        UpdateGoldUI();

#if !UNITY_EDITOR
        // 빌드에서는 DEV 버튼 숨김
        if (devAddGoldButton != null)
            devAddGoldButton.gameObject.SetActive(false);
#endif
    }
    #endregion

    #region Button Binding

    private void BindButtons()
    {
        draw1Button.onClick.RemoveAllListeners();
        draw10Button.onClick.RemoveAllListeners();
        draw100Button.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();

        draw1Button.onClick.AddListener(OnClickDraw1);
        draw10Button.onClick.AddListener(OnClickDraw10);
        draw100Button.onClick.AddListener(OnClickDraw100);
        closeButton.onClick.AddListener(OnClickClose);

        if (devAddGoldButton != null)
        {
            devAddGoldButton.onClick.RemoveAllListeners();
            devAddGoldButton.onClick.AddListener(AddTestGold);
        }
    }

    #endregion

    #region Draw Logic

    private void OnClickDraw1()
    {
        Draw(new GachaDrawConfig(
            drawCount: 1,
            guaranteeMinRarity: 0
        ));
    }

    private void OnClickDraw10()
    {
        Draw(new GachaDrawConfig(
            drawCount: 10,
            guaranteeMinRarity: 3
        ));
    }

    private void OnClickDraw100()
    {
        Draw(new GachaDrawConfig(
            drawCount: 100,
            guaranteeMinRarity: 5
        ));
    }

    private void Draw(GachaDrawConfig config)
    {
        int spentGold;
        var results = gacha.DrawWithGold(config, out spentGold);

        if (results == null || results.Count == 0)
        {
            Debug.Log("[ShopPanel] 가챠 실패 (골드 부족 또는 오류)");
            return;
        }

        UpdateGoldUI();

        // 결과 팝업 열기
        var popup = UIManager.Instance
            .OpenUI<UIPopup>("GachaResultPopup") as GachaResultPopup;

        if (popup == null)
        {
            Debug.LogError("[ShopPanel] GachaResultPopup 열기 실패");
            return;
        }

        popup.ShowMany(results);
    }

    #endregion

    #region UI Update

    private void UpdateGoldUI()
    {
        if (goldText != null && Manager.Save.CurrentData != null)
        {
            goldText.text = Manager.Save.CurrentData.gold.ToString();
        }
    }

    #endregion

    #region DEV Tools

    private void AddTestGold()
    {
        Manager.Save.AddGold(10000);
        Manager.Save.SaveCurrentUser();
        UpdateGoldUI();

        Debug.Log("[DEV] 테스트 골드 +10000 지급");
    }

    #endregion

    #region Close

    private void OnClickClose()
    {
        UIManager.Instance.CloseTop();
    }

    #endregion
}
