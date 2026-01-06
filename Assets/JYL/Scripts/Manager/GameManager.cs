using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private StageDataSO currentStageData;

    public StageSaveService stageService;
    public List<CharController> Characters;

    // 어드레서블 수정 중 
    public BattleManager CurrentBattleManager { get; set; }

    public bool IsBattle { get; set; }
    public bool IsGameOver { get; set; }
    public bool IsGameClear { get; set; }
    public bool IsPaused { get; set; }

    public List<UniTask> tasks = new();
    
    protected override void Awake()
    {
        base.Awake();
        stageService = new StageSaveService();
    }

    public void StageServiceInit()
    {
        if (!stageService.EnsureInitialized())
        {
            Debug.LogError("스테이지 세이브 서비스 초기화 실패");
        }
    }
    
    
    public void SetCharacters(List<CharController> characters)
    {
        Characters = characters;
    }
    public void ClearCharacters() => Characters.Clear();
    
    public StageDataSO GetStageData() => currentStageData;
    
    public void SetStageData(StageDataSO stageData) => currentStageData = stageData;
    
    public void ClearStageData() => currentStageData = null;
}
