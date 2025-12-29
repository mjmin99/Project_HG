using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DropSkill : MonoBehaviour
{
    private Image image;
    private RectTransform rectTransform;
    private Camera cam;
    
    public void Init(int index, Sprite sprite, Transform enemyTransform, BattleManager battleManager)
    {
        cam = Camera.main;
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        image.sprite = sprite;
        gameObject.SetActive(true);
        var targetTransform = battleManager.skillPresenter.skillButtonPanel[index].GetComponent<RectTransform>();
        
        Vector3 pos = Vector3.zero;
        if (cam)
        {
            pos = cam.WorldToScreenPoint(enemyTransform.position);
            Debug.Log($"시작 좌표: X :{pos.x} Y:{pos.y} Z:{pos.z} " +
                      $"\n 목표 좌표: X: {targetTransform.anchoredPosition.x} Y: {targetTransform.anchoredPosition.y}");
        }
        rectTransform.position = new Vector2(pos.x + 300, pos.y);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(rectTransform
            .DOJumpAnchorPos(targetTransform.anchoredPosition, 300f, 1, 2f));
        sequence.AppendCallback(() 
            => battleManager.GetSkill(index));
        sequence.AppendCallback(() 
            => Destroy(gameObject));
    }
}
