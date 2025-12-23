//using System;
//using UnityEngine;

//// 스테이지 진행 로직 전담
//// - 클리어 기록 갱신
//// - 최고 기록 비교
//// - 스테이지 입장 가능 여부 판단

//public class StageProgressService
//{
//    private readonly StageProgressData data;
//    private readonly int stagesPerWorld;

//    public StageProgressService(StageProgressData data, int stagesPerWorld = 5)
//    {
//        this.data = data ?? new StageProgressData();
//        this.stagesPerWorld = Mathf.Max(1, stagesPerWorld);
//    }

//    // 해당 스테이지의 기록을 가져오거나 없으면 생성
//    public StageClearRecord GetOrCreate(int world, int stage)
//    {
//        var key = StageKeyUtil.ToKey(world, stage);
//        if (!data.records.TryGetValue(key, out var r))
//        {
//            r = new StageClearRecord();
//            data.records[key] = r;
//        }
//        return r;
//    }

//    // 해당 스테이지가 이미 클리어되었는지 확인
//    public bool IsCleared(int world, int stage)
//    {
//        var key = StageKeyUtil.ToKey(world, stage);
//        return data.records.TryGetValue(key, out var r) && r.cleared;
//    }

//    // 전투 결과를 반영하여 클리어 기록 갱신
//    public void ApplyClearResult(int world, int stage, long clearTimeMs, int score, int stars = 0)
//    {
//        var r = GetOrCreate(world, stage);

//        r.cleared = true;
//        r.clearCount += 1;
//        r.lastClearedAtUtc = DateTime.UtcNow.ToString("o");

//        if (r.bestClearTimeMs == 0 || clearTimeMs < r.bestClearTimeMs) r.bestClearTimeMs = clearTimeMs;
//        if (score > r.bestScore) r.bestScore = score;
//        if (stars > r.bestStars) r.bestStars = stars;
//    }

//    // 스테이지 입장 가능 여부 판단
//    public bool CanEnter(int world, int stage)
//    {
//        // 이미 클리어한 스테이지는 항상 입장 가능
//        if (IsCleared(world, stage)) return true;

//        // 직전 스테이지가 클리어면 입장 가능(선형 진행 기본 규칙)
//        var prev = GetPreviousStage(world, stage);

//        // (1-1) 같은 시작점 처리
//        if (prev.world == 0)
//            return true;

//        return IsCleared(prev.world, prev.stage);
//    }

//    // 이전 스테이지 계산
//    private (int world, int stage) GetPreviousStage(int world, int stage)
//    {
//        if (world <= 1 && stage <= 1)
//            return (0, 0);

//        if (stage > 1)
//            return (world, stage - 1);

//        return (world - 1, stagesPerWorld);
//    }
//}
