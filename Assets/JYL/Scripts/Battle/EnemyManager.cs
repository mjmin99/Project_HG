using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Random = UnityEngine.Random;

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
    private Camera cam;
    
    public void Init()
    {
        stageData = Manager.Game.GetStageData();
        curWaveSpawns = stageData.waves[waveIndex].spawns;
        lastWaveIndex = stageData.waves.Count - 1;
        curWaveEnemies = CreateEnemy();
        cam = Camera.main;
    }

    private List<EnemyController> CreateEnemy() 
    {
        var list = new List<EnemyController>(); 

        foreach (var e in curWaveSpawns)
        {
            GameObject go = new GameObject($"{e.id}");
            go.transform.SetParent(enemiesParent);
            // TODO: 적 위치 잡아주기
            float rndX = Random.Range(0, 2f);
            var camPos = cam.transform.position;
            go.transform.position = new Vector3(camPos.x + 2f + rndX, 0, 0);
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
        if (!isDead) return;
        
        curWaveEnemies.Remove(controller);
        
        if (curWaveEnemies.Count != 0) return;
        
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
