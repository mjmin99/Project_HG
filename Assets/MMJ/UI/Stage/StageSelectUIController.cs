using UnityEngine;
using UnityEngine.UI;

public class StageSelectUIController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private StageButtonUI stageButtonPrefab;

    [Header("Data")]
    [SerializeField] private StageDatabaseSO stageDatabase;

    private StageSaveService stageSave;

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
        stageSave = FindAnyObjectByType<StageSaveService>();
    }

    public void Open()
    {
        panelRoot.SetActive(true);
        BuildStageButtons();
    }

    public void Close()
    {
        panelRoot.SetActive(false);
    }

    private void BuildStageButtons()
    {
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        // 예: 월드 1~5, 스테이지 1~5
        for (int w = 1; w <= 5; w++)
        {
            for (int s = 1; s <= 5; s++)
            {
                if (!stageDatabase.TryGet(w, s, out var stageData))
                    continue;

                bool canEnter = stageSave.CanEnter(w, s);
                bool isCleared = stageSave.IsCleared(w, s);

                var btn = Instantiate(stageButtonPrefab, contentRoot);
                btn.Bind(w, s, canEnter, isCleared, OnStageSelected);
            }
        }
    }

    private void OnStageSelected(int world, int stage)
    {
        if (!stageDatabase.TryGet(world, stage, out var stageData))
            return;

        StageRuntimeManager.Instance.SetStage(stageData);
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
    }
}
