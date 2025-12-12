using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultPanelController : MonoBehaviour
{
    public GameObject panel;
    public Image icon;
    public TMP_Text nameText;
    public GameObject rarityGroup;
    public GameObject newBadge;
    public Button okButton;

    private void Awake()
    {
        okButton.onClick.AddListener(Hide);
    }

    public void Show(CharacterModel model, bool isNew)
    {
        panel.SetActive(true);

        // 아이콘 설정
        icon.sprite = model.Icon;

        // 이름 설정
        nameText.text = model.name;

        // 별 표시
        int rarity = model.rarity;
        for (int i = 0; i < rarityGroup.transform.childCount; i++)
        {
            rarityGroup.transform.GetChild(i).gameObject
                .SetActive(i < rarity);
        }

        // NEW 표시
        newBadge.SetActive(isNew);
    }

    public void Hide()
    {
        panel.SetActive(false);

        // 메인씬/상점 캐릭터 리스트 갱신 필요할 때:
        // FindObjectOfType<CharacterListUI>().Refresh();
    }
}
