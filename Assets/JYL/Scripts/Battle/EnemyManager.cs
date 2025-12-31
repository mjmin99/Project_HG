using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [Header("Set Refs")]
    [SerializeField] private EnemyDatabase enemyDatabase;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private Transform enemiesParent;
    [SerializeField] public DamageUI enemyDamageUI;
    [SerializeField] public DropSkill skillDropPrefab;

    private StageDataSO stageData;

    private int waveIndex;
    private int lastWaveIndex;
    private List<StageSpawn> curWaveSpawns;
    private List<EnemyController> curWaveEnemies;
    private Camera cam;

    private int enemyLayer;
    
    public void Init()
    {
        enemyLayer = LayerMask.NameToLayer("Enemy");
        cam = Camera.main;
        stageData = Manager.Game.GetStageData();
        curWaveSpawns = stageData.waves[waveIndex].spawns;
        lastWaveIndex = stageData.waves.Count - 1;
        enemyDamageUI.Init();
        curWaveEnemies = CreateEnemy();
    }

    private List<EnemyController> CreateEnemy() 
    {
        var list = new List<EnemyController>(); 

        foreach (var e in curWaveSpawns)
        {
            GameObject go = new GameObject($"{e.id}");
            go.transform.SetParent(enemiesParent);
            float rndX = Random.Range(0, 2f);
            var camPos = cam.transform.position;
            go.transform.position = new Vector3(camPos.x + 6f + rndX, 0, 0);
            go.layer = enemyLayer;
            
            var enemy = go.AddComponent<EnemyController>();
            
            var info = enemyDatabase.Get(e.id);
            if (info.isBoss)
            {
                enemy.Init(info, enemyDamageUI, battleManager.uiCanvas); 
                CheckBossDialog().Forget(); // 다이얼로그 재생을 위한 확인
            }
            else
            {
                enemy.Init(info, enemyDamageUI);
            }
            
            enemy.isDead.Subscribe(x=>EnemyDeadEvent(x,enemy).Forget()).AddTo(enemy);
            
            if (info.isBoss)
            { 
                enemy.curHp.Subscribe(x => DropSkillBoss(enemy, x)).AddTo(enemy);
            }
            else
            {
                enemy.isDead.Subscribe(x => DropSkillEnemy(enemy, x)).AddTo(enemy);
            }
            
            list.Add(enemy);
        }
        
        return list;
    }

    private async UniTask CheckBossDialog()
    {
        DialogCondition condition;
        switch(stageData.world)
        {
            case 1:
                condition = DialogCondition.WorldBoss1;
                Manager.Dialog.CheckDialogCondition(condition);
                await Manager.Dialog.StartDialog(DialogKey.Scene3);
                break;
            case 2:
                condition = DialogCondition.WorldBoss2;
                Manager.Dialog.CheckDialogCondition(condition);
                await Manager.Dialog.StartDialog(DialogKey.Scene6);
                break;
            case 3:
                condition = DialogCondition.WorldBoss3;
                Manager.Dialog.CheckDialogCondition(condition);
                await Manager.Dialog.StartDialog(DialogKey.Scene8);
                break;
            case 4:
                condition = DialogCondition.WorldBoss4;
                Manager.Dialog.CheckDialogCondition(condition);
                await Manager.Dialog.StartDialog(DialogKey.Scene10);
                break;
            case 5:
                condition = DialogCondition.WorldBoss5;
                Manager.Dialog.CheckDialogCondition(condition);
                await Manager.Dialog.StartDialog(DialogKey.Scene12);
                break;
            default:
                Debug.LogWarning($"다이얼로그 컨디션이 설정되지 않은 보스임!! : 월드 {stageData.world}");
                return;
        }
        Manager.Dialog.MarkDialogCondition(condition);
    }

    private async UniTask CheckBossDeadDialog()
    {
        DialogCondition condition;
        switch(stageData.world)
        {
            case 1:
                condition = DialogCondition.WorldBoss1Down;
                Manager.Dialog.CheckDialogCondition(condition);
                await Manager.Dialog.StartDialog(DialogKey.Scene4);
                break;
            case 5:
                condition = DialogCondition.WorldBoss5Down;
                Manager.Dialog.CheckDialogCondition(condition);
                await Manager.Dialog.StartDialog(DialogKey.Scene13);
                break;
            default:
                return;
        }
        Manager.Dialog.MarkDialogCondition(condition);
    }

    private async UniTask EnemyDeadEvent(bool isDead, EnemyController controller)
    {
        if (!isDead) return;
        if (controller.enemyInfo.isBoss)
        {
            await CheckBossDeadDialog();
        }
        
        curWaveEnemies.Remove(controller);
        
        if (curWaveEnemies.Count != 0) return;
        
        if (waveIndex == lastWaveIndex)
        {
            battleManager.StageClear().Forget();
            return;
        }

        waveIndex++;
        curWaveSpawns = stageData.waves[waveIndex].spawns;
        curWaveEnemies = CreateEnemy();
    }

    private void DropSkillEnemy(EnemyController controller, bool isDead) // 일반 에너미 죽을 시 스킬 드롭
    {
        if (!isDead) return;
        // 랜덤 확률 적용
        int rnd = Random.Range(0, 100);
        if (rnd > 50) // 50퍼 확률로
        {
            int index = Random.Range(0, battleManager.skills.Count - 1);
            var skill = battleManager.skills[index];
            var go = Instantiate(skillDropPrefab, battleManager.uiCanvas);
            go.Init(index, skill.skillIcon, controller.transform, battleManager);
            go.transform.SetParent(battleManager.uiCanvas);
        }
    }

    private void DropSkillBoss(EnemyController controller, float curHp)
    {
        float hpPercent = controller.GetHpPercent();
        
        int skillIndex = 0;
        
        switch (hpPercent)
        {
            case < 0.2f:
                skillIndex = 3;
                break;
            case < 0.4f:
                skillIndex = 2;
                break;
            case < 0.6f:
                skillIndex = 1;
                break;
            case < 0.8f:
                skillIndex = 0;
                break;
            default:
                return;
        }
        
        for (int i = 0; i < skillIndex; i++)
        {
            if (!controller.skillDropHp[i])
            {
                controller.skillDropHp[i] = true;
            }
        }

        if (controller.skillDropHp[skillIndex]) return;
        Debug.Log($"보스 스킬 드랍 시작{hpPercent}");
        controller.skillDropHp[skillIndex] = true;

        int rndIndex = Random.Range(0, battleManager.skills.Count - 1);
        var skill = battleManager.skills[rndIndex];
        var go = Instantiate(skillDropPrefab, battleManager.uiCanvas);
        go.Init(rndIndex, skill.skillIcon, controller.transform, battleManager);
        go.transform.SetParent(battleManager.uiCanvas);
    }
}
