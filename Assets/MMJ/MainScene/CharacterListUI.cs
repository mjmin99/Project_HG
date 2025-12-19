using UnityEngine;

public class CharacterListUI : MonoBehaviour
{
    [Header("UI")]
    public Transform container;
    public GameObject itemPrefab;

    private PartySetupPanel partySetupPanel;

    private void Awake()
    {
        partySetupPanel = GetComponentInParent<PartySetupPanel>();

        if (partySetupPanel == null)
            Debug.LogError("[CharacterListUI] PartySetupPanel not found");
    }

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
            var itemUI = go.GetComponent<CharacterItemUI>();
            itemUI.Setup(id, partySetupPanel);
        }
    }
}
