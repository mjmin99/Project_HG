using UnityEngine;
using UnityEngine.UI;

public class BattleSceneButton : MonoBehaviour
{
    [SerializeField] Button battleSceneButton;

    [Header("Test Stage (나중에 StageSelectUI에서 설정)")]
    [SerializeField] int testWorld = 1;
    [SerializeField] int testStage = 1;

    private void Awake()
    {
        battleSceneButton.onClick.AddListener(OnClickBattleStart);
    }

    public void OnClickBattleStart()
    {
        // StageContext 설정 (테스트용)
        StageContext.SelectedStage = new StageId(testWorld, testStage);

        Debug.Log($"[BattleSceneButton] 스테이지 설정: {StageContext.SelectedStage}");

        SceneChanger.Instance.LoadScene("BattleScene");
    }
}