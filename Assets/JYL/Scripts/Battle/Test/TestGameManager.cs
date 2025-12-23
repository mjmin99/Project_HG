using UnityEngine;

public class TestGameManager : MonoBehaviour
{
    // TODO: 인게임에서는 UI에 의해 스테이지 선택 시 정해짐
    [SerializeField] private StageDataSO testStageData;

    // TODO: 임시 캐릭터 파티 편성.
    // 실제로는 GameManager측에서 CharacterInstance와 Model을 가지고
    // BattleManager 쪽에서 컨트롤러 생성해야 함
    [SerializeField] private TestCharController[] characters;
    [SerializeField] private TestEnemyController[] enemies;

    public int curStageWorld;
    public int curStageLevel;
    void Awake()
    {
        curStageWorld = testStageData.world;
        curStageLevel = testStageData.stage;
    }

    public TestCharController[] GetParty()
    {
        return characters;
    }

    public TestEnemyController[] GetEnemies()
    {
        return enemies;
    }

    public StageDataSO GetTestStageData()
    {
        return testStageData;
    }
}
