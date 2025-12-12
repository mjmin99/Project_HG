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
            SaveManager.Instance.CurrentData.gold += goldReward;
            SaveManager.Instance.SaveCurrentUser();
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
