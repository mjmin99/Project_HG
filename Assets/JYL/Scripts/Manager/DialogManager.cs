using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    /// <summary>
    /// 각 값들과 참조들을 정확히 연결해야 함
    /// StartDialog()로 대화 시작
    /// </summary>
    [Header("Set Manager")]
    [SerializeField] private AudioManager audioManager;

    [Header("Set Values")] 
    [SerializeField] private string csvPath = "CSV/TestDialog";
    [SerializeField][Range(0.01f, 0.1f)] private float outputTime = 0.04f;    
    
    [Header("Set UI References")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject backGroundPanel;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject characterContent;
    [SerializeField] private PortraitUIPanel portraitUIPanel;
    [SerializeField] private PortraitPrefab portraitPrefab;
    [SerializeField] private TMP_Text nameField;
    [SerializeField] private TMP_Text dialogText;
    [SerializeField] private Button skipButton;
   
    // TODO: Dialog Test
    [Header("For Test")]
    [SerializeField] private Button testButton;
    [SerializeField] private DialogKey testKey = DialogKey.Test1;

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
    void Awake()
    {
        Init();
    }

    private void Init()
    {
        backGroundPanel.gameObject.SetActive(false);
        fadeImage.gameObject.SetActive(false);
        GetDialogFromCsv();
        testButton.OnClickAsObservable().Subscribe(_ => StartDialog(testKey).ToAsyncLazy());
        originFadeImgColor = fadeImage.color;
        fadedImgColor = originFadeImgColor;
        fadedImgColor.a = 0f;
        skipButton.OnClickAsObservable().Subscribe(_ => DialogSkip());
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
    
    // 대화 씬 시작에 쓰이는 외부용 함수
    public async UniTask StartDialog(DialogKey key)
    {
        Dialog dialog = GetDialog(key);
        Time.timeScale = 0f;
        dialogText.text = "";
        nameField.text = "";
        
        foreach (DialogLine line in dialog.dialogContents)
        {
            await ProcessDialogLine(line); // 한 줄 처리
        }

        await EndDialog();
    }
    
    #region Inner Logic
    // Dialog의 Line을 한 줄씩 로직처리
    private async UniTask ProcessDialogLine(DialogLine line)
    {
        // 대사의 종류에 따른 처리
        await TaskDialogLine(line);
        
        await TypeText(line.dialogContent);
    }

    // 글자 출력 함수
    private async UniTask TypeText(string content)
    {
        if (string.IsNullOrWhiteSpace(content) || isSkip)
        {
            typingTween.Kill();
            return;
        }
        dialogText.text = content;
        dialogText.maxVisibleCharacters = 0;

        int total = content.Length;
        isTyping = true;

        typingTween = DOTween.To(
            () => dialogText.maxVisibleCharacters,
            x => dialogText.maxVisibleCharacters = x, 
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
                dialogText.maxVisibleCharacters = total;
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
                await portraitUIPanel.AddPortrait(line.speakerId);
                break;
            case DialogType.CharacterOut:
                // 현재 스프라이트 패널 UI에서 해당 캐릭터 뺌
                await portraitUIPanel.RemovePortrait(line.speakerId);
                break;
            case DialogType.NoVoice:
                // 노 보이스는 나레이션임
                nameField.text = "";
                // 이름 부분 UI 끄기
                await portraitUIPanel.HighlightOff();
                break;
            case DialogType.WithVoice:
                // 보이스 찾아서 출력
                audioManager.PlayVoice(line.lineId);
                // 화자 이름 설정
                nameField.text = line.speakerId;
                // 화자 하이라이트
                await portraitUIPanel.HighlightSpeaker(line.speakerId);
                break;
            case DialogType.PlaySfx:
                // 효과음 재생 기능
                float delay = audioManager.PlaySfx(line.sfxKey);
                await UniTask.Delay((int)delay*1000);
                break;
        }
    }

    // 대화 UI 종료
    private async UniTask EndDialog()
    {
        nameField.text = "";
        dialogText.text = "";
        
        await DialogFadeOut();
        await portraitUIPanel.InitializeUI();
        
        Time.timeScale = 1f;
        isSkip = false;
        isTyping = false;
    }
    
    // 배경 변경
    private async UniTask ChangeBackGround(BackgroundType type)
    {
        var container = Resources.Load<SpriteContainer>($"Image/Background/{type}");
        
        // 처음 변경하는 경우
        if (!backGroundPanel.gameObject.activeSelf)
        {
            backgroundImage.sprite = container.sprite;
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        // 대화 중간 변경하는 경우
        else
        {
            await FadeOutBackUI();
            backgroundImage.sprite = container.sprite;
            await FadeInBackUI();
        }
    }

    // 대화 창 UI 페이드 인
    private async UniTask DialogFadeIn()
    {
        backGroundPanel.SetActive(true);
        await FadeInBackUI();
        
        await UniTask.Yield(PlayerLoopTiming.Update);
    }

    // 대화 창 UI 페이드아웃
    private async UniTask DialogFadeOut()
    {
        await FadeOutBackUI();
        backGroundPanel.SetActive(false);
        
        await UniTask.Yield(PlayerLoopTiming.Update);
    }

    private async UniTask FadeInBackUI()
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = originFadeImgColor;
        
        await UniTask.Yield(PlayerLoopTiming.Update);
        
        await fadeImage
            .DOColor(fadedImgColor, 1f)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .AsyncWaitForCompletion()
            .AsUniTask();
        
        fadeImage.gameObject.SetActive(false);
    }

    private async UniTask FadeOutBackUI()
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = fadedImgColor;
        
        
        await  UniTask.Yield(PlayerLoopTiming.Update);
        
        await fadeImage.DOColor(originFadeImgColor, 1f)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .AsyncWaitForCompletion()
            .AsUniTask();
        
        
        fadeImage.gameObject.SetActive(false);
    }
    
    // 대화 스킵에 쓰이는 함수. 버튼에 달림
    private void DialogSkip()
    {
        isSkip = true;
    }
    #endregion
}
