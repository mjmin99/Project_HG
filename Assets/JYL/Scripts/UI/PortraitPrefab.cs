using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PortraitPrefab : MonoBehaviour
{
    [SerializeField] private Image portraitImage;

    private Image[] images;
    public string speakerID;
    private readonly Color originColor = Color.white;
    private readonly Color dimColor = Color.dimGray;

    public void Init(Sprite sprite, string speaker)
    {
        this.speakerID = speaker; 
        images = GetComponentsInChildren<Image>();
        portraitImage.sprite = sprite;
    }

    public async UniTask FadeInPortrait(float duration = 0.3f)
    {
        var list = new List<UniTask>();
        foreach (var image in images)
        {
            list.Add(image.FadeInImage(duration).SetUpdate(true).AsyncWaitForCompletion().AsUniTask());
        }
        await UniTask.WhenAll(list);
    }

    public void FadeOutPortrait(float duration = 0.3f)
    {
        foreach (var image in images)
        {
            image.FadeOutImage(duration).SetUpdate(true);
        }
    }

    public void HighlightIn()
    {
        
        portraitImage.DOColor(dimColor,0.2f).SetUpdate(true);
    }

    public void HighlightOut()
    {
        portraitImage.DOColor(originColor, 0.2f).SetUpdate(true);
    }
}
