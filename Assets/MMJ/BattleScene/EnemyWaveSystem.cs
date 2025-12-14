using UnityEngine;
using System.Collections;

public class EnemyWaveSystem : MonoBehaviour
{
    public static EnemyWaveSystem Instance;

    [Header("Wave Settings")]
    public WaveInfo[] waves;          // BattleStageApplier에서 주입
    public Transform spawnPoint;      // 적이 스폰될 위치

    public bool IsAllWavesCleared { get; private set; } = false;

    private int currentWave = 0;
    private Coroutine waveRoutine;

    private void Awake()
    {
        Instance = this;
    }


    /// <summary>
    /// BattleStageApplier에서 호출
    /// 스테이지에 맞는 웨이브 데이터를 주입
    /// </summary>
    public void SetWaves(WaveInfo[] newWaves)
    {
        waves = newWaves;
    }

    /// <summary>
    /// 웨이브 전투 시작
    /// </summary>
    public void Begin()
    {
        if (waves == null || waves.Length == 0)
        {
            Debug.LogError("[EnemyWaveSystem] waves가 설정되지 않았습니다!");
            return;
        }

        // 이전 실행 중인 코루틴 정리
        if (waveRoutine != null)
        {
            StopCoroutine(waveRoutine);
            waveRoutine = null;
        }

        IsAllWavesCleared = false;
        currentWave = 0;

        waveRoutine = StartCoroutine(RunWaves());
    }

    /// <summary>
    /// 전체 웨이브 진행 코루틴
    /// </summary>
    private IEnumerator RunWaves()
    {
        // 전투 시작 전 대기 (연출용)
        yield return new WaitForSeconds(1f);

        while (currentWave < waves.Length)
        {
            Debug.Log($"[웨이브 시작] Wave {currentWave + 1}");

            yield return StartCoroutine(PlayWave(waves[currentWave]));

            currentWave++;
        }

        // 모든 웨이브 종료
        IsAllWavesCleared = true;
        Debug.Log("[전투 종료] 모든 웨이브 클리어!");

        // Enemy가 모두 죽었는지 확인 후 승리 판정
        BattleResultManager.Instance.CheckVictory();
    }

    /// <summary>
    /// 단일 웨이브 처리
    /// </summary>
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

        // 현재 웨이브 적이 모두 죽을 때까지 대기
        while (EnemyManager.Instance.GetAllEnemies().Count > 0)
            yield return null;

        Debug.Log($"[웨이브 완료] Wave {currentWave + 1}");

        // 다음 웨이브 전 대기
        yield return new WaitForSeconds(wave.delayAfterWave);
    }

    /// <summary>
    /// 적 스폰
    /// </summary>
    private void SpawnEnemy(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("[EnemyWaveSystem] enemyPrefab이 null입니다!");
            return;
        }

        Instantiate(prefab, spawnPoint.position, Quaternion.identity);
    }
}
