using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TestDamageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text damageTextPrefab;
    [SerializeField] private int poolSize = 10; // 오브젝트 풀링. 풀링 갯수
    
    private RectTransform parentCanvas;

    private Stack<TMP_Text> textPool = new();

    public void Init(RectTransform rectT)
    {
        parentCanvas = rectT;
        for (int i = 0; i < poolSize; i++)
        {
            CreateTextInstance();
        }
    }

    private void CreateTextInstance()
    {
        var newText = Instantiate(damageTextPrefab, parentCanvas);
        newText.gameObject.SetActive(false);
        textPool.Push(newText);
    }
    
    public async UniTaskVoid ShowDamageEffect(int damage)
    {
        if(textPool.Count == 0) CreateTextInstance();
        var newText = textPool.Pop();
        
        newText.gameObject.SetActive(true);
        newText.SetText($"{damage}");// damage.ToString(); ...(x)
                                     // SetText가 가비지가 더 적음
        // 인스턴스 초기화
        newText.alpha = 1f;
        newText.transform.localScale = Vector3.one;

        // 위치 값 가져오기. canvas가 적마다 달려있으므로 상관없음
        newText.rectTransform.anchoredPosition = Vector2.zero;
        
        // 랜덤 오프셋 설정
        float randX = Random.Range(0.1f, 0.4f); // UnityEngine.Random이 더 간편하면서 성능상 문제 없음
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
        await seq.AsyncWaitForCompletion();
        
        newText.gameObject.SetActive(false);
        textPool.Push(newText);
    }
}
