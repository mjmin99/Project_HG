using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private float rewindCoolDown = 15f;

    [Header("Set UI")] 
    [SerializeField] public RectTransform uiCanvas;
    [SerializeField] private CharacterHpPresenter characterHpPresenter;
    [SerializeField] public SkillPresenter skillPresenter;
    
    private readonly List<CharController> characters = new();
    private List<Transform> charTransforms = new();
    public readonly Dictionary<int, CharController> skillDict = new();

    private int characterLayer;
    private Camera cam;
    public readonly List<SkillInfo> skills = new();
    
    // 스킬 타입 딕셔너리
    // 배치된 스킬 타입에 따라 UI의 이미지도 정해짐
    // 클릭 시, 해당 스킬을 소지한 캐릭터 중, 배치된 캐릭터를 찾아 스킬 사용
    private StageDataSO stageData;
    private long clearTime;
    public int score;

    private float gameOverTimer;
    private bool isGameOver;

    private void Start()
    {
        StartStage();
    }
    private void FixedUpdate()
    {
        if (!cam) return;
        
        float x = 0f;
        int count = 0;
        for(int i = 0; i< characters.Count; i++)
        {
            if (!characters[i].isDead.Value)
            {
                x += charTransforms[i].position.x;
                count++;
            }
            
        }
        if (count == 0) return;
        
        x /= count;
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

    private void StartStage() // 스테이지 시작 시
    {
        characterLayer = LayerMask.NameToLayer("Player");
        
        cam = Camera.main;
        SetMaps();
        InitCharacters();
        enemyManager.Init();
        Manager.Game.SetCharacters(characters);
        damageUI.Init();

        stageData = Manager.Game.GetStageData();
        clearTime = DateTime.Now.Millisecond; 
        charTransforms = characters.Select(c => c.transform).ToList();
        
        // 스킬 연결
        skillPresenter.Init(skills);
        // 캐릭터 UI 연결
        characterHpPresenter.Init();
    }
    
    public void StageClear() // 스테이지 클리어 시 세이브 데이터에 클리어 정보 저장
    {
        clearTime = DateTime.Now.Millisecond - clearTime;
        Manager.Game.stageService
            .ApplyClearResult(stageData.world, stageData.stage, clearTime, score);
        Manager.Save.SaveCurrentUser();
        Time.timeScale = 0f;
        // TODO : Stage Clear UI 띄우기 구현 필요
        // TODO: 전투 관련 조작 막기, ESC 조작 막기
        // UI에서 재시작, 메인으로 돌아가기 클릭 시 까지 수행 멈춤
        SceneManager.LoadScene("MainScene");
        Time.timeScale = 1f;
    }
    
    private void GameOver()
    {
        // TODO : 조작 로직 막기 구현 필요
        // 게임 오버 UI 띄우기
    }

    public void PauseGame() // 게임 일시정지. UI매니저 쪽에서 관리함
    {
        Time.timeScale = 0;
    }

    public void ReturnGame() // 일시정지에서 게임으로 되돌아가기. UI 매니저 쪽에서 관리함
    {
        Time.timeScale = 1;
    }

    public void ExitStage()
    {
        Time.timeScale = 1;
        // TODO: 씬 전환
        Manager.Game.ClearCharacters();
    }

    public void RewindTime() // 시간 되감기 스킬. 버튼으로 구현
    {
        foreach (var c in characters)
        {
            c.RewindTime();
        }

        skillPresenter.SetRewind(rewindTime, rewindCoolDown);
    }

    // 스킬 아이콘 클릭 시 스킬 사용
    public void OnClickSkills(int characterId, int index)
    {
        var tmp = skills.Find(x => x.charId == characterId);
        bool canUse = !isGameOver && tmp.skillCount.Value > 0;
        
        if (!canUse) return;
        
        if (skillDict[characterId].UseSkill())
        {
            tmp.skillCount.Value--;
            skillPresenter.SetTxt(index,$"{tmp.type} : {tmp.skillCount}");
        }
    }

    public void GetSkill(int index)
    {
        var tmp = skills[index];
        if (tmp.skillCount.Value >= 3) return;
        tmp.skillCount.Value++;
        skillPresenter.skillButtonPanel[index].transform.localScale = Vector3.one * 0.7f;
        skillPresenter.skillButtonPanel[index].transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.InBounce);
    }
    
    //내부로직
    private void SetMaps() // 맵 생성
    {
        mapPresenter.Init();
    } 

    // 캐릭터 정보 가져오고 초기화
    private void InitCharacters()
    {
        int[] partySet = Manager.Save.CurrentData.partySet;
        
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
            var skillInfo = character.skillPrefab;
            var skillCooldown = skillInfo.GetSkillCooldown();
            skills.Add(new SkillInfo(
                model.id,skillInfo.skillIcon,
                skillInfo.skillType,
                0,
                skillCooldown));
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
