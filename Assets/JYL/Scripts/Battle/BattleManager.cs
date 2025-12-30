using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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
    [SerializeField] private StageClearPanel  stageClearPanel;
    
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
    private float clearTime;
    public int score;

    private float gameOverTimer;
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
        if (!(gameOverTimer > 0f) || !Manager.Game.IsGameOver) return;
        
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
        Manager.Game.IsBattle = true;
        Manager.Game.SetCharacters(characters);
        damageUI.Init();

        stageData = Manager.Game.GetStageData();
        clearTime = Time.time; 
        charTransforms = characters.Select(c => c.transform).ToList();
        
        // 스킬 연결
        skillPresenter.Init(skills);
        // 캐릭터 UI 연결
        characterHpPresenter.Init();
        
        stageClearPanel.gameObject.SetActive(false);
    }
    
    public async UniTask StageClear() // 스테이지 클리어 시 세이브 데이터에 클리어 정보 저장
    {
        Debug.Log("BattleManager 스테이지 클리어");
        Manager.Game.IsGameClear = true;
        clearTime = Time.time - clearTime;
        Manager.Game.stageService
            .ApplyClearResult(stageData.world, stageData.stage, (long)clearTime, score);
        
        Manager.Save.SaveCurrentUser();
        await UniTask.WhenAll(Manager.Game.tasks);
        
        UIManager.Instance.CloseTop();
        
        stageClearPanel.gameObject.SetActive(true);
        stageClearPanel.Init(clearTime);
    }
    
    private void GameOver() // 게임 오버 조건 구독은 CheckAlive, 조건 체크는 Update에서 진행
    {
        Debug.Log("BattleManager 스테이지 클리어 실패");
        Manager.Game.IsGameOver = true;
        Time.timeScale = 0f;
        // 게임 오버 UI 띄우기
        UIManager.Instance.OpenUI<UIPanel>("BattleOptionPanel");
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
        bool canUse = !Manager.Game.IsGameOver && tmp.skillCount.Value > 0;
        
        if (!canUse) return;
        
        if (skillDict[characterId].UseSkill())
        {
            tmp.skillCount.Value--;
            skillPresenter.SetTxt(index,$"{tmp.type} : {tmp.skillCount}");
        }
    }

    public void GetSkill(int index)
    {
        if (Manager.Game.IsGameClear) return;
        var tmp = skills[index];
        if (tmp.skillCount.Value >= 3) return;
        tmp.skillCount.Value++;
        skillPresenter.skillButtonPanel[index].transform.localScale = Vector3.one * 0.7f;
        var t1 = skillPresenter.skillButtonPanel[index].transform
            .DOScale(Vector3.one, 0.3f)
            .SetEase(Ease.OutBounce)
            .AsyncWaitForCompletion()
            .AsUniTask();
        Manager.Game.tasks.Add(t1);
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
            Manager.Game.IsGameOver = false;
            return;
        }
        
        gameOverTimer = rewindTime;
        Manager.Game.IsGameOver = true;
    }
}
