using UnityEngine;

public class CharacterListPanel : UIPanel
{
    [Header("Party Preview (Top Slots)")]
    [SerializeField] private PartyPreviewSlotUI[] partyPreviewSlots; // size = 3

    private CharacterListUI listUI;

    public override void OnOpen()
    {
        base.OnOpen();

        listUI = GetComponentInChildren<CharacterListUI>(true);
        if (listUI == null)
        {
            Debug.LogError("[CharacterListPanel] CharacterListUI not found");
            return;
        }

        listUI.Refresh();

        RefreshPartyPreview();
    }

    private void OnEnable()
    {
        PartyService.OnPartyChanged += HandlePartyChanged;
    }

    private void OnDisable()
    {
        PartyService.OnPartyChanged -= HandlePartyChanged;
    }

    private void HandlePartyChanged(int[] partySet, int changedSlot, int characterId)
    {
        RefreshPartyPreview();
    }

    private void RefreshPartyPreview()
    {
        var party = PartyService.GetPartySet();

        for (int i = 0; i < partyPreviewSlots.Length; i++)
        {
            if (party[i] == -1)
            {
                partyPreviewSlots[i].SetEmpty();
            }
            else
            {
                var model = Manager.Character.models[party[i]];
                partyPreviewSlots[i].SetCharacter(
                    model.characterName,
                    model.Icon
                );
            }
        }
    }
}
