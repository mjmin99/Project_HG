using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DamageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text damageTextPrefab;
    [SerializeField] private int poolSize = 30;
    [SerializeField] private float fontSize = 0.15f;
    [SerializeField] private Transform returnPool;

    private readonly Stack<TMP_Text> textPool = new();
    private readonly Dictionary<Transform, RectTransform> parentCanvas = new();

    public void Init()
    {
        // 중복 Init 방지
        if (damageTextPrefab == null)
        {
            Debug.LogError("[DamageUI] damageTextPrefab is null");
            return;
        }
        if (returnPool == null) returnPool = transform;

        for (int i = textPool.Count; i < poolSize; i++)
            CreateTextInstance();
    }

    private void CreateTextInstance()
    {
        var newText = Instantiate(damageTextPrefab, returnPool);
        newText.gameObject.SetActive(false);
        textPool.Push(newText);
    }

    public async UniTask ShowDamageEffect(int damage, Transform targetTransform, bool isPlayerHit, bool isCritical)
    {
        // 호출 시점에 대상이 이미 파괴된 경우 방어
        if (!targetTransform) return;
        if (!this || !gameObject) return;

        if (textPool.Count == 0) CreateTextInstance();
        var newText = textPool.Pop();

        // newText가 혹시 파괴돼있으면 재생성
        if (!newText)
        {
            CreateTextInstance();
            newText = textPool.Pop();
            if (!newText) return;
        }

        // "부모 캔버스"가 파괴된 경우 재생성하도록
        var childCanvas = CheckChildCanvas(targetTransform);
        if (!childCanvas)
        {
            ReturnToPoolSafe(newText);
            return;
        }

        newText.transform.SetParent(childCanvas, worldPositionStays: false);

        newText.gameObject.SetActive(true);
        newText.SetText($"{damage}");

        newText.alpha = 1f;
        newText.color = isCritical ? Color.red : Color.white;
        newText.fontSize = isCritical ? (fontSize * 1.2f) : fontSize; // *= 누적 버그 방지
        newText.transform.localScale = Vector3.one;
        newText.rectTransform.anchoredPosition = Vector2.zero;

        float randX = isPlayerHit ? Random.Range(-0.4f, -0.1f) : Random.Range(0.1f, 0.4f);
        Vector2 endPos = new Vector2(randX, 0.3f);

        // 이전 트윈 제거
        newText.DOKill();

        // 트윈을 newText에 링크 (오브젝트 Destroy되면 자동 Kill)
        Sequence seq = DOTween.Sequence();
        seq.SetLink(newText.gameObject, LinkBehaviour.KillOnDestroy);

        seq.Join(newText.rectTransform
            .DOJumpAnchorPos(endPos, 0.75f, 1, 1.2f)
            .SetEase(Ease.OutBounce));

        seq.Insert(1f, newText
            .DOFade(0f, 0.2f));

        // 씬 전환/패널 종료 등으로 DamageUI가 Destroy되면 await 즉시 취소
        var ct = this.GetCancellationTokenOnDestroy();

        try
        {
            var task = seq.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(ct);

            // 기존 코드에서 tasks에 넣는 로직 유지하되, 취소 가능한 Task만 넣기
            // Manager.Game.tasks.Add(task);

            await task;
        }
        catch (System.OperationCanceledException)
        {
            // 씬 전환/파괴로 취소된 경우 정상 흐름
        }
        finally
        {
            // 완료/취소 후에도 오브젝트가 살아있을 때만 만지기
            if (newText)
                ReturnToPoolSafe(newText);
        }
    }

    public async UniTask ShowHealEffect(int amount, Transform targetTransform)
    {
        if (!targetTransform) return;
        if (!this || !gameObject) return;

        if (textPool.Count == 0) CreateTextInstance();
        var newText = textPool.Pop();

        if (!newText)
        {
            CreateTextInstance();
            newText = textPool.Pop();
            if (!newText) return;
        }

        var childCanvas = CheckChildCanvas(targetTransform);
        if (!childCanvas)
        {
            ReturnToPoolSafe(newText);
            return;
        }

        newText.transform.SetParent(childCanvas, worldPositionStays: false);

        newText.gameObject.SetActive(true);
        newText.SetText($"{amount}");

        newText.alpha = 1f;
        newText.color = Color.greenYellow;
        newText.fontSize = fontSize;
        newText.transform.localScale = Vector3.one;
        newText.rectTransform.anchoredPosition = Vector2.zero;

        Vector2 endPos = new Vector2(-0.1f, 0.3f);

        newText.DOKill();

        Sequence seq = DOTween.Sequence();
        seq.SetLink(newText.gameObject, LinkBehaviour.KillOnDestroy);

        seq.Join(newText.rectTransform
            .DOAnchorPos(endPos, 0.75f)
            .SetEase(Ease.OutCubic));

        seq.Insert(1f, newText
            .DOFade(0f, 0.2f));

        var ct = this.GetCancellationTokenOnDestroy();

        try
        {
            var task = seq.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(ct);
            // Manager.Game.tasks.Add(task);
            await task;
        }
        catch (System.OperationCanceledException)
        {
            // 정상
        }
        finally
        {
            if (newText)
                ReturnToPoolSafe(newText);
        }
    }

    private void ReturnToPoolSafe(TMP_Text txt)
    {
        if (!txt) return;

        // 트윈 정리
        txt.DOKill();

        if (returnPool == null) returnPool = transform;

        txt.gameObject.SetActive(false);
        txt.transform.SetParent(returnPool, worldPositionStays: false);
        textPool.Push(txt);
    }

    public RectTransform CheckChildCanvas(Transform targetTransform)
    {
        // targetTransform이 파괴된 경우
        if (!targetTransform) return null;

        // 캐시된 RectTransform이 Destroy된 경우 캐시 제거
        if (parentCanvas.TryGetValue(targetTransform, out var cached))
        {
            if (cached) return cached;
            parentCanvas.Remove(targetTransform);
        }

        // 기존 Canvas가 있으면 그것 사용
        var targetCanvas = targetTransform.GetComponentInChildren<Canvas>();
        if (targetCanvas != null)
        {
            var rt = targetCanvas.GetComponent<RectTransform>();
            if (rt)
            {
                parentCanvas[targetTransform] = rt;
                return rt;
            }
        }

        // 없으면 새로 생성
        GameObject go = new GameObject("DamageUI");
        go.transform.SetParent(targetTransform, worldPositionStays: false);

        var newCanvas = go.AddComponent<Canvas>();
        newCanvas.renderMode = RenderMode.WorldSpace;

        var tr = go.GetComponent<RectTransform>();
        tr.anchoredPosition = Vector2.up * 0.25f;

        parentCanvas[targetTransform] = tr;
        return tr;
    }
}
