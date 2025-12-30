using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class StageSaveService
{
    private StageProgressData data;

    #region Init

    public bool EnsureInitialized()
    {
        // if (isTest)
        // {
        //     var stages = Resources.LoadAll<StageDataSO>($"Stage/");
        //     SaveData d = new SaveData(stages);
        //     data = d.stageProgress;
        //     return true;
        // }
        
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
        
        if (data.Cache.TryGetValue(key, out var r))
        {
            return r.cleared;
        }
        
        Manager.Save.CurrentData.RebuildStageProgress();
        
        return Manager.Save.CurrentData.stageProgress.Cache.TryGetValue(key, out var s) && s.cleared;
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
        var curRecord = GetStageRecord(world, stage);
        if (curRecord == null)
        {
            return false;
        }
        Debug.Log($"{world}_{stage}");
        var prevRecord = GetStageRecord(curRecord.prevWorld, curRecord.prevLevel);
        if (prevRecord == null)
        {
            Debug.Log($"이전 스테이지가 null{curRecord.prevWorld}_{curRecord.prevLevel}");
            return false;
        }

        return IsCleared(prevRecord.world, prevRecord.level);
    }

    public StageRecord GetStageRecord(int world, int stage)
    {
        if (!EnsureInitialized())
            return null;

        return data.Cache.GetValueOrDefault(StageKeyUtil.ToKey(world, stage));
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

        if (!data.Cache.TryGetValue(key, out var r))
        {
            Debug.LogWarning("딕셔너리에 없음");
            // if (!stageDatabase.TryGet(world, stage, out var stageData))
            // {
            //     Debug.LogError($"[StageSaveService] StageDataSO not found: {key}");
            //     return;
            // }
            //
            // r = new StageRecord(stageData);
            // data.records.Add(r);
            // data.Cache[key] = r;
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
