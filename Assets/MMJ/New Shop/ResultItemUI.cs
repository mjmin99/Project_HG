using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultItemUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public GameObject rarityGroup; // 별 5개 자식
    public GameObject newBadge;

    public void Set(CharacterModel model, bool isNew)
    {
        icon.sprite = model.Icon;
        nameText.text = model.characterName;

        int rarity = model.rarity;
        for (int i = 0; i < rarityGroup.transform.childCount; i++)
            rarityGroup.transform.GetChild(i).gameObject.SetActive(i < rarity);

        newBadge.SetActive(isNew);
    }
}
