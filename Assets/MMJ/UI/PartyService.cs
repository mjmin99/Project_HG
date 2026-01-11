using System;
using UnityEngine;




public static class PartyService
{
    public static readonly CharacterRole[] SlotRoles =
    {
        CharacterRole.Tank,   // slot 0
        CharacterRole.Dealer, // slot 1
        CharacterRole.Healer  // slot 2
    };

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
            return;

        if (slotIndex < 0 || slotIndex >= 3)
            return;

        if (!Manager.Character.models.TryGetValue(characterId, out var model))
            return;

        var requiredRole = SlotRoles[slotIndex];

        // 직업 불일치 → 차단
        if (model.role != requiredRole)
        {
            ToastUtil.Error($"이 슬롯에는 {requiredRole}만 배치할 수 있어요");
            return;
        }

        var party = Manager.Save.CurrentData.partySet;

        // 중복 제거
        for (int i = 0; i < party.Length; i++)
        {
            if (party[i] == characterId)
            {
                party[i] = -1;
                OnPartyChanged?.Invoke(party, i, -1);
            }
        }

        party[slotIndex] = characterId;
        OnPartyChanged?.Invoke(party, slotIndex, characterId);

        Manager.Save.PatchPartySet();
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
        Manager.Save.PatchPartySet();
    }
}
