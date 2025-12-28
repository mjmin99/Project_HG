using System;
using UnityEngine;

// 스테이지 진행 로직 전담
// - 클리어 기록 갱신
// - 최고 기록 비교
// - 스테이지 입장 가능 여부 판단

public class StageSaveService
{
    private StageProgressData data;

    // TODO: 배틀 씬 테스트 전용
    public void Init(bool isTest = false)
    {
        if (isTest)
        {
            var stages = Resources.LoadAll<StageDataSO>($"Stage/");
            SaveData d = new SaveData(stages);
            data = d.stageProgress;
            return;
        }
        data = Manager.Save.CurrentData.stageProgress;
    }
    // 해당 스테이지의 기록을 가져오거나 없으면 생성
    public StageRecord GetStageRecord(int world, int level)
    {
        var key = StageKeyUtil.ToKey(world, level);
        if (!data.records.TryGetValue(key,out var r))
        {
            Debug.LogWarning($"해당 데이터가 없음:  {key}");
        }
        return r;
    }

    // 해당 스테이지가 이미 클리어되었는지 확인
    public bool IsCleared(int world, int stage)
    {
        var key = StageKeyUtil.ToKey(world, stage);
        return data.records.TryGetValue(key, out var r) && r.cleared;
    }

    // 전투 결과를 반영하여 클리어 기록 갱신
    public void ApplyClearResult(int world, int stage, long clearTimeMs, int score, int stars = 0)
    {
        var r = GetStageRecord(world, stage);

        r.cleared = true;
        r.clearCount += 1;
        r.lastClearedAtUtc = DateTime.UtcNow.ToString("o");

        if (r.bestClearTimeMs == 0 || clearTimeMs < r.bestClearTimeMs) r.bestClearTimeMs = clearTimeMs;
        if (score > r.bestScore) r.bestScore = score;
        if (stars > r.bestStars) r.bestStars = stars;
    }
    
    // 오버로딩
    public void ApplyClearResult(StageRecord record, long clearTime, int score, int stars = 0)
    {
        record.cleared = true;
        record.clearCount += 1;
        record.lastClearedAtUtc = DateTime.UtcNow.ToString("o");
        
        if(record.bestClearTimeMs == 0 || clearTime <  record.bestClearTimeMs) record.bestClearTimeMs = clearTime;
        if (score > record.bestScore) record.bestScore = score;
        if (stars > record.bestStars) record.bestStars = stars;
        
    }

    // 스테이지 입장 가능 여부 판단
    public bool CanEnter(int world, int stage)
    {
        // 이미 클리어한 스테이지는 항상 입장 가능
        if (IsCleared(world, stage)) return true;

        // 직전 스테이지가 클리어면 입장 가능(선형 진행 기본 규칙)
        var curRecord = GetStageRecord(world, stage);
        var prevRecord = GetStageRecord(curRecord.prevWorld, curRecord.prevLevel);

        // 1-1이거나, 이전 스테이지 클리어 한 경우
        return curRecord.prevWorld == 0 || IsCleared(prevRecord.world, prevRecord.level);
    }

    // 오버로딩. TODO: 무엇을 쓸 지는 UI 짜면서 정하기
    public bool CanEnter(StageRecord record)
        => record.prevWorld == 0 || GetStageRecord(record.prevWorld, record.prevLevel).cleared;
}
