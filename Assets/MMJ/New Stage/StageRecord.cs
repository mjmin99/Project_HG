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
    // 저장용 (JsonUtility가 직렬화 가능)
    public List<StageRecord> records;

    // 런타임 캐시 (저장 안 됨)
    [NonSerialized]
    private Dictionary<string, StageRecord> cache;

    public Dictionary<string, StageRecord> Cache
    {
        get
        {
            if (cache == null)
            {
                Debug.Log("Getter으로 들어옴");
                RebuildCache();
            }
            return cache;
        }
    }

    // 신규 세이브 생성 시: 모든 스테이지에 대한 기본 레코드 생성
    public StageProgressData(StageDataSO[] stages)
    {
        Debug.Log("신규 스테이지 프로그레스 생성");
        records = new List<StageRecord>();
        records.Clear();
        foreach (var d in stages)
        {
            records.Add(new StageRecord(d));
        }
    }

    // Firebase 로드 후 보정용
    public void RebuildCache()
    {
        Debug.Log("리빌드 캐시");
        // 방어
        if (records == null|| records.Count == 0)
        {
            Debug.LogWarning("레코드가 없음");
            records = new List<StageRecord>();
            var stages = Resources.LoadAll<StageDataSO>($"Stage/StageDataSO");
            records.Clear();
            foreach (var d in stages)
            {
                records.Add(new StageRecord(d));
            }
            return;
        }

        if (cache != null) return;
        
        cache = new Dictionary<string, StageRecord>();

        foreach (var r in records)
        {
            var key = StageKeyUtil.ToKey(r.world, r.level);
            if(!cache.TryAdd(key,r)) Debug.LogError($"이미 있는 키임{key}");
        }
    }
}