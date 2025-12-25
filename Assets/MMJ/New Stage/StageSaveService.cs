using System;
using System.Collections.Generic;
using UnityEngine;

public class StageSaveService : MonoBehaviour
{
    private StageProgressData data;

    private bool EnsureInitialized()
    {
        if (data != null && data.records != null)
            return true;

        if (Manager.Save == null || Manager.Save.CurrentData == null)
            return false;

        data = Manager.Save.CurrentData.stageProgress;

        if (data == null)
            return false;

        // 핵심: Dictionary 보정
        if (data.records == null)
        {
            Debug.LogWarning("[StageSaveService] records was null. Rebuilding...");
            data.records = new Dictionary<string, StageRecord>();
        }

        return true;
    }

    public StageRecord GetStageRecord(int world, int level)
    {
        if (!EnsureInitialized()) return null;

        var key = StageKeyUtil.ToKey(world, level);
        data.records.TryGetValue(key, out var r);
        return r;
    }

    public bool IsCleared(int world, int stage)
    {
        if (!EnsureInitialized()) return false;

        var key = StageKeyUtil.ToKey(world, stage);
        return data.records.TryGetValue(key, out var r) && r.cleared;
    }

    public bool CanEnter(int world, int stage)
    {
        // 시작 스테이지는 항상 오픈
        if (world == 1 && stage == 1)
            return true;

        if (!EnsureInitialized()) return false;

        // 이미 클리어한 스테이지
        if (IsCleared(world, stage)) return true;

        var curRecord = GetStageRecord(world, stage);
        if (curRecord == null) return false;

        // 1-1 같은 시작 스테이지
        if (curRecord.prevWorld == 0) return true;

        return IsCleared(curRecord.prevWorld, curRecord.prevLevel);
    }

    public void ApplyClearResult(int world, int stage, long clearTimeMs, int score, int stars = 0)
    {
        if (!EnsureInitialized()) return;

        var r = GetStageRecord(world, stage);
        if (r == null) return;

        r.cleared = true;
        r.clearCount += 1;
        r.lastClearedAtUtc = DateTime.UtcNow.ToString("o");

        if (r.bestClearTimeMs == 0 || clearTimeMs < r.bestClearTimeMs)
            r.bestClearTimeMs = clearTimeMs;

        if (score > r.bestScore)
            r.bestScore = score;

        if (stars > r.bestStars)
            r.bestStars = stars;
    }
}
