using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class PortraitUIPanel : MonoBehaviour
{
    [Header("Set References")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private PortraitPrefab portraitPrefab;

    [Header("Set Values")]
    [SerializeField] private float betweenGap = 50f;

    // 생성한 초상화들 관리
    private readonly List<RectTransform> portraits = new();
    private readonly List<PortraitPrefab> portraitsList = new();
    private readonly Dictionary<string, PortraitPrefab> portraitDict = new();
    
    // 출력하는 화면의 정보를 담음
    private CanvasScaler scaler;
    private Vector2 resolution;
    private RectTransform prefabRect;
    private float rectX;
    
    void Awake()
    {
        Init();
    }

    // 초기화
    private void Init()
    {
        scaler = GetComponentInParent<CanvasScaler>();
        resolution = scaler.referenceResolution;
        prefabRect = portraitPrefab.GetComponent<RectTransform>();
        // 생성한 프리팹의 UI 가로 길이
        rectX = prefabRect.rect.width;
    }

    // 대화 시작 시, 대화 UI들 초기화 작업
    public async UniTask InitializeUI()
    {
        portraitDict.Clear();
        foreach (var go in portraitsList)
        {
            DestroyImmediate(go.gameObject);
        }
        portraitsList.Clear();
        portraits.Clear();
        
        await UniTask.Yield(PlayerLoopTiming.Update);
    }
    
    // Portrait 추가 함수. 애니메이션 적용
    public async UniTask AddPortrait(string key)
    {
        // 프리팹 생성 및 컬렉션에 추가
        PortraitPrefab go = Instantiate(portraitPrefab, panel);
        SpriteContainer spriteContainer = Resources.Load<SpriteContainer>($"Image/{key}");
        go.Init(spriteContainer.sprite, key);
        var rect = go.GetComponent<RectTransform>();
        rect.localScale = Vector3.one * 0.2f;
        rect.gameObject.SetActive(false);
        portraits.Add(rect);
        portraitsList.Add(go);
        
        if (!portraitDict.TryAdd(key, go))
        {
            Debug.LogWarning($"이미 키에 대한 값이 있음: {key}");
        }

        var firstTargetPos = FindFirstTargetPosition();

        // 트윈 될 위치들의 리스트
        var targetPosList = GetTargetPositions(firstTargetPos);
        
        // 기존 Portrait들의 AnchoredPosition을 변경하여 애니메이션 효과
        if (portraits.Count > 1)
        {
            await TweenAllPortraitsTo(targetPosList);
        }
        
        // 추가할 요소의 위치를 아래로 미리 변경하여, 이동 애니메이션 효과 줌
        rect.anchoredPosition = targetPosList[^1] + new Vector2(0, -120);
        
        // 추가된 Portrait을 Tween
        rect.gameObject.SetActive(true);
        await TweenPortraitTo(targetPosList[^1], go);
    }
    
    // Portrait 삭제
    public async UniTask RemovePortrait(string key)
    {
        // key에 해당하는 go 찾은 후 애니메이션 처리
        if (portraits.Count == 0) return;
        if (!portraitDict.TryGetValue(key, out var go))
        {
            Debug.LogWarning($"{key}에 해당하는 프리팹이 딕셔너리에 없음");
            return;
        }
        var rt =  go.GetComponent<RectTransform>();

        await RemovePortraitAnimation(go, rt);
        
        // 딕셔너리, 리스트에서 해당 객체들 삭제
        portraits.Remove(rt);
        portraitsList.Remove(go);
        portraitDict.Remove(key);
        
        // 남은 애들 기준으로 위치 산정
        var targetPosList = GetTargetPositions(FindFirstTargetPosition());
        
        // 남은 애들 재배치 
        await TweenAllPortraitsTo(targetPosList,true);
    }

    // 현재 Portrait 위치들을 가져옴
    // private List<Vector2> GetCurrentPositions()
    // {
    //     var curPos = new List<Vector2>();
    //     foreach (var rect in portraits)
    //     {
    //         curPos.Add(rect.anchoredPosition);
    //     }
    //     return curPos;
    // }
    
    // 첫 번째 요소의 위치 및 나머지 요소들의 리스트를 찾는 함수
    private Vector2 FindFirstTargetPosition()
    {
        // 총 앵커 길이 : (가로 길이 * count - 1) + 50 * (count - 1)
        // UI의 가운데가 생성 포지션이기 때문에,
        // 좌우 반반 짤라서 하나의 UI 길이가 사라짐.
        // 그래서 Count - 1
        float fullLength = rectX * (portraits.Count - 1) + betweenGap * (portraits.Count - 1);

        // 남는 왼쪽 앵커 길이 : (화면 전체 길이 - 총길이) / 2
        float firstElementXPos = (resolution.x - fullLength) / 2;
        // 첫 요소의 앵커 포지션 = 남는 길이 + 350 / 2
        Vector2 firstTargetPos = new Vector2(firstElementXPos, 0);
        return firstTargetPos;
    }
    
    // 트윈할 위치의 정보들을 만들어서 가져옴
    private List<Vector2> GetTargetPositions(Vector2 firstPos)
    {
        var targetPositions = new List<Vector2> { firstPos };
        for (int i = 1; i < portraits.Count; i++)
        {
            // 첫 번째 요소의 위치를 기준으로 나머지 요소들의 위치를 만듦
            var pos = new Vector2(firstPos.x + i * (portraits[i].rect.width + betweenGap), firstPos.y);
            targetPositions.Add(pos);
        }
        return targetPositions;
    }

    // 경우에 따라 마지막 요소를 제외한 Portrait들의 위치를 조정함
    // includeLast로 전체 Portrait의 위치 조정 가능
    private async UniTask TweenAllPortraitsTo(List<Vector2> targetPositions, bool includeLast = false)
    {
        var tasks = new List<UniTask>();
        int index = includeLast ? portraits.Count : portraits.Count - 1;

        for (int i = 0; i < index; i++)
        {
            var rt = portraits[i];
            var target = targetPositions[i];

            Tweener t = rt.DOAnchorPos(target, 0.5f).SetUpdate(true).SetEase(Ease.OutBack);
            tasks.Add(t.SetUpdate(true).AsyncWaitForCompletion().AsUniTask());
        }

        await UniTask.WhenAll(tasks);
    }

    // 마지막에 추가된 Portrait의 애니메이션
    private async UniTask TweenPortraitTo(Vector2 targetPositions, PortraitPrefab go)
    {
        RectTransform rt = portraits[^1];
        var tasks = new List<UniTask>();
        
        Tweener t1 = rt.DOAnchorPos(targetPositions, 0.5f).SetUpdate(true).SetEase(Ease.OutElastic);
        tasks.Add(t1.SetUpdate(true).AsyncWaitForCompletion().AsUniTask());
        // 추가적인 연출(Scale-in, Fade-in)
        tasks.Add(rt.DOScale(1f, 0.3f).SetUpdate(true).SetEase(Ease.OutQuart).AsyncWaitForCompletion().AsUniTask());
        tasks.Add(go.FadeInPortrait());
        
        await UniTask.WhenAll(tasks);
    }

    // 삭제되는 Portrait의 애니메이션 적용
    private async UniTask RemovePortraitAnimation(PortraitPrefab go,RectTransform rt)
    {
        var tasks = new List<UniTask>();
        var targetPos = rt.anchoredPosition + new Vector2(0, rt.anchoredPosition.y - 120);
        tasks.Add(rt.DOAnchorPos(targetPos, 0.5f).SetUpdate(true).SetEase(Ease.InCirc).AsyncWaitForCompletion().AsUniTask());
        tasks.Add(rt.DOScale(0.2f, 0.3f).SetUpdate(true).SetEase(Ease.InQuart).AsyncWaitForCompletion().AsUniTask());
        tasks.Add(go.FadeOutPortrait());
        
        await UniTask.WhenAll(tasks);
    }
    
    // 화자만 하이라이트 처리
    public async UniTask HighlightSpeaker(string speaker)
    {
        // 스프라이트 오브젝트 찾아서 색상 값 원래대로 변경. 나머지 스프라이트들은 반대로 색 낮춤
        var  tasks = new List<UniTask>();
        foreach (var portrait in portraitsList)
        {
            tasks.Add(string.Equals(portrait.speakerID, speaker)
                ? portrait.HighlightIn() : portrait.HighlightOut());
        }
        await UniTask.WhenAll(tasks);
    }

    // 전체 하이라이트 OFF
    public async UniTask HighlightOff()
    {
        var tasks = new List<UniTask>();
        foreach (var portrait in portraitsList)
        {
            tasks.Add(portrait.HighlightOut());
        }
        await  UniTask.WhenAll(tasks);
    }
}
