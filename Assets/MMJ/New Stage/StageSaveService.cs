using System;
using UnityEngine;

public class StageSaveService : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private StageDatabaseSO stageDatabase;

    private StageProgressData data;

    #region Init

    private bool EnsureInitialized()
    {
        if (data != null)
            return true;

        if (Manager.Save == null || Manager.Save.CurrentData == null)
        {
            Debug.LogWarning("[StageSaveService] SaveData not ready");
            return false;
        }

        data = Manager.Save.CurrentData.stageProgress;

        if (data == null)
        {
            Debug.LogError("[StageSaveService] stageProgress is NULL");
            return false;
        }

        data.RebuildCache();
        return true;
    }

    #endregion

    #region Query

    public bool IsCleared(int world, int stage)
    {
        if (!EnsureInitialized())
            return false;

        var key = StageKeyUtil.ToKey(world, stage);
        return data.Cache.TryGetValue(key, out var r) && r.cleared;
    }

    public bool CanEnter(int world, int stage)
    {
        // 시작 스테이지 예외
        if (world == 1 && stage == 1)
            return true;

        if (!EnsureInitialized())
            return false;

        // 이미 클리어
        if (IsCleared(world, stage))
            return true;

        // 이전 스테이지 기준
        int prevWorld = world;
        int prevStage = stage - 1;

        if (prevStage <= 0)
        {
            prevWorld -= 1;
            prevStage = 5; // 월드당 스테이지 수
        }

        if (prevWorld <= 0)
            return false;

        return IsCleared(prevWorld, prevStage);
    }

    public StageRecord GetStageRecord(int world, int stage)
    {
        if (!EnsureInitialized())
            return null;

        data.Cache.TryGetValue(StageKeyUtil.ToKey(world, stage), out var r);
        return r;
    }

    #endregion

    #region Apply

    public void ApplyClearResult(
        int world,
        int stage,
        long clearTimeMs,
        int score,
        int stars = 0)
    {
        if (!EnsureInitialized())
            return;

        var key = StageKeyUtil.ToKey(world, stage);

        // 🔥 없으면 생성
        if (!data.Cache.TryGetValue(key, out var r))
        {
            if (!stageDatabase.TryGet(world, stage, out var stageData))
            {
                Debug.LogError($"[StageSaveService] StageDataSO not found: {key}");
                return;
            }

            r = new StageRecord(stageData);
            data.records.Add(r);
            data.Cache[key] = r;
        }

        r.cleared = true;
        r.clearCount += 1;
        r.lastClearedAtUtc = DateTime.UtcNow.ToString("o");

        if (r.bestClearTimeMs == 0 || clearTimeMs < r.bestClearTimeMs)
            r.bestClearTimeMs = clearTimeMs;

        if (score > r.bestScore)
            r.bestScore = score;

        if (stars > r.bestStars)
            r.bestStars = stars;

        Debug.Log($"[StageSaveService] Cleared {key}");
    }

    #endregion
}
