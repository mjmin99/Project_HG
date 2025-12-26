using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private StageDataSO stageData;

    public StageSaveService stageService;

    protected override void Awake()
    {
        base.Awake();
        stageService = new StageSaveService();
        stageService.Init();
    }


    public StageDataSO GetStageData()
    {
        return stageData;
    }
}
