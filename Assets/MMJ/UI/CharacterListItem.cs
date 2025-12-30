using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterListItem : MonoBehaviour
{
    private int characterId;

    [SerializeField] private Button btn;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text levelText;
    public void Setup(int id)
    {
        characterId = id;
        var model = Manager.Character.models[characterId];
        var inst = Manager.Character.instances[id];
        if (nameText != null) nameText.text = model.characterName;
        if (icon != null) icon.sprite = model.Icon;

        if (levelText != null)
            levelText.text = $"Lv.{inst.level}";
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        var panel = UIManager.Instance.OpenUI<CharacterDetailPanel>("CharacterDetailPanel");
        panel.SetCharacter(characterId);
    }
}
