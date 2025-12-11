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

        Debug.Log($"전투씬 파티 로드: [{party[0]}, {party[1]}, {party[2]}]");

        for (int i = 0; i < party.Length; i++)
        {
            int id = party[i];

            if (id == -1)
            {
                Debug.Log($"Slot {i} 비어 있음 → 스킵");
                continue;
            }

            // 모델/스탯 얻기
            CharacterModel model = CharacterManager.Instance.models[id];
            CharacterStats stats = CharacterManager.Instance.GetStats(id);

            // 프리팹 instantiate
            GameObject obj = Instantiate(model.prefab, playerSpawnPoints[i].position, Quaternion.identity);

            // 전투용 유닛 스크립트 붙이기
            PlayerBattleUnit unit = obj.AddComponent<PlayerBattleUnit>();
            unit.Init(id, stats);
        }

        Debug.Log("전투씬 파티 배치 완료!");
    }
}
