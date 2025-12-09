using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerController[] players;
    public Transform[] spawnPoints;

    private void Awake()
    {
        Instance = this;

        // CSV 로드
        List<CharacterModel> models = CharacterCSVLoader.Load();
        CharacterManager.Instance.LoadModels(models);

        // 프리팹 연결
        foreach (var model in CharacterManager.Instance.models.Values)
        {
            model.prefab = Resources.Load<GameObject>($"Characters/{model.name}");
            if (model.prefab == null)
                Debug.LogWarning($"Prefab not found for {model.name}");
        }
    }

    private void Start()
    {
        // Firebase 로딩이 끝날 때까지 대기
        StartCoroutine(WaitAndLoadParty());
    }

    private System.Collections.IEnumerator WaitAndLoadParty()
    {
        while (SaveManager.Instance.CurrentData == null)
            yield return null;

        LoadParty();
    }

    public void LoadParty()
    {
        int[] party = SaveManager.Instance.CurrentData.partySet;

        Debug.Log($"파티 데이터: [{party[0]}, {party[1]}, {party[2]}]");

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
                Debug.LogError($"모델에 ID {id} 없음");
                continue;
            }

            if (!CharacterManager.Instance.instances.ContainsKey(id))
            {
                Debug.LogError($"CharacterInstance에 ID {id} 없음");
                continue;
            }

            CharacterStats stats = CharacterManager.Instance.GetStats(id);
            players[i].ApplyCharacter(id, stats);
        }

        Debug.Log("파티 로딩 완료!");
    }
}
