using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class CharacterManager : Singleton<CharacterManager>
{
    public Dictionary<int, CharacterModel> models = new();
    public Dictionary<int, CharacterInstance> instances = new();


    // public void LoadModels() // 테스트용 함수. 테스트 끝난 후 지우기
    // {
    //     var characterModels = CharacterCSVLoader.Load();
    //     LoadModels(characterModels);
    //     for (int i = 0; i < characterModels.Count; i++)
    //     {
    //         instances[i] = new CharacterInstance();
    //         switch(characterModels[i].role)
    //         {
    //             case CharacterRole.Dealer:
    //                 instances[i].skillType = SkillType.StrongAttack;
    //                 break;
    //             case CharacterRole.Tank:
    //                 instances[i].skillType = SkillType.Parrying;
    //                 break;
    //             case CharacterRole.Healer:
    //                 instances[i].skillType = SkillType.AllHeal;
    //                 break;
    //         }
    //     }
    // }
    public void LoadModels(List<CharacterModel> list)
    {
        models.Clear();
        foreach (var m in list)
        {
            models[m.id] = m;
        }

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
            SaveManager.RaiseCharacterAcquired(inst);
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
            SaveManager.RaiseCharacterAcquired(inst);
            Debug.Log($"[CharacterManager] 캐릭터 획득! ID: {id}");
        }
        else
        {
            // 중복 → 조각 지급
            inst.shard += 10;
            Debug.Log($"[CharacterManager] 중복 캐릭터! 조각 +10 (현재: {inst.shard})");
        }
    }

    public int RequiredExp(int level)
    {
        // 레벨당 필요한 강화 횟수
        int enhanceCount = level * 3;

        // 강화 1회당 EXP (골드와 1:1)
        int enhanceExp = GetEnhanceCostByLevel(level);

        return enhanceCount * enhanceExp;
    }

    public int GetEnhanceCostByLevel(int level)
    {
        const int BASE_COST = 5;
        return BASE_COST + level * 5;
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
        // if (!Manager.Save.TrySpendGold(cost))
        //   return false;

        if (inst.shard < cost)
            return false;

        inst.shard -= cost;

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

        Manager.Save.SaveCurrentUser();
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

    public bool TryEnhanceCharacter(int characterId)
    {
        if (!instances.TryGetValue(characterId, out var inst))
            return false;

        int cost = GetEnhanceCost(characterId);

        // 골드 소모
        if (!Manager.Save.TrySpendGold(cost))
            return false;

        // 경험치 = 소모 골드 (1:1)
        AddExp(characterId, cost);

        Manager.Save.SaveCurrentUser();
        return true;
    }

    public int GetEnhanceCost(int characterId)
    {
        if (!instances.TryGetValue(characterId, out var inst))
            return int.MaxValue;

        // 기본 비용
        const int BASE_COST = 5;

        // 레벨 비례 증가
        return BASE_COST + inst.level * 5;
    }
}