using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    public Dictionary<int, CharacterModel> models = new();
    public Dictionary<int, CharacterInstance> instances = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadModels(List<CharacterModel> list)
    {
        models.Clear();
        foreach (var m in list)
            models[m.id] = m;

        Debug.Log($"[CharacterManager] 모델 {list.Count}개 로드됨");
    }

    public void LoadUserInstances(List<CharacterInstance> list)
    {
        instances.Clear();
        foreach (var inst in list)
            instances[inst.id] = inst;

        Debug.Log($"[CharacterManager] 인스턴스 {list.Count}개 로드됨");
    }

    public CharacterStats GetStats(int id)
    {
        if (!models.TryGetValue(id, out var model))
        {
            Debug.LogError($"[CharacterManager] 모델 ID {id} 없음");
            return new CharacterStats();
        }

        if (!instances.TryGetValue(id, out var inst))
        {
            Debug.LogError($"[CharacterManager] 인스턴스 ID {id} 없음");
            return new CharacterStats();
        }

        // ⚠ 성급은 model.rarity만 사용
        return inst.GetStats(model);
    }

    public void AddExp(int id, int amount)
    {
        if (!instances.TryGetValue(id, out var inst))
        {
            Debug.LogError($"[CharacterManager] 인스턴스 ID {id} 없음");
            return;
        }

        inst.exp += amount;

        while (inst.exp >= RequiredExp(inst.level))
        {
            inst.exp -= RequiredExp(inst.level);
            inst.level++;
            Debug.Log($"[CharacterManager] 캐릭터 {id} 레벨업! Lv.{inst.level}");
        }
    }

    public void GiveCharacter(int id)
    {
        if (!models.ContainsKey(id))
        {
            Debug.LogError($"[CharacterManager] 모델 ID {id} 없음");
            return;
        }

        if (!instances.TryGetValue(id, out var inst))
        {

            inst = new CharacterInstance
            {
                id = id,
                isOwned = true,
                level = 1,
                exp = 0,
                shard = 0
            };

            instances[id] = inst;
            Debug.Log($"[CharacterManager] 신규 캐릭터 생성 & 획득 ID: {id}");
            return;
        }

        if (!inst.isOwned)
        {
            inst.isOwned = true;
            inst.level = 1;
            inst.exp = 0;
            inst.shard = 0;

            Debug.Log($"[CharacterManager] 캐릭터 획득! ID: {id}");
        }
        else
        {
            // 중복 캐릭터 → 조각 변환
            inst.shard += 10;
            Debug.Log($"[CharacterManager] 중복 캐릭터! 조각 +10 (현재: {inst.shard})");
        }
    }


    private int RequiredExp(int level)
    {
        return level * 5;
    }
}
