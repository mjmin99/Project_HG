using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private StageDataSO stageData;

    public StageSaveService stageService;
    public List<CharController> Characters;

    protected override void Awake()
    {
        base.Awake();
        stageService = new StageSaveService();
        // TODO: 테스트용 함수로 변경 사용
        stageService.EnsureInitialized();
    }


    public StageDataSO GetStageData()
    {
        return stageData;
    }
    public void SetCharacters(List<CharController> characters)
    {
        Characters = characters;
    }
    public void ClearCharacters() => Characters.Clear();
}
