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
            // 신규 캐릭터 생성
            inst = new CharacterInstance
            {
                id = id,
                isOwned = true,
                level = 1,
                exp = 0,
                shard = 0
            };

            instances[id] = inst;
            Debug.Log($"[CharacterManager] 신규 캐릭터 획득! ID: {id}");
            return;
        }

        if (!inst.isOwned)
        {
            // 미소유 → 소유 전환
            inst.isOwned = true;
            inst.level = 1;
            inst.exp = 0;
            inst.shard = 0;

            Debug.Log($"[CharacterManager] 캐릭터 획득! ID: {id}");
        }
        else
        {
            // 중복 → 조각 지급
            inst.shard += 10;
            Debug.Log($"[CharacterManager] 중복 캐릭터! 조각 +10 (현재: {inst.shard})");
        }
    }

    private int RequiredExp(int level)
    {
        return level * 5;
    }

    // 장착과 해제용으로 만들었는데 더이상 안쓰게 됨
    //public bool CanEquipAbility(int characterId, AbilityInstance ability)
    //{
    //    if (!models.TryGetValue(characterId, out var model))
    //        return false;
    //
    //    if (!instances.TryGetValue(characterId, out var inst))
    //        return false;
    //
    //    // 슬롯 수 초과 체크
    //    int unlockedSlots = inst.GetUnlockedAbilitySlotCount(model);
    //
    //    if (inst.abilities.Count >= unlockedSlots)
    //        return false;
    //
    //    // 중복 장착 방지 (같은 AbilityId)
    //    if (inst.abilities.Exists(a => a.abilityId == ability.abilityId))
    //        return false;
    //
    //    return true;
    //}
    // public bool EquipAbility(int characterId, AbilityInstance ability)
    // {
    //     if (!CanEquipAbility(characterId, ability))
    //         return false;
    // 
    //     instances[characterId].abilities.Add(ability);
    //     return true;
    // }
    // 
    // public bool UnequipAbility(int characterId, int abilityId)
    // {
    //     if (!instances.TryGetValue(characterId, out var inst))
    //         return false;
    // 
    //     int removed = inst.abilities.RemoveAll(a => a.abilityId == abilityId);
    //     return removed > 0;
    // }

    public bool TryRerollAbilities(int characterId)
    {
        if (!instances.TryGetValue(characterId, out var inst))
            return false;

        if (!models.TryGetValue(characterId, out var model))
            return false;

        inst.SyncAbilitySlots(model); // 슬롯 수 보정(레벨 기반)

        // 어빌리티 재설정에 드는 비용 로직
        int lockedCount = CountLockedSlots(inst);
        int cost = 10 + lockedCount * 10;

        // 만약 골드로 하고 싶다면 아래
        if (!SaveManager.Instance.TrySpendGold(cost))
            return false;

        // 만약 캐릭터 중복 뽑기 재화로 돌리고 싶다면 아래-> ui는 뭐 상관 없음 ㅋ 호환 가능
        // if (inst.shard < cost)
        //    return false;
        //
        // inst.shard -= cost;

        var pool = AbilityDatabase.GetPoolFor(model);

        foreach (var slot in inst.abilitySlots)
        {
            if (slot.isLocked)
                continue;

            if (pool.Count == 0)
            {
                slot.ability = null;
                continue;
            }

            int index = Random.Range(0, pool.Count);
            slot.ability = pool[index];
        }

        SaveManager.Instance.SaveCurrentUser();
        return true;
    }

    private AbilityInstance GetRandomAbility(List<AbilityInstance> pool)
    {
        int index = Random.Range(0, pool.Count);
        return pool[index];
    }

    private int CountLockedSlots(CharacterInstance inst)
    {
        int count = 0;

        foreach (var slot in inst.abilitySlots)
        {
            if (slot.isLocked)
                count++;
        }

        return count;
    }
}