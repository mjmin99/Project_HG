using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DamageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text damageTextPrefab;
    [SerializeField] private int poolSize = 30; // 오브젝트 풀링. 풀링 갯수
    [SerializeField] private float fontSize = 0.15f;
    [SerializeField] private Transform returnPool;

    private readonly Stack<TMP_Text> textPool = new();
    private readonly Dictionary<Transform, RectTransform> parentCanvas = new();
    public void Init()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateTextInstance();
        }
    }

    private void CreateTextInstance()
    {
        var newText = Instantiate(damageTextPrefab, returnPool);
        newText.gameObject.SetActive(false);
        textPool.Push(newText);
    }
    
    public async UniTask ShowDamageEffect(int damage, Transform targetTransform, bool isPlayerHit, bool isCritical)
    {
        if(textPool.Count == 0) CreateTextInstance();
        
        var newText = textPool.Pop();
        var childCanvas = CheckChildCanvas(targetTransform);
        newText.transform.SetParent(childCanvas);
        
        newText.gameObject.SetActive(true);
        newText.SetText($"{damage}");// damage.ToString(); ...(x)
                                     // SetText가 가비지가 더 적음
        // 인스턴스 초기화
        newText.alpha = 1f;
        if (isCritical)
        {
            newText.color = Color.red;
            newText.fontSize *= 1.2f;
        }
        else
        {
            newText.color = Color.white;
            newText.fontSize = fontSize;
        }
        newText.transform.localScale = Vector3.one;

        // 위치 값 가져오기. canvas가 적마다 달려있으므로 상관없음
        newText.rectTransform.anchoredPosition = Vector2.zero;
        
        // 랜덤 오프셋 설정
        float randX = isPlayerHit 
            ? Random.Range(-0.4f, -0.1f) 
            : Random.Range(0.1f, 0.4f); // UnityEngine.Random이 더 간편하면서 성능상 문제 없음
        Vector2 endPos = new Vector2(randX, 0.3f);
        
        // 이전 Tween이 실행 중이라면 제거
        newText.DOKill();
        
        Sequence seq = DOTween.Sequence();
        
        // Join : 현재 실행중인 Tween에 상관없이 추가함. 즉, 동시재생
        // Append : 현재 실행 중인 Tween이 종료되면 이어서 수행
        // Insert : 현재 실행 중인 Tween과 상관없이, 일정 시간이 지나면 시작함
        seq.Join(newText.rectTransform
            .DOJumpAnchorPos(endPos,0.75f,1,1.2f)
            .SetEase(Ease.OutBounce));
        seq.Insert(1f,newText
            .DOFade(0f, 0.2f));
        
        Manager.Game.tasks.Add(seq.AsyncWaitForCompletion().AsUniTask());
        await seq.AsyncWaitForCompletion();
        if (newText.gameObject != null)
        {
            newText.gameObject.SetActive(false);
            newText.transform.SetParent(returnPool);
            textPool.Push(newText);
        }
    }

    public async UniTask ShowHealEffect(int amount, Transform targetTransform)
    {
        if(textPool.Count == 0) CreateTextInstance();
        
        var newText = textPool.Pop();
        var childCanvas = CheckChildCanvas(targetTransform);
        newText.transform.SetParent(childCanvas);
        
        newText.gameObject.SetActive(true);
        newText.SetText($"{amount}");
        newText.alpha = 1f;
        newText.color = Color.greenYellow;
        newText.fontSize = fontSize;
        newText.transform.localScale = Vector3.one;
        
        // 위치 값 가져오기. canvas가 적마다 달려있으므로 상관없음
        newText.rectTransform.anchoredPosition = Vector2.zero;
        
        Vector2 endPos = new Vector2(-0.1f, 0.3f);
        
        // 이전 Tween이 실행 중이라면 제거
        newText.DOKill();
        
        Sequence seq = DOTween.Sequence();
        
        // Join : 현재 실행중인 Tween에 상관없이 추가함. 즉, 동시재생
        // Append : 현재 실행 중인 Tween이 종료되면 이어서 수행
        // Insert : 현재 실행 중인 Tween과 상관없이, 일정 시간이 지나면 시작함
        seq.Join(newText.rectTransform
            .DOAnchorPos(endPos,0.75f)
            .SetEase(Ease.OutCubic));
        seq.Insert(1f,newText
            .DOFade(0f, 0.2f));
        
        Manager.Game.tasks.Add(seq.AsyncWaitForCompletion().AsUniTask());
        
        await seq.AsyncWaitForCompletion();
        
        if (newText.gameObject)
        {
            newText.gameObject.SetActive(false);
            newText.transform.SetParent(returnPool);
            textPool.Push(newText);
        }
    }

    public RectTransform CheckChildCanvas(Transform targetTransform)
    {
        if (parentCanvas.TryGetValue(targetTransform, out var canvas)) return canvas;
        
        var targetCanvas = targetTransform.GetComponentInChildren<Canvas>();
        
        if (targetCanvas != null)
        {
            parentCanvas.Add(targetTransform, targetCanvas.GetComponent<RectTransform>());
            return parentCanvas[targetTransform];
        }
            
        GameObject go = new GameObject("DamageUI");
        go.transform.SetParent(targetTransform);
        
        var newCanvas = go.AddComponent<Canvas>();
        newCanvas.renderMode = RenderMode.WorldSpace;
        
        var tr = go.GetComponent<RectTransform>();
        tr.anchoredPosition = Vector2.up * 0.25f;
        parentCanvas.Add(targetTransform, tr);
        
        return tr;
    }
}
