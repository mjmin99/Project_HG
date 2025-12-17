using UnityEngine;
using System.Collections.Generic;

// 배틀씬 전용 에너미 생성, 삭제, 정리 관리자
public class EnemyManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private EnemyDatabase enemyDatabase;
    
    // 현재 생성된 에너미컨트롤러 목록
    private readonly List<TestEnemyController> activeEnemies = new();

    // StageDataSO를 받아 현재 스테이지의 모든 웨이브 스폰 요청
    public void SpawnEnemies(StageWave wave)
    {
        foreach (var spawn in wave.spawns)
        {
            for (int i = 0; i < spawn.count; i++)
            {
                SpawnEnemy(spawn.id);
            }
        }
    }

    // monsterId 기반 EnemyController 생성
    private void SpawnEnemy(int id)
    {
        Enemy model = enemyDatabase.Get(id);

        if (model == null)
        {
            Debug.LogError($"[EnemyManager] 에너미 찾을 수 없음: {id}");
            return;
        }

        // PrefabPath로 프리팹 로드
        GameObject prefab = Resources.Load<GameObject>(model.name);

        if (prefab = null)
        {
            Debug.LogError($"[EnemyManager] 프리펩 찾을 수 없음: {model.name}");
            return;
        }

        GameObject go = Instantiate(prefab, transform.position, Quaternion.identity);

        TestEnemyController controller = go.GetComponent<TestEnemyController>();
        if (controller == null)
        {
            Debug.LogError("[EnemyManager] 에너미컨트롤러 컴포넌트 없음");
            Destroy(go);
            return;
        }

        controller.Init(model);

        activeEnemies.Add(controller);
    }

    // 배틀 종료시 호출
    public void ClearAll()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null)
            { 
                Destroy(enemy.gameObject);
            }
        }

        activeEnemies.Clear();
        Debug.Log("[EnemyManager] All enemies cleared");
    }
}
