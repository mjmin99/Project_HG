using UnityEngine;
using UnityEngine.UI;

public class PartySlotSelectPopup : UIPopup
{
    [Header("Slot Buttons")]
    [SerializeField] private Button btnSlot1;
    [SerializeField] private Button btnSlot2;
    [SerializeField] private Button btnSlot3;

    [Header("Preview")]
    [SerializeField] private PartySlotPreviewUI[] previews; // size = 3

    public override void OnOpen()
    {
        base.OnOpen();

        if (!PartyAssignmentContext.HasPending)
        {
            Debug.LogWarning("[PartySlotSelectPopup] No pending character");
            UIManager.Instance.CloseTop();
            return;
        }

        BindButtons();
        RefreshPreview();
    }

    private void BindButtons()
    {
        btnSlot1.onClick.RemoveAllListeners();
        btnSlot2.onClick.RemoveAllListeners();
        btnSlot3.onClick.RemoveAllListeners();

        btnSlot1.onClick.AddListener(() => SelectSlot(0));
        btnSlot2.onClick.AddListener(() => SelectSlot(1));
        btnSlot3.onClick.AddListener(() => SelectSlot(2));
    }

    private void SelectSlot(int slotIndex)
    {
        int characterId = PartyAssignmentContext.PendingCharacterId;

        if (!Manager.Character.models.TryGetValue(characterId, out var model))
            return;

        var requiredRole = PartyService.SlotRoles[slotIndex];

        if (model.role != requiredRole)
        {
            ToastUtil.Error($"이 슬롯에는 {requiredRole}만 배치할 수 있어요");
            return;
        }

        PartyService.AssignToSlot(slotIndex, characterId);
        PartyAssignmentContext.Clear();

        // 1) 슬롯 선택 팝업 닫기
        UIManager.Instance.CloseTop();

        // 2) 디테일 패널도 같이 닫기
        UIManager.Instance.CloseTop();
    }

    private void RefreshPreview()
    {
        var party = PartyService.GetPartySet();

        for (int i = 0; i < previews.Length; i++)
        {
            if (party[i] == -1)
            {
                previews[i].SetEmpty();
            }
            else
            {
                var model = Manager.Character.models[party[i]];
                previews[i].SetCharacter(model.characterName, model.Icon);
            }
        }
    }
}
