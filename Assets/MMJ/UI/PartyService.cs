using System;
using UnityEngine;

public static class PartyService
{
    // UI들은 이 이벤트를 구독해서 파티 UI를 갱신한다.
    public static event Action<int[], int, int> OnPartyChanged;
    // (partySet, changedSlotIndex, assignedCharacterId)

    public static int[] GetPartySet()
    {
        // SaveManager는 "안정적으로 존재"한다고 했으니 여기서만 접근
        return SaveManager.Instance.CurrentData.partySet;
    }

    public static void AssignToSlot(int slotIndex, int characterId)
    {
        var party = SaveManager.Instance.CurrentData.partySet;
        party[slotIndex] = characterId;

        OnPartyChanged?.Invoke(party, slotIndex, characterId);
    }


    public static void ClearSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= 3) return;
        var party = GetPartySet();
        party[slotIndex] = -1;
        OnPartyChanged?.Invoke(party, slotIndex, -1);
    }
}
