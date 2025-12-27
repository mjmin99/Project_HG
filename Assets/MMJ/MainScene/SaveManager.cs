using System;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SaveManager : Singleton<SaveManager>
{
    public SaveData CurrentData { get; private set; }

    private DatabaseReference db;

    // 유저 세이브 시 사용되는 함수
    public void SaveCurrentUser()
    {
        var user = FirebaseManager.Auth.CurrentUser;
        if (user != null)
            SaveToFirebase(user.UserId);
        else
            Debug.LogWarning("[SaveManager] 로그인된 유저 없음!");
    }

    protected override void Awake()
    {
        base.Awake();
        db = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void InitForUser(string userId, System.Action onComplete)
    {
        LoadFromFirebase(userId, onComplete);
    }

    private void LoadFromFirebase(string userId, System.Action onComplete)
    {
        Debug.Log($"[SaveManager] Firebase 로드 시작: users/{userId}/saveData");

        db.Child("users").Child(userId).Child("saveData")
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogWarning($"[SaveManager] 로드 실패: {task.Exception}");
                    Debug.Log("[SaveManager] → 신규 유저로 판단, 기본 데이터 생성");
                    CurrentData = CreateDefaultSaveData();
                    SaveToFirebase(userId);
                    ApplyToCharacterManager();
                    onComplete?.Invoke();
                    return;
                }

                DataSnapshot snap = task.Result;

                if (snap.Exists && snap.Value != null)
                {
                    string json = snap.GetRawJsonValue();
                    CurrentData = JsonUtility.FromJson<SaveData>(json);
                    Debug.Log("[SaveManager] 기존 세이브 로드 성공");
                }
                else
                {
                    Debug.Log("[SaveManager] Firebase에 데이터 없음");
                    CurrentData = CreateDefaultSaveData();
                    SaveToFirebase(userId);
                }

                if (CurrentData.characters == null)
                {
                    Debug.LogWarning("[SaveManager] characters 리스트가 null → 초기화");
                    CurrentData.characters = new List<CharacterInstance>();
                }

                if (CurrentData.characters.Count == 0)
                {
                    Debug.LogWarning("[SaveManager] 캐릭터 없음 → 기본 캐릭터 지급");
                    CurrentData = CreateDefaultSaveData();
                    SaveToFirebase(userId);
                }

                ApplyToCharacterManager();
                onComplete?.Invoke();
            });
    }

    private void SaveToFirebase(string userId)
    {
        SyncFromCharacterManager();

        string json = JsonUtility.ToJson(CurrentData);

        db.Child("users").Child(userId).Child("saveData")
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                    Debug.LogError($"[SaveManager] 세이브 저장 실패: {task.Exception}");
                else
                    Debug.Log("[SaveManager] Firebase에 세이브 저장 성공!");
            });
    }

    public bool TrySpendGold(int amount)
    {
        if (CurrentData.gold < amount)
            return false;

        CurrentData.gold -= amount;
        return true;
    }

    public void AddGold(int amount)
    {
        CurrentData.gold += amount;
    }

    public bool TrySpendGem(int amount)
    {
        if (CurrentData.gem < amount)
            return false;

        CurrentData.gem -= amount;
        return true;
    }

    public void AddGem(int amount)
    {
        CurrentData.gem += amount;
    }

    private SaveData CreateDefaultSaveData()
    {
        var stages = Resources.LoadAll<StageDataSO>($"Stage/");
        SaveData data = new SaveData(stages);

        if (Manager.Character.models.Count == 0)
        {
            Debug.LogError("[SaveManager] CharacterManager.models가 비어있음! CSV를 먼저 로드해야 합니다!");
            return data;
        }

        // ID 순서로 정렬해서 생성
        foreach (var pair in Manager.Character.models.OrderBy(x => x.Key))
        {
            var model = pair.Value;

            var inst = new CharacterInstance
            {
                id = model.id,
                isOwned = (model.id == 0),
                level = 1,
                exp = 0,
                shard = 0,
                skillType = model.role switch
                {
                    CharacterRole.Dealer => SkillType.StrongAttack,
                    CharacterRole.Tank => SkillType.Parrying,
                    CharacterRole.Healer => SkillType.AllHeal,
                    _ => throw new ArgumentOutOfRangeException()
                }
            };
            
            data.characters.Add(inst);
        }

        data.gold = 1000;
        data.gem = 100;

        Debug.Log($"[SaveManager] 기본 세이브 생성: 캐릭터 {data.characters.Count}개, 골드 {data.gold}, 젬 {data.gem}");
        return data;
    }

    private void ApplyToCharacterManager()
    {
        Manager.Character.LoadUserInstances(CurrentData.characters);
        Debug.Log($"[SaveManager] CharacterManager에 {CurrentData.characters.Count}개 캐릭터 적용");
    }

    private void SyncFromCharacterManager()
    {
        CurrentData.characters.Clear();

        // ID 순서로 정렬해서 동기화
        var sortedInstances = Manager.Character.instances
            .OrderBy(x => x.Key)
            .Select(x => x.Value);

        foreach (var inst in sortedInstances)
            CurrentData.characters.Add(inst);

        Debug.Log($"[SaveManager] ID 순으로 {CurrentData.characters.Count}개 캐릭터 동기화");
    }
}