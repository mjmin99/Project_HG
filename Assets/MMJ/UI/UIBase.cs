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
        rect.localScale = Vector3.one * 0.9f;
        canvasGroup.alpha = 0f;

        Sequence seq = DOTween.Sequence();

        seq.Append(rect.DOScale(1f, 0.2f).SetEase(Ease.OutBack));
        seq.Join(canvasGroup.DOFade(1f, 0.15f));
    }

    protected virtual void PlayCloseAnimation()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(rect.DOScale(0.9f, 0.15f));
        seq.Join(canvasGroup.DOFade(0f, 0.15f));
        seq.OnComplete(() =>
        {
            Destroy(gameObject);
        });
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