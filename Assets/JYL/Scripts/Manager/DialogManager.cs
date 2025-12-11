using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    /// 대화 상황에 맞게 대화 전체 구조체를 딕셔너리로 저장
    /// 매니저에서 함수로 특정 상황에 대한 키를 넣으면, 딕셔너리에서 꺼내와서 수행
    /// 현재 출력중인 텍스트가 음성 재생을 필요로 하는 것인지 판단 필요. 파라매터 bool 추가
    /// 대화 시작 시 Time 스케일 0 으로 변경
    /// 대화 시작 시 대화 UI 패널 오픈
    /// 대화 중, 현재 화자에 맞게 일러스트 스프라이트 하이라이트 효과
    /// 대화에 화자 추가, 대화에 화자 나가는 것을 CSV string으로 판별함
    /// 
    /// 스킵 버튼을 사용하면 대화 종료
    [SerializeField] private string csvPath = "CSV/TestDialog";

    [SerializeField] private AudioManager audioManager;

    [Header("Set Values")] 
    [SerializeField][Range(0.01f, 0.1f)] private float outputTime = 0.04f;    
    
    // UI 관련
    [SerializeField] private GameObject backGroundPanel;
    [SerializeField] private GameObject characterContent;
    [SerializeField] private PortraitPrefab portraitPrefab;
    [SerializeField] private TMP_Text nameField;
    [SerializeField] private TMP_Text dialogText;

    // 텍스트 재생 변수
    private bool isTyping = false;
    private Tween typingTween = null;
    private bool skipTyping = false;
    
    // 다이얼로그 전체 딕셔너리
    private Dictionary<DialogKey, Dialog> dialogs;
    
    // 초상화 리스트
    private List<PortraitPrefab> portraitList = new();
    
    void Awake()
    {
        _ = GetDialogFromCsv();
    }

    private async UniTask GetDialogFromCsv()
    {
        dialogs = await Util.ParseCsvToDialogs(csvPath);
    }
    
    private Dialog GetDialog(DialogKey key)
    {
        if (!dialogs.TryGetValue(key, out Dialog result))
        {
            Debug.LogWarning($"해당 키 값에 대한 다이얼로그 없음: {key.ToString()}");
        }

        return result;
    }

    // 대화 씬 시작에 쓰이는 외부용 함수
    public async UniTask StartDialog(DialogKey key)
    {
        Dialog dialog = GetDialog(key);
        Time.timeScale = 0f;
        backGroundPanel.SetActive(true);
        foreach (DialogLine line in dialog.DialogContents)
        {
            await ProcessDialogLine(line); // 한 줄 처리
        }
    }
    
    // 대사 한 줄 한 줄 처리하기
    private async UniTask ProcessDialogLine(DialogLine line)
    {
        // 대사의 종류에 따른 처리
        TaskDialogLine(line);
        
        // 텍스트 출력
        if(!string.IsNullOrWhiteSpace(line.Content))
            await TypeText(line.Content);
    }

    private async UniTask TypeText(string content)
    {
        dialogText.text = content;
        dialogText.maxVisibleCharacters = 0;

        int total = content.Length;
        isTyping = true;
        skipTyping = false;

        typingTween = DOTween.To(
            () => dialogText.maxVisibleCharacters,
            x => dialogText.maxVisibleCharacters = x, 
            total,
            0.04f * total)
            .SetEase(Ease.Linear);

        // 타이핑 중 키 입력 감지
        while (isTyping)
        {
            
            // 스킵 : 트윈 종료
            if (Input.GetKeyDown(KeyCode.Space))
            {
                skipTyping = true;
                typingTween.Kill();
                dialogText.maxVisibleCharacters = total;
                isTyping = false;
                break;
            }
            
            // 트윈 종료 시
            if (!typingTween.active)
            {
                isTyping = false;
                break;
            }

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        
        // 타이핑 완료되면 다음 입력 기다리기
        await WaitNextKey();
    }

    private async UniTask WaitNextKey()
    {
        // 스킵된 경우 또한 포함
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }
    private void TaskDialogLine(DialogLine line)
    {
        switch (line.Type)
        {
            case DialogType.CharacterIn:
                // 현재 스프라이트 패널 UI에 요소로 신규 캐릭터 추가
                PortraitPrefab portrait = Instantiate(portraitPrefab, characterContent.transform);
                portrait.gameObject.SetActive(true);
                var container = Resources.Load<SpriteContainer>($"Image/{line.SpeakerId}");
                portrait.Init(container.sprite,line.SpeakerId);
                portraitList.Add(portrait);
                portrait.FadeInPortrait();
                break;
            case DialogType.CharacterOut:
                // 현재 스프라이트 패널 UI에서 해당 캐릭터 뺌
                dialogText.text = "";
                break;
            case DialogType.NoVoice:
                // 노 보이스는 나레이션임
                nameField.text = "";
                // 이름 부분 UI 끄기
                HighlightOff();
                break;
            case DialogType.WithVoice:
                // 보이스 찾아서 출력
                audioManager.PlayVoice(line.LineId);
                // 화자 이름 설정
                nameField.text = line.SpeakerId;
                // 화자 하이라이트
                HighlightSpeaker(line.SpeakerId);
                break;
        }
    }

    private void HighlightSpeaker(string speaker)
    {
        // 스프라이트 오브젝트 찾아서 색상 값 원래대로 변경. 나머지 스프라이트들은 반대로 색 낮춤

        foreach (var portrait in portraitList)
        {
            if (portrait.speakerID == speaker)
            {
                portrait.HighlightIn();
            }
            else
            {
                portrait.HighlightOut();
            }
        }
    }

    private void NoHighlight()
    {
        foreach (var portrait in portraitList)
        {
            portrait.HighlightOut();
        }
    }
    private void HighlightOff()
    {
        foreach (var portrait in portraitList)
        {
            portrait.HighlightOut();
        }
    }


    
}
