using UnityEngine;

public class CharacterListUI : MonoBehaviour
{
    [Header("UI")]
    public Transform container;
    public GameObject itemPrefab;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);

        foreach (var pair in CharacterManager.Instance.instances)
        {
            int id = pair.Key;
            var inst = pair.Value;
            if (!inst.isOwned) continue;

            GameObject go = Instantiate(itemPrefab, container);

            // item은 "캐릭터 선택"만 처리
            var itemUI = go.GetComponent<CharacterListItem>();
            itemUI.Setup(id);
        }
    }
}
