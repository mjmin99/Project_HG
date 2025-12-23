using System;
using System.Collections.Generic;
using UnityEngine;

// 전투 시스템은 StageDataSO를 읽기 전용으로 참조

// SO 기반 개별 스테이지 데이터
// 런타임 읽기 전용
[CreateAssetMenu(menuName = "Stage/Stage Data", fileName = "StageData_")]
public class StageDataSO : ScriptableObject
{
    [Header("Set Map Prefab Path")]
    public string mapPrefabPath;

    [Header("Set Stage Values")]
    public int world;
    public int stage;
    public int prevWorld;
    public int prevStage;

    [Header("Rewards")]
    public int rewardGold;

    [Header("Waves")]
    public List<StageWave> waves = new List<StageWave>();
}

// 스테이지 내 웨이브 정보
[Serializable]
public class StageWave
{
    // 웨이브 시작 전 대기(= 웨이브 텀)
    public float waitBeforeWave = 1f; 

    // 이 웨이브에서 스폰되는 적들
    public List<StageSpawn> spawns = new List<StageSpawn>();
}

[Serializable]
public class StageSpawn
{
    public int id;     // 전투진행시 ID->Prefab 매핑
    public int count = 1;
    public float spawnInterval = 0.5f;
}
