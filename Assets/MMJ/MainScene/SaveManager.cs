using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public SaveData CurrentData { get; private set; }

    private DatabaseReference db;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            db = FirebaseDatabase.DefaultInstance.RootReference;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitForUser(string userId, System.Action onComplete)
    {
        LoadFromFirebase(userId, onComplete);
    }

    public void LoadFromFirebase(string userId, System.Action onComplete)
    {
        db.Child("users").Child(userId).Child("saveData")
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogWarning($"[SaveManager] 로드 실패: {task.Exception} → 신규 데이터 생성");
                    CurrentData = CreateDefaultSaveData();
                    SaveToFirebase(userId);
                    ApplyToCharacterManager();
                    onComplete?.Invoke();
                    return;
                }

                DataSnapshot snap = task.Result;

                if (snap.Exists)
                {
                    string json = snap.GetRawJsonValue();
                    CurrentData = JsonUtility.FromJson<SaveData>(json);
                    Debug.Log("[SaveManager] 기존 세이브 로드 성공");
                }
                else
                {
                    Debug.Log("[SaveManager] 기존 세이브 없음 → 기본값 생성");
                    CurrentData = CreateDefaultSaveData();
                    SaveToFirebase(userId);
                }

                if (CurrentData.characters == null)
                    CurrentData.characters = new List<CharacterInstance>();

                if (CurrentData.characters.Count == 0)
                {
                    Debug.Log("[SaveManager] 캐릭터 없음 → 기본 캐릭터 지급");
                    CurrentData = CreateDefaultSaveData();
                    SaveToFirebase(userId);
                }

                ApplyToCharacterManager();
                onComplete?.Invoke();
            });
    }

    public void SaveToFirebase(string userId)
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
                    Debug.Log("[SaveManager] 세이브 저장 성공!");
            });
    }

    public void SaveCurrentUser()
    {
        var user = FirebaseManager.Auth.CurrentUser;
        if (user != null)
            SaveToFirebase(user.UserId);
        else
            Debug.LogWarning("[SaveManager] 로그인된 유저 없음!");
    }

    // 재화 관리 함수
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
        SaveData data = new SaveData();

        if (CharacterManager.Instance.models.Count == 0)
        {
            Debug.LogError("[SaveManager] CharacterManager.models가 비어있음! CSV를 먼저 로드해야 합니다!");
            return data;
        }

        foreach (var pair in CharacterManager.Instance.models)
        {
            var model = pair.Value;

            CharacterInstance inst = new CharacterInstance
            {
                id = model.id,
                isOwned = (model.id == 0),
                level = model.id == 0 ? 1 : 0,
                exp = 0,
                shard = 0
            };

            data.characters.Add(inst);
        }

        data.gold = 1000; // 초기재화
        data.gem = 100;

        Debug.Log($"[SaveManager] 기본 세이브 생성: 캐릭터 {data.characters.Count}개");
        return data;
    }

    private void ApplyToCharacterManager()
    {
        CharacterManager.Instance.LoadUserInstances(CurrentData.characters);
        Debug.Log($"[SaveManager] CharacterManager에 {CurrentData.characters.Count}개 캐릭터 적용");
    }

    private void SyncFromCharacterManager()
    {
        CurrentData.characters.Clear();

        foreach (var inst in CharacterManager.Instance.instances.Values)
            CurrentData.characters.Add(inst);
    }
}