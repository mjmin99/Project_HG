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

        if (Manager.Character.models.Count == 0)
        {
            Debug.LogError("[GameManager] CharacterManager.models가 비어있음! LobbyPanel을 먼저 거쳐야 합니다.");
            return;
        }

        Debug.Log($"[GameManager] 캐릭터 모델 {Manager.Character.models.Count}개 확인됨");
    }

    private void OnEnable()
    {
        PartyService.OnPartyChanged += OnPartyChanged;
    }

    private void OnDisable()
    {
        PartyService.OnPartyChanged -= OnPartyChanged;
    }

    private void Start()
    {
        StartCoroutine(WaitAndLoadParty());
    }

    private IEnumerator WaitAndLoadParty()
    {
        while (Manager.Save.CurrentData == null)
            yield return null;

        LoadParty();
    }

    private void OnPartyChanged(int[] partySet, int changedSlot, int characterId)
    {
        // 메인씬에서도 즉시 반영
        LoadParty();
    }

    public void LoadParty()
    {
        int[] party = Manager.Save.CurrentData.partySet;

        Debug.Log($"[GameManager] 파티 데이터: [{party[0]}, {party[1]}, {party[2]}]");

        for (int i = 0; i < players.Length; i++)
        {
            int id = party[i];

            if (id == -1)
            {
                players[i].ClearCharacter();
                continue;
            }

            if (!Manager.Character.models.ContainsKey(id))
            {
                Debug.LogError($"[GameManager] 모델에 ID {id} 없음");
                continue;
            }

            if (!Manager.Character.instances.ContainsKey(id))
            {
                Debug.LogError($"[GameManager] CharacterInstance에 ID {id} 없음");
                continue;
            }

            CharacterStats stats = Manager.Character.GetStats(id);
            players[i].ApplyCharacter(id, stats);
        }

        Debug.Log("[GameManager] 파티 로딩 완료!");
    }
}
