using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterItemUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Button btnInfo;

    private int characterId;

    public void Setup(int id)
    {
        characterId = id;

        var model = Manager.Character.models[id];
        nameText.text = model.characterName;
        icon.sprite = model.Icon;

        btnInfo.onClick.RemoveAllListeners();
        btnInfo.onClick.AddListener(OnClickInfo);
    }

    private void OnClickInfo()
    {
        var panel = UIManager.Instance
            .OpenUI<CharacterDetailPanel>("CharacterDetailPanel");

        panel.SetCharacter(characterId);
    }
}
