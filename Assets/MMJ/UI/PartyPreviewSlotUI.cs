using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyPreviewSlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;

    public void SetEmpty()
    {
        icon.enabled = false;
        nameText.text = "비어 있음";
    }

    public void SetCharacter(string characterName, Sprite sprite)
    {
        icon.enabled = true;
        icon.sprite = sprite;
        nameText.text = characterName;
    }
}
