using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text goldText;
    public ResultPanelController resultPanel;

    [Header("Buttons")]
    public Button goMainSceneButton;
    public Button drawOneButton;
    public Button drawTenButton;
    public Button drawHundredButton;

    private GachaService gacha;

    private void Awake()
    {
        goMainSceneButton.onClick.AddListener(BackToMain);

        gacha = new GachaService(SaveManager.Instance, CharacterManager.Instance);

        drawOneButton.onClick.AddListener(() => OnClickDraw(new GachaDrawConfig(1, 0)));    // 1뽑: 보장 없음
        drawTenButton.onClick.AddListener(() => OnClickDraw(new GachaDrawConfig(10, 3)));  // 10뽑: 3성 이상 1개 보장
        drawHundredButton.onClick.AddListener(() => OnClickDraw(new GachaDrawConfig(100, 5))); // 100뽑: 5성 이상 1개 보장
    }

    private void Start()
    {
        UpdateGoldUI();
    }

    void UpdateGoldUI()
    {
        goldText.text = SaveManager.Instance.CurrentData.gold.ToString();
    }

    private void OnClickDraw(GachaDrawConfig config)
    {
        int spent;
        var results = gacha.DrawWithGold(config, out spent);

        if (results == null)
        {
            Debug.Log("[ShopManager] 골드 부족 또는 가챠 실패");
            return;
        }

        UpdateGoldUI();

        // 통합 결과 UI로 표시
        resultPanel.ShowMany(results);
    }

    public void BackToMain()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void AddTestGold()
    {
        SaveManager.Instance.AddGold(10000);
        SaveManager.Instance.SaveCurrentUser();
        UpdateGoldUI();

        Debug.Log("[DEV] 테스트 골드 +10000 지급");
    }
}
