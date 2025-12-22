using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanelController : MonoBehaviour
{
    [Header("Root")]
    public GameObject panel;
    public Button okButton;

    [Header("Optional Title")]
    public TMP_Text titleText;

    [Header("ScrollView")]
    public Transform content;          // ScrollView/Viewport/Content
    public ResultItemUI itemPrefab;    // 결과 셀 프리팹

    private readonly List<GameObject> spawned = new();

    private void Awake()
    {
        okButton.onClick.AddListener(Hide);
    }

    // 기존 단일 표시가 필요하면 유지 (내부적으로 ShowMany 사용)
    public void Show(CharacterModel model, bool isNew)
    {
        ShowMany(new List<GachaResult> { new GachaResult(model.id, isNew) });
    }

    public void ShowMany(List<GachaResult> results)
    {
        panel.SetActive(true);

        if (titleText != null)
            titleText.text = $"획득 결과 ({results.Count})";

        ClearItems();

        foreach (var r in results)
        {
            if (!CharacterManager.Instance.models.TryGetValue(r.characterId, out var model))
                continue;

            var item = Instantiate(itemPrefab, content);
            item.Set(model, r.isNew);
            spawned.Add(item.gameObject);
        }

        // 보기 좋게: 희귀도 높은 순 정렬하고 싶으면 여기서 정렬 가능
        // (지금은 "뽑힌 순서" 유지 추천)
    }

    private void ClearItems()
    {
        for (int i = 0; i < spawned.Count; i++)
            Destroy(spawned[i]);
        spawned.Clear();
    }

    public void Hide()
    {
        panel.SetActive(false);
        ClearItems();
    }
}
