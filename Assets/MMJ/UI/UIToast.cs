using TMPro;
using UnityEngine;
using DG.Tweening;

public class UIToast : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private RectTransform visualRoot; // ⭐ 핵심

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (visualRoot == null)
            visualRoot = transform as RectTransform;

        canvasGroup = visualRoot.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = visualRoot.gameObject.AddComponent<CanvasGroup>();
    }

    public void Show(string message, float duration)
    {
        messageText.text = message;

        visualRoot.anchoredPosition += Vector2.down * 30;
        canvasGroup.alpha = 0;

        Sequence seq = DOTween.Sequence();
        seq.Append(visualRoot.DOAnchorPosY(
            visualRoot.anchoredPosition.y + 30, 0.2f));
        seq.Join(canvasGroup.DOFade(1, 0.2f));
        seq.AppendInterval(duration);
        seq.Append(canvasGroup.DOFade(0, 0.2f));
        seq.OnComplete(() => Destroy(gameObject));
    }
}
