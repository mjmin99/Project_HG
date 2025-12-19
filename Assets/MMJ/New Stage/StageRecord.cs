using System;
using System.Collections.Generic;
using UnityEngine;

// 개별 스테이지 클리어 기록을 Firebase에 그대로 JSON으로 저장 하기 위해
[Serializable]
public record StageRecord
{
    public int world;               // 해당 레코드의 스테이지 이름
    public int level;
    public int prevWorld;           // 이전 스테이지 정보
    public int prevLevel;
    public bool cleared;            // 클리어 여부
    public long bestClearTimeMs;    // 베스트 클리어 시간 ms 단위
    public int bestScore;           // 최고 점수
    public int bestStars;           // 필요없지만 일단 냅둬
    public int clearCount;          // 클리어 횟수
    public string lastClearedAtUtc; // 필요없지만 일단 냅둬

    public StageRecord(StageDataSO data)
    {
        world = data.world;
        level = data.stage;
        prevWorld = data.prevWorld;
        prevLevel = data.prevStage;
        cleared = false;
        bestClearTimeMs = 0;
        bestScore = 0;
        bestStars = 0;
        clearCount = 0;
        lastClearedAtUtc = "";
    }
}

[Serializable]
public class StageProgressData
{
    // key: "W01-S005"
    // Value: 해당 스테이지의 클리어 기록
    public Dictionary<string,StageRecord> records = new();

    public StageProgressData(StageDataSO[] stages)
    {
        foreach (var d in stages)
        {
            StageRecord record = new(d);
            records.Add(StageKeyUtil.ToKey(d.world,d.stage),record);
        }
    }
}