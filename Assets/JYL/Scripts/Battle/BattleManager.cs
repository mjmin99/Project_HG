using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [Header("Set Refs")] 
    [SerializeField] private MapPresenter mapPresenter;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] public DamageUI damageUI;
    
    [Header("Set Character Refs")]
    [SerializeField] private Transform characterParent;
    [SerializeField] private Transform[] characterPos;
    
    [Header("Set Values")]
    [SerializeField] private float rewindTime = 5f;

    [Header("Set UI")] 
    [SerializeField] private TestSkillUI skillUI;
    
    private readonly List<CharController> characters = new();
    private List<Transform> charTransforms = new();
    private readonly Dictionary<int, CharController> skillDict = new();

    private int characterLayer;
    private Camera cam;
    private readonly List<SkillCounter> skills = new();
    
    // 스킬 타입 딕셔너리
    // 배치된 스킬 타입에 따라 UI의 이미지도 정해짐
    // 클릭 시, 해당 스킬을 소지한 캐릭터 중, 배치된 캐릭터를 찾아 스킬 사용
    private StageDataSO stageData;
    private long clearTime;
    public int score;

    private float gameOverTimer;
    private bool isGameOver;

    private void FixedUpdate()
    {
        if (!cam) return;
        
        float x = 0f;
        foreach (var c in charTransforms)
        {
            x += c.position.x;
        }
        x /= charTransforms.Count;
        cam.transform.position = Vector3.Lerp(cam.transform.position,new Vector3(x + 1f,cam.transform.position.y,cam.transform.position.z), Time.fixedDeltaTime * 10);
    }
    
    private void Update()
    {
        if (!(gameOverTimer > 0f) || !isGameOver) return;
        
        gameOverTimer -= Time.deltaTime;
        
        if (gameOverTimer <= 0f)
        {
            GameOver();
        }
    }
    
    // TODO: 테스트용으로 함수 수행
    [ContextMenu("Start")]
    public void StartStage() // 스테이지 시작 시
    {
        characterLayer = LayerMask.NameToLayer("Player");
        
        cam = Camera.main;
        SetMaps();
        InitCharacters(true);
        enemyManager.Init();
        Manager.Game.SetCharacters(characters);
        damageUI.Init();

        stageData = Manager.Game.GetStageData();
        clearTime = DateTime.Now.Millisecond; 
        charTransforms = characters.Select(c => c.transform).ToList();
        
        // TODO: 스킬 테스트
        skillUI.Init(skills);
        for(int i = 0; i < skills.Count ; i++)
        {
            int index = i;
            skills[i].skillCount
                .Subscribe(n 
                    => skillUI.SetTxt(index, $"{skills[index].type}: {n} left"))
                .AddTo(skillUI);
        }
        
        // TODO: 스킬 테스트
        skills[0].skillCount.Value++;
        skills[0].skillCount.Value++;
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
    public void OnClickSkills(int characterId, int num)
    {
        var tmp = skills.Find(x => x.charId == characterId);
        bool canUse = !isGameOver && tmp.skillCount.Value > 0;
        
        if (!canUse) return;
        
        if (skillDict[characterId].UseSkill())
        {
            tmp.skillCount.Value--;
            skillUI.SetTxt(num,$"{tmp.type} : {tmp.skillCount}");
        }
    }

    public void GetSkill(int characterId)
    {
        var tmp = skills.Find(x => x.charId == characterId);
        if (tmp.skillCount.Value >= 3) return;
        tmp.skillCount.Value++; ;
    }
    
    //내부로직
    private void SetMaps() // 맵 생성
    {
        mapPresenter.Init();
    } 

    // 캐릭터 정보 가져오고 초기화
    // TODO : 테스트. 캐릭터 정보 미리 정하기
    private void InitCharacters(bool isTest)
    {
        int[] partySet;
        if (isTest) // TODO: 테스트 종료시 삭제
        {
            partySet = new[] { 0, 1, 5 };
        }
        else
        {
            partySet = Manager.Save.CurrentData.partySet;
        }
        
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
            go.tag = "Player";
            go.layer = characterLayer; 
            
            var stat = Manager.Character.GetStats(member);
            var character = go.AddComponent<CharController>();
            character.Init(member, stat, rewindTime, damageUI);
            character.isDead.Subscribe(_ => CheckAlive()).AddTo(character);
            
            var inst = Manager.Character.instances[member];
            skillDict.Add(model.id, character);
            skills.Add(new SkillCounter(model.id,inst.skillType));
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
