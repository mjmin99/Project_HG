using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    // 정적 캐릭터 모델 (CSV)
    public Dictionary<int, CharacterModel> models = new();

    // 유저의 캐릭터 인스턴스
    public Dictionary<int, CharacterInstance> instances = new();

    public SaveData SaveData { get; private set; }



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

    public void LoadFromSaveData(SaveData data)
    {
        SaveData = data;
        instances.Clear();
        foreach (var inst in data.characters)
            instances[inst.id] = inst;
    }

    public void SaveToSaveData()
    {
        SaveData.characters.Clear();
        foreach (var inst in instances.Values)
            SaveData.characters.Add(inst);
    }

    // CSV 로드 후 모델 저장
    public void LoadModels(List<CharacterModel> list)
    {
        models.Clear();
        foreach (var m in list)
            models[m.id] = m;
    }

    // Firebase → CharacterManager 로 유저 캐릭터 로드
    public void LoadUserInstances(List<CharacterInstance> list)
    {
        instances.Clear();
        foreach (var inst in list)
            instances[inst.id] = inst;
    }

    // 최종 스탯 계산
    public CharacterStats GetStats(int id)
    {
        return instances[id].GetStats(models[id]);
    }

    // 경험치 추가
    public void AddExp(int id, int amount)
    {
        var inst = instances[id];
        inst.exp += amount;

        while (inst.exp >= RequiredExp(inst.level))
        {
            inst.exp -= RequiredExp(inst.level);
            inst.level++;
        }
    }

    private int RequiredExp(int level) => level * 5;
}
