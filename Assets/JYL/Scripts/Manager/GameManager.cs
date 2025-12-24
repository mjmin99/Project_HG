using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private StageDataSO stageData;

    public StageSaveService saveService;

    public void ()
    {
        
    }


    public StageDataSO GetStageData()
    {
        return stageData;
    }
}
