using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

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

    // todo --------- 어드레서블 적용 중
    private BattlePrefabLoader prefabLoader;


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
    private async void Start()
    {
        await StartStageAsync();
    }

    private void FixedUpdate()
    {
        // 어드레서블 수정 중
        if (characters == null || characters.Count == 0)
            return;

        if (!cam) return;
        
        float x = 0f;
        int count = 0;
        // for(int i = 0; i< characters.Count; i++)
        // {
        //     if (!characters[i].isDead.Value)
        //     {
        //         x += charTransforms[i].position.x;
        //         count++;
        //     }
        //     
        // }

        int loopCount = Mathf.Min(characters.Count, charTransforms.Count);

        for (int i = 0; i < loopCount; i++)
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
        prefabLoader = new BattlePrefabLoader();

        characterLayer = LayerMask.NameToLayer("Player");
        
        cam = Camera.main;
        SetMaps();
        // todo 어드레서블로 고치는 중
        // InitCharacters(); 이전거
        InitCharactersAsync().Forget();
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
        DialogCheck().Forget();
        Manager.Audio.SwapClip(AudioClipType.BGM, $"W{stageData.world}BGM").Forget();
    }

    private async UniTask StartStageAsync()
    {
        prefabLoader = new BattlePrefabLoader();

        characterLayer = LayerMask.NameToLayer("Player");
        cam = Camera.main;

        SetMaps();

        // 캐릭터/스킬 로딩이 끝날 때까지 기다린다
        await InitCharactersAsync();

        // 여기부터는 "캐릭터가 확실히 존재"하는 상태
        Manager.Game.IsBattle = true;
        Manager.Game.SetCharacters(characters);

        // (추천) charTransforms는 지우는 게 베스트지만, 유지한다면 여기서 다시 만든다
        charTransforms = characters.Select(c => c.transform).ToList();

        // UI는 이제 안전하게 초기화 가능
        skillPresenter.Init(skills);
        characterHpPresenter.Init();

        // 카메라 첫 프레임 스냅(첫 진입 이상현상 방지)
        SnapCameraToAliveCharacters();

        // 나머지
        damageUI.Init();
        enemyManager.Init();

        stageData = Manager.Game.GetStageData();
        clearTime = Time.time;

        stageClearPanel.gameObject.SetActive(false);

        DialogCheck().Forget();
        Manager.Audio.SwapClip(AudioClipType.BGM, $"W{stageData.world}BGM").Forget();
    }

    private void SnapCameraToAliveCharacters()
    {
        if (!cam || characters == null || characters.Count == 0) return;

        float x = 0f;
        int count = 0;

        foreach (var c in characters)
        {
            if (c == null || c.isDead.Value) continue;
            x += c.transform.position.x;
            count++;
        }

        if (count == 0) return;

        x /= count;
        cam.transform.position = new Vector3(x + 1f, cam.transform.position.y, cam.transform.position.z);
    }

    private async UniTask DialogCheck()
    {
        if (stageData.stage != 1) return;
        
        DialogCondition condition;
        
        switch(stageData.world)
        {
            case 1:
                condition = DialogCondition.EnterW1S1;
                if (!Manager.Dialog.CheckDialogCondition(condition))
                {
                    await Manager.Dialog.StartDialog(DialogKey.Scene2);
                }
                break;
            case 2:
                condition = DialogCondition.EnterW2S1;
                if (!Manager.Dialog.CheckDialogCondition(condition))
                {
                    await Manager.Dialog.StartDialog(DialogKey.Scene5);
                }
                break;
            case 3:
                condition = DialogCondition.EnterW3S1;
                if (!Manager.Dialog.CheckDialogCondition(condition))
                {
                    await Manager.Dialog.StartDialog(DialogKey.Scene7);
                }
                break;
            case 4:
                condition = DialogCondition.EnterW4S1;
                if (!Manager.Dialog.CheckDialogCondition(condition))
                {
                    await Manager.Dialog.StartDialog(DialogKey.Scene9);
                }
                break;
            case 5:
                condition = DialogCondition.EnterW5S1;
                if (!Manager.Dialog.CheckDialogCondition(condition))
                {
                    await Manager.Dialog.StartDialog(DialogKey.Scene11);
                }
                break;
            default:
                Debug.LogWarning("다이얼로그가 설정되지 않은 월드임");
                return;
        }
        Manager.Dialog.MarkDialogCondition(condition);
    }
    
    public async UniTask StageClear() // 스테이지 클리어 시 세이브 데이터에 클리어 정보 저장
    {
        Debug.Log("BattleManager 스테이지 클리어");
        Manager.Game.IsGameClear = true;
        clearTime = Time.time - clearTime;
        Manager.Game.stageService
            .ApplyClearResult(stageData.world, stageData.stage, (long)clearTime, score);
        // Manager.Save.AddGold(Manager.Game.GetStageData().rewardGold);
        // Manager.Save.SaveCurrentUser();
        var key = StageKeyUtil.ToKey(stageData.world, stageData.stage);
        var record = Manager.Game.stageService.GetMutableRecord(stageData.world, stageData.stage);

        Manager.Save.PatchGold();
        Manager.Save.PatchStageRecord(key, record);

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
            .SetEase(Ease.OutElastic)
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
    // todo 어드레서블로 고친 캐릭터 초기화 어싱크
    private async UniTask InitCharactersAsync()
    {
        if (prefabLoader == null)
        {
            Debug.LogError("[BattleManager] prefabLoader is null");
            return;
        }

        if (characterParent == null || characterPos == null || characterPos.Length == 0)
        {
            Debug.LogError("[BattleManager] Character spawn references not set");
            return;
        }

        int[] partySet = Manager.Save.CurrentData.partySet;
        int count = 0;

        foreach (var member in partySet)
        {
            if (!Manager.Character.models.TryGetValue(member, out var model))
            {
                Debug.LogError($"[BattleManager] Character model not found: {member}");
                continue;
            }

            // 배틀 전용 프리팹 주소
            string address = $"Characters/{model.characterName}_Battle";

            var instance = await prefabLoader.LoadAndSpawn(address, characterParent);
            if (instance == null)
                continue;

            if (count >= characterPos.Length)
            {
                Debug.LogError("[BattleManager] characterPos overflow");
                break;
            }

            instance.transform.position = characterPos[count++].position;
            instance.tag = "Player";
            instance.layer = characterLayer;

            var character = instance.GetComponent<CharController>();
            if (character == null)
            {
                Debug.LogError($"[BattleManager] CharController missing on {address}");
                continue;
            }

            var stat = Manager.Character.GetStats(member);
            character.Init(member, stat, rewindTime, damageUI);

            character.isDead
                .Subscribe(_ => CheckAlive())
                .AddTo(character);

            characters.Add(character);

            // === 스킬/UI 연동 ===
            skillDict.Add(model.id, character);

            var skillInfo = character.skillPrefab;
            if (skillInfo == null)
            {
                Debug.LogError($"[BattleManager] SkillPrefab missing on {address}");
                continue;
            }

            var cooldown = skillInfo.GetSkillCooldown();
            skills.Add(new SkillInfo(
                model.id,
                skillInfo.skillIcon,
                skillInfo.skillType,
                0,
                cooldown
            ));
        }

        Manager.Game.SetCharacters(characters);
    }

    // 어드레서블용 캐릭터 릴리즈~ 
    public void ReleaseBattleCharacters()
    {
        prefabLoader?.ReleaseAll();
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
