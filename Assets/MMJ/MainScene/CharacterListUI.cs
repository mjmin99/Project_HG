using UnityEngine;

public class CharacterListUI : MonoBehaviour
{
    public Transform container;
    public GameObject itemPrefab;

    private void Start()
    {
        Initialize();
    }


    // 가챠로 새 캐릭터를 얻은 후 UI 즉시 갱신
    // 유저가 다른 필터 버튼을 눌러서 정렬/검색할 때
    // 파티 배치 후 스탯 변동으로 UI 갱신 필요할 때와 같은 상황에서 초기화를 다시 해줄 필요가 있다고 판단해서 일단 이렇게 만듬
    public void Initialize()
    {
        PopulateList();
    }

    void PopulateList()
    {
        // 기존 UI 전부 제거
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        foreach (var pair in CharacterManager.Instance.instances)
        {
            int id = pair.Key;
            if (!pair.Value.isOwned) continue;

            GameObject obj = Instantiate(itemPrefab, container);
            obj.GetComponent<CharacterItemUI>().Set(id);
        }
    }
}
