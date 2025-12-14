using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleResultUI : MonoBehaviour
{
    public static BattleResultUI Instance;

    [SerializeField] GameObject panel;
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text rewardText;
    [SerializeField] Button btnOK;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowVictory(int goldReward)
    {
        panel.SetActive(true);
        titleText.text = "VICTORY!";
        titleText.color = Color.yellow;

        rewardText.text = $"+ {goldReward} Gold";

        btnOK.onClick.RemoveAllListeners();
        btnOK.onClick.AddListener(() =>
        {
            // 1) 보상 지급
            SaveManager.Instance.CurrentData.gold += goldReward;

            // 2) 스테이지 진행도 갱신
            StageId cleared = StageContext.SelectedStage;
            var data = SaveManager.Instance.CurrentData;

            // 마지막 클리어 스테이지 갱신 (더 앞이면 업데이트)
            if (cleared.world > data.clearedWorld ||
                (cleared.world == data.clearedWorld && cleared.stage > data.clearedStage))
            {
                data.clearedWorld = cleared.world;
                data.clearedStage = cleared.stage;
            }

            // 3) 저장
            SaveManager.Instance.SaveCurrentUser();

            // 4) 메인씬 이동
            SceneChanger.Instance.LoadScene("MainScene");
        });
    }

    public void ShowDefeat()
    {
        panel.SetActive(true);
        titleText.text = "DEFEAT...";
        titleText.color = Color.red;

        rewardText.text = "No Rewards";

        btnOK.onClick.RemoveAllListeners();
        btnOK.onClick.AddListener(() =>
        {
            SceneChanger.Instance.LoadScene("MainScene");
        });
    }
}
