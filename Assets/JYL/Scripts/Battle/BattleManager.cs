using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Set Refs")] 
    [SerializeField] private MapPresenter mapPresenter;
    [SerializeField] private EnemyManager enemyManager;
    
    [Header("Set Character Refs")]
    [SerializeField] private Transform characterParent;
    [SerializeField] private Transform[] characterPos;
    
    [Header("Set Values")]
    [SerializeField] private float rewindTime = 5f;
    
    private readonly List<CharController> characters = new();
    
    private StageDataSO stageData;
    private long clearTime;
    public int score;

    private float gameOverTimer;

    /// <summary>
    /// 1. 캐릭터 생성
    /// 2. StageData 불러오기
    /// 3. StageData 기준으로 에너미 및 웨이브 생성
    /// 4. 맵 생성(스테이지 키 값을 가지고 맵 데이터 불러옴) - 스크롤링 기법 적용
    /// 5. 생성 포인트에 웨이브에 맞춰 적 생성
    /// 6. 생성 포인트에 플레이어 생성 및 전투 진행(클리어 시 까지)
    /// </summary>
    // 외부 스테이지 셀렉트 UI 에서 수행되는 함수
    private void Update()
    {
        if (gameOverTimer > 0f)
        {
            gameOverTimer -= Time.deltaTime;
            if (gameOverTimer <= 0f)
            {
                GameOver();
            }
        }
    }
    public void StartStage() // 스테이지 시작 시
    {
        InitCharacters();
        enemyManager.Init();

        stageData = Manager.Game.GetStageData();
        clearTime = DateTime.Now.Millisecond;
    }
    
    public void StageClear() // 스테이지 클리어 시 세이브 데이터에 클리어 정보 저장
    {
        // TODO : Stage Clear UI 띄우기
        clearTime = DateTime.Now.Millisecond - clearTime;
        Manager.Game.stageService
            .ApplyClearResult(stageData.world, stageData.stage, clearTime, score);
        // TODO: 전투 관련 조작 막기, ESC 조작 막기
    }
    
    private void GameOver()
    {
        // TODO : 조작 로직 막기
        // 게임 오버 UI 띄우기
    }

    public void PauseGame() // 게임 일시정지
    {
        Time.timeScale = 0;
        // TODO: UI 띄우기
    }

    public void ReturnGame()
    {
        Time.timeScale = 1;
        // TODO: UI 닫기
    }

    public void ExitStage()
    {
        Time.timeScale = 1;
        // TODO: 씬 전환
    }

    public void RewindTime() // 시간 되감기 스킬
    {
        foreach (var c in characters)
        {
            c.RewindTime();
        }

    }

    private void SetMaps() // 맵 생성
    {
        mapPresenter.Init();
    } 

    // 캐릭터 정보 가져오고 초기화
    private void InitCharacters()
    {
        var partySet = Manager.Save.CurrentData.partySet;
        int count = 0;
        foreach (var member in partySet)
        {
            var stat = Manager.Character.GetStats(member);
            var model = Manager.Character.models[member];
            var go = new GameObject(model.characterName)
            {
                transform =
                {
                    position = characterPos[count++].position
                }
            };
            go.transform.SetParent(characterParent);
            var character = go.AddComponent<CharController>();
            character.Init(member, rewindTime);
            characters.Add(character);
            character.isDead.Subscribe(x => CheckAlive()).AddTo(character);
        }
    }

    // 플레이어 캐릭터가 전부 죽으면,
    // 시간 되감기 만큼 카운트 후 게임 오버 처리
    private void CheckAlive()
    {
        foreach (var c in characters)
        {
            if (!c.isDead.Value) return;
        }
        gameOverTimer = rewindTime;
    }
    
}
