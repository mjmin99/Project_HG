using UnityEngine;

public class BattleLoadManager : MonoBehaviour
{
    public Transform[] playerSpawnPoints; // 전투씬에서 3개 미리 배치
                                          // P0, P1, P2

    private void Start()
    {
        LoadPlayersFromParty();
    }

    void LoadPlayersFromParty()
    {
        int[] party = SaveManager.Instance.CurrentData.partySet;

        for (int i = 0; i < party.Length; i++)
        {
            int id = party[i];
            if (id == -1) continue;

            CharacterModel model = CharacterManager.Instance.models[id];
            CharacterStats stats = CharacterManager.Instance.GetStats(id);

            // 🔥 전투 전용 루트
            GameObject battleRoot = new GameObject($"BattlePlayer_{id}");
            battleRoot.transform.position = playerSpawnPoints[i].position;

            // 캐릭터 비주얼 (기존 프리팹)
            GameObject visual = Instantiate(model.prefab, battleRoot.transform);

            // 전투 유닛
            PlayerBattleUnit unit = battleRoot.AddComponent<PlayerBattleUnit>();
            unit.Init(id, stats);
        }
    }
}
