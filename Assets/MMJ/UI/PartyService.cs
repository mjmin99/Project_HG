using System;
using UnityEngine;

public static class PartyService
{
    // (partySet, changedSlotIndex, assignedCharacterId)
    public static event Action<int[], int, int> OnPartyChanged;

    public static int[] GetPartySet()
    {
        return Manager.Save.CurrentData.partySet;
    }

    /// <summary>
    /// 지정 슬롯에 캐릭터 배치.
    /// - 같은 캐릭터가 다른 슬롯에 있으면 제거
    /// - CurrentData.partySet 수정
    /// - 이벤트 발행
    /// - Firebase 저장 호출
    /// </summary>
    public static void AssignToSlot(int slotIndex, int characterId)
    {
        if (Manager.Save == null || Manager.Save.CurrentData == null)
        {
            Debug.LogError("[PartyService] SaveManager/CurrentData not ready.");
            return;
        }

        if (slotIndex < 0 || slotIndex >= 3)
        {
            Debug.LogError($"[PartyService] Invalid slotIndex: {slotIndex}");
            return;
        }

        var party = Manager.Save.CurrentData.partySet;

        // 이미 파티에 있는 캐릭터면 기존 슬롯 비우기 (중복 방지)
        for (int i = 0; i < party.Length; i++)
        {
            if (party[i] == characterId)
            {
                party[i] = -1;
                OnPartyChanged?.Invoke(party, i, -1);
            }
        }

        // 배치
        party[slotIndex] = characterId;

        // 이벤트 발행 (UI 즉시 갱신)
        OnPartyChanged?.Invoke(party, slotIndex, characterId);

        // 저장 (안 하면 재실행 시 원복됨)
        Manager.Save.SaveCurrentUser();
    }

    public static void ClearSlot(int slotIndex)
    {
        if (Manager.Save == null || Manager.Save.CurrentData == null)
            return;

        var party = Manager.Save.CurrentData.partySet;

        if (slotIndex < 0 || slotIndex >= party.Length)
            return;

        party[slotIndex] = -1;
        OnPartyChanged?.Invoke(party, slotIndex, -1);
        Manager.Save.SaveCurrentUser();
    }
}
