using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterItemUI : MonoBehaviour
{
    [Header("UI")]
    public Image icon;
    public TMP_Text nameText;

    [Header("Buttons")]
    public Button btnInfo;
    public Button btnAssign;

    private int characterId;
    private PartySetupPanel partySetupPanel;

    public void Setup(int id, PartySetupPanel setupPanel)
    {
        characterId = id;
        partySetupPanel = setupPanel;

        var model = CharacterManager.Instance.models[id];
        nameText.text = model.characterName;
        icon.sprite = model.Icon;

        btnInfo.onClick.RemoveAllListeners();
        btnAssign.onClick.RemoveAllListeners();

        // 정보 버튼
        btnInfo.onClick.AddListener(() =>
        {
            var panel = UIManager.Instance
                .OpenUI<CharacterDetailPanel>("CharacterDetailPanel");
            panel.SetCharacter(characterId);
        });

        // 편성 버튼 → PartySetupPanel에게만 알림
        btnAssign.onClick.AddListener(() =>
        {
            partySetupPanel.AssignSelectedCharacter(characterId);
        });
    }
}
