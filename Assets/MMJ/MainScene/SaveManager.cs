using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;

public partial class SaveManager : Singleton<SaveManager>
{
    private DatabaseReference db;
    private string userIdCached;
    public SaveData CurrentData { get; private set; }

    // patch 배치용
    private readonly Dictionary<string, object> pending = new();
    private bool flushScheduled;

    public static event Action<CharacterInstance> OnCharacterAcquired;

    protected override void Awake()
    {
        base.Awake();
        db = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private DatabaseReference UserSaveRef =>
        db.Child("users").Child(userIdCached).Child("saveData");

    public void SetUserContext(string userId)
    {
        userIdCached = userId;
    }

    // === 핵심: 변경 경로만 누적 ===
    public void EnqueuePatch(string path, object value)
    {
        pending[path] = value;
        ScheduleFlush();
    }

    public void EnqueuePatch(IDictionary<string, object> updates)
    {
        foreach (var kv in updates)
            pending[kv.Key] = kv.Value;

        ScheduleFlush();
    }

    private void ScheduleFlush()
    {
        if (flushScheduled) return;
        flushScheduled = true;
        FlushLater().Forget();
    }

    private async UniTaskVoid FlushLater()
    {
        // 연타 저장(강화/가챠 등) 묶기
        await UniTask.Delay(TimeSpan.FromMilliseconds(200));
        flushScheduled = false;

        if (pending.Count == 0) return;

        var copy = new Dictionary<string, object>(pending);
        pending.Clear();

        _ = UserSaveRef.UpdateChildrenAsync(copy)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                    Debug.LogError($"[SaveManager] Patch save failed: {task.Exception}");
                else
                    Debug.Log($"[SaveManager] Patch saved ({copy.Count} fields)");
            });
    }
    public void PatchGold()
    {
        if (CurrentData == null) return;
        EnqueuePatch("gold", CurrentData.gold);
    }

    public void PatchGem()
    {
        if (CurrentData == null) return;
        EnqueuePatch("gem", CurrentData.gem);
    }

    public void PatchPartySet()
    {
        if (CurrentData == null || CurrentData.partySet == null || CurrentData.partySet.Length < 3)
            return;

        var p = CurrentData.partySet;
        EnqueuePatch(new Dictionary<string, object>
        {
            ["partySet/0"] = p[0],
            ["partySet/1"] = p[1],
            ["partySet/2"] = p[2],
        });
    }

    public void PatchCharacter(int characterId)
    {
        if (Manager.Character == null) return;
        if (!Manager.Character.instances.TryGetValue(characterId, out var inst)) return;

        string root = $"characters/{characterId}";

        var updates = new Dictionary<string, object>
        {
            [$"{root}/isOwned"] = inst.isOwned,
            [$"{root}/level"] = inst.level,
            [$"{root}/exp"] = inst.exp,
            [$"{root}/shard"] = inst.shard,
        };

        // 선택: skillType도 세이브에 포함할 거면
        // updates[$"{root}/skillType"] = (int)inst.skillType;

        EnqueuePatch(updates);
    }

    public void PatchStageRecord(string stageKey, StageRecord r)
    {
        if (r == null) return;
        string root = $"stageProgress/{stageKey}";

        EnqueuePatch(new Dictionary<string, object>
        {
            [$"{root}/cleared"] = r.cleared,
            [$"{root}/clearCount"] = r.clearCount,
            [$"{root}/bestClearTimeMs"] = r.bestClearTimeMs,
            [$"{root}/bestScore"] = r.bestScore,
            [$"{root}/bestStars"] = r.bestStars,
            [$"{root}/lastClearedAtUtc"] = r.lastClearedAtUtc,
        });
    }
    public void PatchDialogFlag(DialogCondition condition, bool value = true)
    {
        EnqueuePatch($"dialogFlags/{(int)condition}", value);
    }

    public static void RaiseCharacterAcquired(CharacterInstance inst)
    {
        OnCharacterAcquired?.Invoke(inst);
    }
    public bool TrySpendGold(int amount)
    {
        if (CurrentData.gold < amount)
            return false;

        CurrentData.gold -= amount;
        return true;
    }
    public void InitForUser(string userId, Action onComplete)
    {
        LoadFromFirebase(userId, onComplete);
    }

    private void LoadFromFirebase(string userId, Action onComplete)
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

    private void ApplyToCharacterManager()
    {
        Manager.Character.LoadUserInstances(CurrentData.characters);
        Debug.Log($"[SaveManager] CharacterManager에 {CurrentData.characters.Count}개 캐릭터 적용");
    }


    private SaveData CreateDefaultSaveData()
    {
        var stages = Resources.LoadAll<StageDataSO>($"Stage/StageDataSO");
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

        data.gold = 500;
        data.gem = 0;

        Debug.Log($"[SaveManager] 기본 세이브 생성: 캐릭터 {data.characters.Count}개, 골드 {data.gold}, 젬 {data.gem}");
        return data;
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
