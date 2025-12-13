using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerController[] players;
    public Transform[] spawnPoints;

    private void Awake()
    {
        Instance = this;

        if (CharacterManager.Instance.models.Count == 0)
        {
            Debug.LogError("[GameManager] CharacterManager.models가 비어있음! LobbyPanel을 먼저 거쳐야 합니다.");
            return;
        }

        Debug.Log($"[GameManager] 캐릭터 모델 {CharacterManager.Instance.models.Count}개 확인됨");
    }

    private void Start()
    {
        StartCoroutine(WaitAndLoadParty());
    }

    private IEnumerator WaitAndLoadParty()
    {
        while (SaveManager.Instance.CurrentData == null)
        {
            yield return null;
        }

        LoadParty();
    }

    public void LoadParty()
    {
        int[] party = SaveManager.Instance.CurrentData.partySet;

        Debug.Log($"[GameManager] 파티 데이터: [{party[0]}, {party[1]}, {party[2]}]");

        for (int i = 0; i < players.Length; i++)
        {
            int id = party[i];

            if (id == -1)
            {
                players[i].ClearCharacter();
                continue;
            }

            if (!CharacterManager.Instance.models.ContainsKey(id))
            {
                Debug.LogError($"[GameManager] 모델에 ID {id} 없음");
                continue;
            }

            if (!CharacterManager.Instance.instances.ContainsKey(id))
            {
                Debug.LogError($"[GameManager] CharacterInstance에 ID {id} 없음");
                continue;
            }

            CharacterStats stats = CharacterManager.Instance.GetStats(id);
            players[i].ApplyCharacter(id, stats);
        }

        Debug.Log("[GameManager] 파티 로딩 완료!");
    }
}