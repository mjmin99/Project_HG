using System.Linq;
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
    // 진짜 온 스테이지 셀렉티드 함수임!!!
    private void OnStageSelected(int world, int stage)
    {
        if (!stageDatabase.TryGet(world, stage, out var stageData))
            return;
        
        if (Manager.Save.CurrentData.partySet.Contains(-1)) // 파티 구성이 완료되어야 출발 가능
        {
            ToastUtil.Error("파티 구성을 완료해주세요!");
            return;
        }
        UIManager.Instance.CloseTop();
        Manager.Game.SetStageData(stageData);
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
    }


    // // TODO : 테스트 함수. 테스트 종료 후 삭제예정
    // // 여기는 그냥 스테이지를 누르면 임시로 클리어를 세이브로 보낼 수 있도록 민만준이 만든것
    // private void OnStageSelected(int world, int stage)
    // {
    //     // 1. StageSaveService 찾기
    //     if (stageSave == null)
    //     {
    //         Debug.LogError("[WorldStageList] StageSaveService not found");
    //         return;
    //     }
    //
    //     // 2. 이미 클리어한 스테이지일 경우 빠지는 경로 -> 여기 그냥 입장 가능하게 바꿔서 넣어야할듯
    //     if (stageSave.IsCleared(world, stage))
    //     {
    //         ToastUtil.Error("이미 클리어한 스테이지입니다");
    //         return;
    //     }
    //
    //     // 3. 테스트용 클리어 데이터
    //     long fakeClearTime = Random.Range(30_000, 120_000);
    //     int fakeScore = Random.Range(1000, 5000);
    //     int fakeStars = Random.Range(1, 3);
    //
    //     // 4. 세이브 데이터에 클리어 반영
    //     stageSave.ApplyClearResult(
    //         world,
    //         stage,
    //         fakeClearTime,
    //         fakeScore,
    //         fakeStars
    //     );
    //
    //     // 5. Firebase / 로컬 세이브 저장
    //     Manager.Save.SaveCurrentUser();
    //
    //     // 6. 토스트 표시
    //     ToastUtil.Success($"W{world}-{stage} 스테이지 완료!");
    //
    //     Debug.Log($"[TEST CLEAR] W{world}-{stage} 클리어 저장됨");
    //
    //     // 7. (선택) UI 즉시 갱신
    //     Build();
    // }

}
