using UnityEngine;

public class PartySetupPanel : UIPanel
{
    private PartyUI partyUI;
    private int selectedCharacterId = -1;

    protected override void Awake()
    {
        base.Awake();
        partyUI = GetComponentInChildren<PartyUI>();

        if (partyUI == null)
            Debug.LogError("[PartySetupPanel] PartyUI not found");
    }

    /// <summary>
    /// 캐릭터 선택 (CharacterItemUI에서 호출)
    /// </summary>
    public void SelectCharacter(int characterId)
    {
        selectedCharacterId = characterId;
        Debug.Log($"[PartySetupPanel] 캐릭터 선택됨: {characterId}");
    }

    /// <summary>
    /// 슬롯 클릭 (PartySlotUI에서 호출)
    /// </summary>
    public void OnPartySlotClicked(int slotIndex)
    {
        if (selectedCharacterId < 0)
        {
            Debug.LogWarning("[PartySetupPanel] 먼저 캐릭터를 선택하세요");
            return;
        }

        partyUI.SelectSlot(slotIndex);
        partyUI.AssignCharacter(selectedCharacterId);
        Debug.Log($"slot={slotIndex}, selectedChar={selectedCharacterId}");
    }

    public void AssignSelectedCharacter(int characterId)
    {
        if (partyUI == null)
        {
            Debug.LogError("[PartySetupPanel] PartyUI not found");
            return;
        }

        partyUI.AssignCharacter(characterId);
    }
}
