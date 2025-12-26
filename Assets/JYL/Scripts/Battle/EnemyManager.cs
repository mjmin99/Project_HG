using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UniRx;

public class EnemyManager : MonoBehaviour
{
    [Header("Set Refs")]
    [SerializeField] private EnemyDatabase enemyDatabase;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private Transform enemiesParent;

    private StageDataSO stageData;

    private int waveIndex;
    private int lastWaveIndex;
    private List<StageSpawn> curWaveSpawns;
    private List<EnemyController> curWaveEnemies;
    
    public void Init()
    {
        stageData = Manager.Game.GetStageData();
        curWaveSpawns = stageData.waves[waveIndex].spawns;
        lastWaveIndex = stageData.waves.Count - 1;
        curWaveEnemies = CreateEnemy();
    }

    private List<EnemyController> CreateEnemy() 
    {
        var list = new List<EnemyController>(); 

        foreach (var e in curWaveSpawns)
        {
            GameObject go = new GameObject($"{e.id}");
            go.transform.SetParent(enemiesParent);
            // 적 위치 잡아주기
            var enemy = go.AddComponent<EnemyController>();
            var info = enemyDatabase.Get(e.id);
            enemy.Init(info);
            enemy.isDead.Subscribe(x=>EnemySubscribe(x,enemy)).AddTo(enemy);
            list.Add(enemy);
        }
        
        return list;
    }

    private void EnemySubscribe(bool isDead, EnemyController controller)
    {
        if (isDead)
        {
            curWaveEnemies.Remove(controller);
            if (curWaveEnemies.Count == 0)
            {
                if (waveIndex == lastWaveIndex)
                {
                    battleManager.StageClear();
                    return;
                }

                waveIndex++;
                curWaveSpawns = stageData.waves[waveIndex].spawns;
                curWaveEnemies = CreateEnemy();
            }
        }
    }
}
