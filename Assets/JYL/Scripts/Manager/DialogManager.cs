using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class DialogManager : Singleton<DialogManager>
{
    // StartDialog()로 대화 시작

    private DialogUICanvas canvas;
    private PortraitPrefab portraitPrefab;
    
    [Header("Set Values")] 
    [SerializeField] private string csvPath = "CSV/TestDialog";
    [SerializeField][Range(0.01f, 0.1f)] private float outputTime = 0.04f;    

    // 텍스트 재생 변수
    private bool isTyping;
    private Tween typingTween;
    private bool isSkip;
    
    // 대화 UI 페이드 인/아웃 이미지 컬러 캐싱
    private Color fadedImgColor;
    private Color originFadeImgColor;
    
    // 다이얼로그 전체 딕셔너리
    private Dictionary<DialogKey, Dialog> dialogs;
    
    #region Initialize
    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        canvas = Resources.Load<DialogUICanvas>($"UI/DialogCanvas");
        canvas = Instantiate(canvas,transform);
        canvas.Init();
        portraitPrefab = Resources.Load<PortraitPrefab>($"UI/PortraitPrefab");
        GetDialogFromCsv();
        originFadeImgColor = canvas.fadeImage.color;
        fadedImgColor = originFadeImgColor;
        fadedImgColor.a = 0f;
    }

    private void GetDialogFromCsv()
    {
        dialogs = Util.ParseCsvToDialogs(csvPath);
    }
    
    private Dialog GetDialog(DialogKey key)
    {
        if (!dialogs.TryGetValue(key, out Dialog result))
        {
            Debug.LogWarning($"해당 키 값에 대한 다이얼로그 없음: {key.ToString()}");
        }

        return result;
    }
    #endregion
    
    #region 외부 요청 함수
    // 대화 씬 시작에 쓰이는 외부용 함수
    public async UniTask StartDialog(DialogKey key)
    {
        Dialog dialog = GetDialog(key);
        Time.timeScale = 0f;
        canvas.dialogText.text = "";
        canvas.nameField.text = "";
        
        foreach (DialogLine line in dialog.dialogContents)
        {
            await ProcessDialogLine(line); // 한 줄 처리
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        await EndDialog();
    }
    
    // 대화 스킵에 쓰이는 함수. 버튼에 달림
    public void DialogSkip()
    {
        isSkip = true;
    }

    public bool CheckDialogCondition(DialogCondition condition)
    {
        return Manager.Save.CurrentData.dialogRecord.CheckDialogCondition(condition);
    }

    public void MarkDialogCondition(DialogCondition condition)
    {
        Manager.Save.CurrentData.dialogRecord.MarkDialogCondition(condition);
    }
    #endregion
    
    #region Inner Logic
    // Dialog의 Line을 한 줄씩 로직처리
    private async UniTask ProcessDialogLine(DialogLine line)
    {
        // 대사의 종류에 따른 처리
        await TaskDialogLine(line);
        await UniTask.Yield(PlayerLoopTiming.Update);
        
        await TypeText(line.dialogContent);
        await UniTask.Yield(PlayerLoopTiming.Update);
    }

    // 글자 출력 함수
    private async UniTask TypeText(string content)
    {
        if (string.IsNullOrWhiteSpace(content) || isSkip)
        {
            typingTween.Kill();
            return;
        }
        canvas.dialogText.text = content;
        canvas.dialogText.maxVisibleCharacters = 0;

        int total = content.Length;
        isTyping = true;

        typingTween = DOTween.To(
            () => canvas.dialogText.maxVisibleCharacters,
            x => canvas.dialogText.maxVisibleCharacters = x, 
            total,
            outputTime * total)
            .SetUpdate(true)
            .SetEase(Ease.Linear);

        // 타이핑 중 키 입력 감지
        while (isTyping)
        {
            // 스킵 시
            if (isSkip)
            {
                typingTween.Kill();
                isTyping = false;
                await UniTask.Yield(PlayerLoopTiming.Update);
                break;
            }
            
            // 출력 스킵 : 트윈 종료
            if (Input.GetKeyDown(KeyCode.Space))
            {
                typingTween.Kill();
                canvas.dialogText.maxVisibleCharacters = total;
                isTyping = false;
                await UniTask.Yield(PlayerLoopTiming.Update);
                break;
            }
            
            // 트윈 종료 시
            if (!typingTween.active)
            {
                isTyping = false;
                await UniTask.Yield(PlayerLoopTiming.Update);
                break;
            }

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        
        // 타이핑 완료되면 다음 입력 기다리기
        await WaitNextKey();
    }
    
    // Line 출력 완료 시, 키 입력을 기다림
    private async UniTask WaitNextKey()
    {
        // 스킵된 경우 또한 포함
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            if (isSkip) return;
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        await UniTask.Yield(PlayerLoopTiming.Update);
    }
    
    // 들어온 라인의 로직을 종류에 맞게 처리
    private async UniTask TaskDialogLine(DialogLine line)
    {
        if (isSkip) return;
        switch (line.dialogType)
        {
            case DialogType.DialogStart:
                await DialogFadeIn();
                break;
            case DialogType.ChangeBackground:
                await ChangeBackGround(line.backgroundType);
                break;
            case DialogType.CharacterIn:
                // 현재 스프라이트 패널 UI에 요소로 신규 캐릭터 추가
                await canvas.portraitUIPanel.AddPortrait(line.speakerId);
                break;
            case DialogType.CharacterOut:
                // 현재 스프라이트 패널 UI에서 해당 캐릭터 뺌
                await canvas.portraitUIPanel.RemovePortrait(line.speakerId);
                break;
            case DialogType.NoVoice:
                // 노 보이스는 나레이션임
                canvas.nameField.text = "";
                // 이름 부분 UI 끄기
                await canvas.portraitUIPanel.HighlightOff();
                break;
            case DialogType.WithVoice:
                // 보이스 찾아서 출력
                Manager.Audio.PlayVoice(line.lineId);
                // 화자 이름 설정
                canvas.nameField.text = line.speakerId;
                // 화자 하이라이트
                await canvas.portraitUIPanel.HighlightSpeaker(line.speakerId);
                break;
            case DialogType.PlaySfx:
                // 효과음 재생 기능
                float delay = Manager.Audio.PlaySfx(line.sfxKey);
                await UniTask.Delay((int)delay*1000);
                break;
        }
    }

    // 대화 UI 종료
    private async UniTask EndDialog()
    {
        canvas.nameField.text = "";
        canvas.dialogText.text = "";
        
        await DialogFadeOut();
        await canvas.portraitUIPanel.InitializeUI();
        
        Time.timeScale = 1f;
        isSkip = false;
        isTyping = false;
        Manager.Audio.StopVoice();
    }
    
    // 배경 변경
    private async UniTask ChangeBackGround(BackgroundType type)
    {
        var container = Resources.Load<SpriteContainer>($"Image/Background/{type}");
        
        // 처음 변경하는 경우
        if (!canvas.backGroundPanel.gameObject.activeSelf)
        {
            canvas.backgroundImage.sprite = container.sprite;
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        // 대화 중간 변경하는 경우
        else
        {
            await FadeOutBackUI();
            canvas.backgroundImage.sprite = container.sprite;
            await FadeInBackUI();
        }
    }

    // 대화 창 UI 페이드 인
    private async UniTask DialogFadeIn()
    {
        canvas.backGroundPanel.SetActive(true);
        await FadeInBackUI();
        
        await UniTask.Yield(PlayerLoopTiming.Update);
    }

    // 대화 창 UI 페이드아웃
    private async UniTask DialogFadeOut()
    {
        await FadeOutBackUI();
        canvas.backGroundPanel.SetActive(false);
        
        await UniTask.Yield(PlayerLoopTiming.Update);
    }

    private async UniTask FadeInBackUI()
    {
        canvas.fadeImage.gameObject.SetActive(true);
        canvas.fadeImage.color = originFadeImgColor;
        
        await UniTask.Yield(PlayerLoopTiming.Update);
        
        await canvas.fadeImage
            .DOColor(fadedImgColor, 1f)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .AsyncWaitForCompletion()
            .AsUniTask();
        
        canvas.fadeImage.gameObject.SetActive(false);
    }

    private async UniTask FadeOutBackUI()
    {
        canvas.fadeImage.gameObject.SetActive(true);
        canvas.fadeImage.color = fadedImgColor;
        
        
        await  UniTask.Yield(PlayerLoopTiming.Update);
        
        await canvas.fadeImage.DOColor(originFadeImgColor, 1f)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .AsyncWaitForCompletion()
            .AsUniTask();
        
        
        canvas.fadeImage.gameObject.SetActive(false);
    }
    #endregion
}
