using UnityEngine;
using System.Linq;

public class PartyUI : MonoBehaviour
{
    private PartySlotUI[] slots;

    private void Awake()
    {
        // Inspector 연결 대신 자식에서 자동 수집
        slots = GetComponentsInChildren<PartySlotUI>(true);

        if (slots == null || slots.Length == 0)
        {
            Debug.LogError("[PartyUI] PartySlotUI를 찾지 못했습니다.");
            return;
        }

        // slotIndex 기준으로 정렬 (0,1,2)
        slots = slots.OrderBy(s => s.SlotIndex).ToArray();

        // 슬롯 클릭 이벤트 연결
        foreach (var s in slots)
        {
            s.OnSlotClicked -= OnSlotClicked; // 중복 방지
            s.OnSlotClicked += OnSlotClicked;
        }
    }

    private void OnEnable()
    {
        PartyService.OnPartyChanged += HandlePartyChanged;
        RefreshAll();
    }

    private void OnDisable()
    {
        PartyService.OnPartyChanged -= HandlePartyChanged;
    }

    private void OnSlotClicked(int slotIndex)
    {
        Debug.Log($"[PartyUI] Slot {slotIndex} clicked");
        HighlightSlot(slotIndex); // 선택 표시만
    }

    private void HandlePartyChanged(int[] partySet, int changedSlot, int characterId)
    {
        RefreshSlot(changedSlot);
    }

    public void RefreshAll()
    {
        var party = PartyService.GetPartySet();

        for (int i = 0; i < slots.Length; i++)
        {
            ApplySlotVisual(i, party[i]);
        }
    }

    private void RefreshSlot(int slotIndex)
    {
        var party = PartyService.GetPartySet();
        ApplySlotVisual(slotIndex, party[slotIndex]);
    }

    private void ApplySlotVisual(int slotIndex, int characterId)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;

        var slot = slots[slotIndex];

        if (characterId == -1)
        {
            slot.ClearSlot();
            return;
        }

        if (!Manager.Character.models.TryGetValue(characterId, out var model))
        {
            Debug.LogWarning($"[PartyUI] 캐릭터 모델 없음: {characterId}");
            slot.ClearSlot();
            return;
        }

        slot.SetCharacter(model.characterName, model.Icon);
    }

    private void HighlightSlot(int index)
    {
        // 선택 강조 추가 위한 자리
    }
}
