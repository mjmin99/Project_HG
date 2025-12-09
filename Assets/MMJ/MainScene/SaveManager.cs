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
        else Destroy(gameObject);
    }

    // 로그인 후 호출
    public void InitForUser(string userId, System.Action onComplete)
    {
        LoadFromFirebase(userId, onComplete);
    }

    // Firebase 로드
    public void LoadFromFirebase(string userId, System.Action onComplete)
    {
        db.Child("users").Child(userId).Child("saveData")
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogWarning("로드 실패 → 신규 데이터 생성");

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
                }
                else
                {
                    Debug.Log("기존 세이브 없음 → 기본값 생성");
                    CurrentData = CreateDefaultSaveData();
                    SaveToFirebase(userId);
                }

                // 리스트 보정
                if (CurrentData.characters == null)
                    CurrentData.characters = new List<CharacterInstance>();

                // 캐릭터 데이터 없으면 기본 지급
                if (CurrentData.characters.Count == 0)
                {
                    CurrentData = CreateDefaultSaveData();
                    SaveToFirebase(userId);
                }

                ApplyToCharacterManager();
                onComplete?.Invoke();
            });
    }

    // Firebase 저장
    public void SaveToFirebase(string userId)
    {
        SyncFromCharacterManager();

        string json = JsonUtility.ToJson(CurrentData);

        db.Child("users").Child(userId).Child("saveData")
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                    Debug.LogError("세이브 저장 실패");
                else
                    Debug.Log("세이브 저장 성공!");
            });
    }

    // 편의 저장 함수
    public void SaveCurrentUser()
    {
        var user = FirebaseManager.Auth.CurrentUser;
        if (user != null)
            SaveToFirebase(user.UserId);
    }

    // 기본 세이브 생성 (0번 캐릭터 지급)
    private SaveData CreateDefaultSaveData()
    {
        SaveData data = new SaveData();

        foreach (var pair in CharacterManager.Instance.models)
        {
            var model = pair.Value;

            CharacterInstance inst = new CharacterInstance
            {
                id = model.id,
                isOwned = (model.id == 0),
                level = model.id == 0 ? 1 : 0,
                star = model.id == 0 ? 1 : 0,
                exp = 0,
                shard = 0
            };

            data.characters.Add(inst);
        }

        return data;
    }

    // SaveData → CharacterManager
    private void ApplyToCharacterManager()
    {
        CharacterManager.Instance.LoadUserInstances(CurrentData.characters);
    }

    // CharacterManager → SaveData
    private void SyncFromCharacterManager()
    {
        CurrentData.characters.Clear();

        foreach (var inst in CharacterManager.Instance.instances.Values)
            CurrentData.characters.Add(inst);
    }
}
