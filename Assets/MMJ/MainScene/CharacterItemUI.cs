using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterItemUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public Button selectButton;

    private int characterId;

    public void Set(int id)
    {
        this.characterId = id;
        var model = CharacterManager.Instance.models[id];

        nameText.text = model.name;

        Sprite sp = Resources.Load<Sprite>($"Icons/{model.name}");

        icon.sprite = sp;

        selectButton.onClick.AddListener(() =>
        {
            FindFirstObjectByType<PartyUI>().AssignCharacter(characterId);
        });
    }

    private void OnSelected()
    {
        // PartyUI 찾아서 전달
        var partyUI = FindFirstObjectByType<PartyUI>();
        if (partyUI != null)
        {
            partyUI.AssignCharacter(characterId);
        }
    }
}
