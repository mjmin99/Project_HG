using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PortraitPrefab : MonoBehaviour
{
    [SerializeField] private Image portraitImage;

    private Image[] images;
    public string speakerID;
    private Color originColor = Color.white;
    private Color dimColor = Color.dimGray;

    public void Init(Sprite sprite,  string speakerID)
    {
        this.speakerID = speakerID; 
        images = GetComponentsInChildren<Image>();
        portraitImage.sprite = sprite;
    }

    public void FadeInPortrait()
    {
        foreach (var image in images)
        {
            image.FadeInImage(1f);
        }
    }

    public void FadeOutPortrait()
    {
        foreach (var image in images)
        {
            image.FadeOutImage(1f);
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
