using System;
using System.Collections.Generic;

// 개별 스테이지 클리어 기록을 Firebase에 그대로 JSON으로 저장 하기 위해
[Serializable]
public class StageClearRecord
{
    public bool cleared;            // 클리어 여부
    public long bestClearTimeMs;    // 베스트 클리어 시간 ms 단위
    public int bestScore;           // 최고 점수
    public int bestStars;           // 필요없지만 일단 냅둬
    public int clearCount;          // 클리어 횟수
    public string lastClearedAtUtc; // 필요없지만 일단 냅둬
}

[Serializable]
public class StageProgressData
{
    // key: "W01-S005"
    // Value: 해당 스테이지의 클리어 기록
    public Dictionary<string, StageClearRecord> records = new Dictionary<string, StageClearRecord>();
}
