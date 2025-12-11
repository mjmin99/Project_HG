using UnityEngine;
using System.Collections;

public class EnemyWaveSystem : MonoBehaviour
{
    public static EnemyWaveSystem Instance;

    private void Awake()
    {
        Instance = this;
    }

    [Header("Wave Settings")]
    public WaveInfo[] waves;          // Inspector에서 Wave 구성
    public Transform spawnPoint;      // 적이 스폰될 위치

    public bool IsAllWavesCleared { get; private set; } = false;

    private int currentWave = 0;

    private void Start()
    {
        StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
    {
        yield return new WaitForSeconds(1f); // 전투 준비 시간 (선택)

        while (currentWave < waves.Length)
        {
            Debug.Log($"[웨이브 시작] Wave {currentWave + 1}");
            yield return StartCoroutine(PlayWave(waves[currentWave]));
            currentWave++;
        }

        // 모든 웨이브 종료
        IsAllWavesCleared = true;
        Debug.Log("[전투 종료] 모든 웨이브 클리어!");

        // Enemy가 모두 죽었으면 즉시 승리 판정
        BattleResultManager.Instance.CheckVictory();
    }

    private IEnumerator PlayWave(WaveInfo wave)
    {
        // 웨이브 내부 적 스폰
        foreach (var enemyInfo in wave.enemies)
        {
            for (int i = 0; i < enemyInfo.count; i++)
            {
                SpawnEnemy(enemyInfo.enemyPrefab);
                yield return new WaitForSeconds(enemyInfo.spawnInterval);
            }
        }

        // 웨이브 적이 모두 죽을 때까지 대기
        while (EnemyManager.Instance.GetAllEnemies().Count > 0)
            yield return null;

        Debug.Log($"[웨이브 완료] Wave {currentWave + 1}");

        // 다음 웨이브까지 대기 시간
        yield return new WaitForSeconds(wave.delayAfterWave);
    }

    private void SpawnEnemy(GameObject prefab)
    {
        Instantiate(prefab, spawnPoint.position, Quaternion.identity);
    }
}
