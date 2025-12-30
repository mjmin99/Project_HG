using UnityEngine;
using DG.Tweening;

public abstract class UIBase : MonoBehaviour
{
    public abstract UIType Type { get; }
    public virtual bool CanCloseByESC => true;

    protected RectTransform rect;
    protected CanvasGroup canvasGroup;
    [SerializeField] protected float animDuration = 0.25f;
    protected Sequence sequence;

    private bool _isClosing = false;

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
        Manager.Audio.PlaySfx("SFX_Ok");
    }

    public virtual void OnClose()
    {
        if (_isClosing) return;
        _isClosing = true;
        PlayCloseAnimation();
        Manager.Audio.PlaySfx("SFX_Cancel");
    }

    protected virtual void PlayOpenAnimation()
    {
        KillSequence();

        if (canvasGroup == null) return;

        canvasGroup.alpha = 0f;
        sequence = DOTween.Sequence()
            .Append(canvasGroup.DOFade(1f, animDuration)).SetUpdate(true);
    }

    protected virtual void PlayCloseAnimation()
    {
        KillSequence();

        if (canvasGroup == null)
        {
            Destroy(gameObject);
            return;
        }

        sequence = DOTween.Sequence()
            .Append(canvasGroup.DOFade(0f, animDuration))
            .SetUpdate(true)
            .OnComplete(() =>
            {
                Destroy(gameObject);
            });
    }

    protected void KillSequence()
    {
        if (sequence != null && sequence.IsActive())
        {
            sequence.Kill();
            sequence = null;
        }
    }

    protected virtual void OnDestroy()
    {
        KillSequence();
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