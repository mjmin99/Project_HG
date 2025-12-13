using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PortraitPrefab : MonoBehaviour
{
    [Header("Set Portrait Image")]
    [SerializeField] private Image portraitImage;

    private Image[] images;
    public string speakerID;
    private readonly Color originColor = Color.white;
    private readonly Color dimColor = Color.dimGray;

    // 초기화. UI 패널에서 관리
    public void Init(Sprite sprite, string speaker)
    {
        this.speakerID = speaker; 
        images = GetComponentsInChildren<Image>();
        portraitImage.sprite = sprite;
    }

    // 초상화 추가 시, Fade-IN 효과 추가
    public async UniTask FadeInPortrait(float duration = 0.3f)
    {
        var list = new List<UniTask>();
        foreach (var image in images)
        {
            list.Add(image.FadeInImage(duration).SetUpdate(true).AsyncWaitForCompletion().AsUniTask());
        }
        await UniTask.WhenAll(list);
    }

    // 초상화 삭제 시, Fade-OUT 효과 추가
    public async UniTask FadeOutPortrait(float duration = 0.3f)
    {
        var tasks =  new List<UniTask>();
        foreach (var image in images)
        {
            tasks.Add(image.FadeOutImage(duration).SetUpdate(true).AsyncWaitForCompletion().AsUniTask());
        }
        await UniTask.WhenAll(tasks);
    }
    
    // 대화 중 화자일 때 하이라이트
    public async UniTask HighlightIn()
    {
        await portraitImage.DOColor(originColor, 0.3f)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .AsyncWaitForCompletion()
            .AsUniTask();
    }

    // 대화 중 화자가 아닐 때 하이라이트 꺼짐
    public async UniTask HighlightOut()
    {
        await portraitImage
            .DOColor(dimColor,0.3f)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .AsyncWaitForCompletion()
            .AsUniTask();
    }
}