// using UnityEngine;
//
// namespace JYL{
// public class TestGameManager : MonoBehaviour
// {
//     [SerializeField] private StageDataSO testStageData;
//
//     // 실제로는 GameManager측에서 CharacterInstance와 Model을 가지고
//     // BattleManager 쪽에서 컨트롤러 생성해야 함
//     [SerializeField] private CharController[] characters;
//     [SerializeField] private TestEnemyController[] enemies;
//
//     public int curStageWorld;
//     public int curStageLevel;
//     void Awake()
//     {
//         curStageWorld = testStageData.world;
//         curStageLevel = testStageData.stage;
//     }
//
//     public CharController[] GetParty()
//     {
//         return characters;
//     }
//
//     public TestEnemyController[] GetEnemies()
//     {
//         return enemies;
//     }
//
//     public StageDataSO GetTestStageData()
//     {
//         return testStageData;
//     }
// }
// }
//
