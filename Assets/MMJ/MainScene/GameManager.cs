using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerController[] players;
    public Transform[] spawnPoints;

    // 추후에 할 일 1단계 – 로딩 파이프라인 정리
    // - CSV 로드 위치를 “한 곳”으로 고정
    // - SaveManager.CreateDefaultSaveData에서 models 비어 있을 때 방어 코드 추가
    // - GameManager에서 중복 CSV 로드 제거

    private void Awake() //todo 현재 게임매니저에서도 CSV를 로드하기때문에 추후 삭제할 가능성 있음.
    {
        Instance = this;

        if (CharacterManager.Instance.models.Count > 0)
        {
            Debug.Log("CSV 이미 로드됨");
            LoadPrefabs();
            return;
        }

        // CSV 로드
        List<CharacterModel> models = CharacterCSVLoader.Load();
        CharacterManager.Instance.LoadModels(models);
        LoadPrefabs();
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

    private void LoadPrefabs()
    {
        foreach (var model in CharacterManager.Instance.models.Values)
        {
            model.prefab = Resources.Load<GameObject>($"Characters/{model.characterName}");
            if (model.prefab == null)
                Debug.LogWarning($"Prefab not found for {model.characterName}");
        }
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
