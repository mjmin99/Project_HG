using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartySlotPreviewUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;

    public void SetEmpty()
    {
        icon.enabled = false;
        nameText.text = "비어 있음";
    }

    public void SetCharacter(string name, Sprite sprite)
    {
        icon.enabled = true;
        icon.sprite = sprite;
        nameText.text = name;
    }
}
