using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class WorldStageList : MonoBehaviour
{
    [SerializeField] private int world; // 1~5
    [SerializeField] private Transform contentRoot;
    [SerializeField] private StageButtonUI stageButtonPrefab;
    [SerializeField] private StageDatabaseSO stageDatabase;

    private StageSaveService stageSave;

    private void Awake()
    {
        stageSave = Manager.Game.stageService;

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

    // todo 어드레서블 수정 중 추후 삭제 예정
    // private void OnStageSelected(int world, int stage)
    // {
    //     if (!stageDatabase.TryGet(world, stage, out var stageData))
    //         return;
    //     
    //     UIManager.Instance.CloseTop();
    //     Manager.Game.SetStageData(stageData);
    //     // UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
    //     // 어드레서블 수정 중
    //     Addressables.LoadSceneAsync("Scene/Battle").ToUniTask();
    // }

    private void OnStageSelected(int world, int stage)
    {
        EnterBattle(world, stage).Forget();
    }

    private async UniTask EnterBattle(int world, int stage)
    {
        if (!stageDatabase.TryGet(world, stage, out var stageData))
            return;

        // 스테이지 데이터 저장
        Manager.Game.SetStageData(stageData);

        // 스테이지 선택 UI 닫기
        UIManager.Instance.CloseTop();

        // 배틀 씬 로드 (Addressables)
        await Addressables.LoadSceneAsync("Scene/Battle").ToUniTask();
    }
}
