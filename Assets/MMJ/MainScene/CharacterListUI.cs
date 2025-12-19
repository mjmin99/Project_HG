using UnityEngine;

public class CharacterListUI : MonoBehaviour
{
    [Header("UI")]
    public Transform container;
    public GameObject itemPrefab;

    private PartyUI partyUI;
    private CharacterDetailPanel detailPanel;

    private void OnEnable()
    {
        Refresh();
    }

    /// <summary>
    /// 캐릭터 리스트 전체 갱신
    /// </summary>
    public void Refresh()
    {
        // 기존 아이템 제거
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        // 보유 캐릭터만 리스트에 표시
        foreach (var pair in CharacterManager.Instance.instances)
        {
            int id = pair.Key;
            var inst = pair.Value;

            if (!inst.isOwned)
                continue;

            GameObject go = Instantiate(itemPrefab, container);
            var itemUI = go.GetComponent<CharacterItemUI>();

            // 파티UI + 상세 패널 전달
            itemUI.Setup(id, partyUI, detailPanel);
        }
    }
}
