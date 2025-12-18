using UnityEngine;
using DG.Tweening;

public abstract class UIBase : MonoBehaviour
{
    public abstract UIType Type { get; }
    public virtual bool CanCloseByESC => true;

    protected RectTransform rect;
    protected CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        rect = transform as RectTransform;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public virtual void OnOpen()
    {
        PlayOpenAnimation();
    }

    public virtual void OnClose()
    {
        PlayCloseAnimation();
    }

    protected virtual void PlayOpenAnimation()
    {
        // 시작 상태: 완전 투명 + 살짝 작게
        rect.localScale = Vector3.one * 0.85f;
        canvasGroup.alpha = 0f;

        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true); // TimeScale 무시

        // 동시에 커지고, 동시에 나타남
        seq.Append(rect.DOScale(1f, 0.18f).SetEase(Ease.OutCubic));
        seq.Join(canvasGroup.DOFade(1f, 0.18f));
    }

    protected virtual void PlayCloseAnimation()
    {
        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);

        seq.Append(rect.DOScale(0.9f, 0.15f).SetEase(Ease.InCubic));
        seq.Join(canvasGroup.DOFade(0f, 0.15f));
        seq.OnComplete(() => Destroy(gameObject));
    }
}

public enum UIType
{
    Panel,
    Popup
}

public class UIPanel : UIBase
{
    public override UIType Type => UIType.Panel;
}

public class UIPopup : UIBase
{
    public override UIType Type => UIType.Popup;
}