using UnityEngine;

[System.Serializable]
public class WaveInfo
{
    public EnemySpawnData[] enemies; // 웨이브 안에 여러 적 종류
    public float delayAfterWave = 1f; // 웨이브 종료 후 다음 웨이브까지 대기
}

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;  // 어떤 적
    public int count = 1;           // 몇 마리
    public float spawnInterval = 0.5f; // 적들 사이 간격
}