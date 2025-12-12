using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartySlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;

    public void SetCharacter(int id)
    {
        var model = CharacterManager.Instance.models[id];

        nameText.text = model.name;

        // 스프라이트 로드 방식 변경
        Sprite sp = model.Icon;
        //Sprite sp = Resources.Load<Sprite>($"Icons/{model.name}");

        icon.sprite = sp;
        gameObject.SetActive(true);
    }

    public void ClearSlot()
    {
        nameText.text = "";
        icon.sprite = null;
        gameObject.SetActive(true);
    }
}
