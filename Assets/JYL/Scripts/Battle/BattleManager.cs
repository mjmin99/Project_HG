using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
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
    private readonly Dictionary<SkillType, CharController> skillDict = new();
    
    // 스킬 타입 딕셔너리
    // 배치된 스킬 타입에 따라 UI의 이미지도 정해짐
    // 클릭 시, 해당 스킬을 소지한 캐릭터 중, 배치된 캐릭터를 찾아 스킬 사용
    private StageDataSO stageData;
    private long clearTime;
    public int score;

    private float gameOverTimer;
    private bool isGameOver;
    
    private void Update()
    {
        if (!(gameOverTimer > 0f) || !isGameOver) return;
        
        gameOverTimer -= Time.deltaTime;
        
        if (gameOverTimer <= 0f)
        {
            GameOver();
        }
    }
    
    public void StartStage() // 스테이지 시작 시
    {
        SetMaps();
        InitCharacters();
        enemyManager.Init();
        Manager.Game.SetCharacters(characters);

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
        Manager.Game.ClearCharacters();
    }

    public void RewindTime() // 시간 되감기 스킬
    {
        foreach (var c in characters)
        {
            c.RewindTime();
        }
    }

    // 스킬 아이콘 클릭 시 스킬 사용
    // 스킬 사용 실패 시, false 반환해서 스킬 소모 막음
    public bool OnClickSkills(SkillType skillType) 
    {
        if (isGameOver) return false;
        
        return skillDict[skillType].UseSkill();
    }
    
    //내부로직
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
            var model = Manager.Character.models[member];
            var go = new GameObject(model.characterName)
            {
                transform =
                {
                    position = characterPos[count++].position
                }
            };
            go.transform.SetParent(characterParent);
            
            var stat = Manager.Character.GetStats(member);
            var character = go.AddComponent<CharController>();
            character.Init(member, stat, rewindTime);
            character.isDead.Subscribe(_ => CheckAlive()).AddTo(character);
            
            var inst = Manager.Character.instances[member];
            skillDict.Add(inst.skillType, character);
            
            characters.Add(character);
        }
    }

    // 플레이어 캐릭터가 전부 죽으면,
    // 시간 되감기 만큼 카운트 후 게임 오버 처리
    private void CheckAlive()
    {
        foreach (var c in characters)
        {
            if (c.isDead.Value) continue;
            isGameOver = false;
            return;
        }
        
        gameOverTimer = rewindTime;
        isGameOver = true;
    }
}
