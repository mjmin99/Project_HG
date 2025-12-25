using UnityEngine;

public class WorldStageList : MonoBehaviour
{
    [SerializeField] private int world; // 1~5
    [SerializeField] private Transform contentRoot;
    [SerializeField] private StageButtonUI stageButtonPrefab;
    [SerializeField] private StageDatabaseSO stageDatabase;

    private StageSaveService stageSave;

    private void Awake()
    {
        stageSave = FindAnyObjectByType<StageSaveService>();

        if (stageSave == null)
            Debug.LogError("[WorldStageList] StageSaveService not found!");
    }

    private void OnEnable()
    {
        Build();
    }

    private void Build()
    {
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        for (int stage = 1; stage <= 5; stage++)
        {
            if (!stageDatabase.TryGet(world, stage, out var stageData))
                continue;

            bool canEnter = stageSave.CanEnter(world, stage);
            bool isCleared = stageSave.IsCleared(world, stage);

            var btn = Instantiate(stageButtonPrefab, contentRoot);
            btn.Bind(world, stage, canEnter, isCleared, OnStageSelected);
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
