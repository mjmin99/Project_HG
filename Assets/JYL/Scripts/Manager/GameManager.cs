using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private StageDataSO currentStageData;

    public StageSaveService stageService;
    public List<CharController> Characters;

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
