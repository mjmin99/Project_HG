using UnityEngine;
using UnityEngine.UI;

public class PartyUI : MonoBehaviour
{
    public PartySlotUI[] slots; // 0~2 슬롯

    private int activeSlotIndex = -1;

    private void Start()
    {
        LoadParty();
    }


    public void SelectSlot(int index)
    {
        activeSlotIndex = index;
        Debug.Log($"[PartyUI] {index}번 슬롯 선택됨");
    }

    public void AssignCharacter(int characterId)
    {
        if (activeSlotIndex == -1)
        {
            Debug.LogWarning("슬롯이 선택되지 않았습니다!");
            return;
        }

        // 기존 파티에서 찾기
        int prevIndex = FindCharacterInParty(characterId);

        if (prevIndex != -1)
        {
            SaveManager.Instance.CurrentData.partySet[prevIndex] = -1;
            slots[prevIndex].ClearSlot();
        }

        // 선택된 슬롯에 배치
        SaveManager.Instance.CurrentData.partySet[activeSlotIndex] = characterId;
        slots[activeSlotIndex].SetCharacter(characterId);

        Debug.Log($"캐릭터 {characterId} → 슬롯 {activeSlotIndex}로 교체됨");

        SaveManager.Instance.SaveCurrentUser();
    }


    /* 이전 AssignCharacter(int characterId) 함수
    // 캐릭터 할당 함수 (모든 파티 배치 로직은 여기서만!)
    public void AssignCharacter(int characterId)
    {
        Debug.Log($"[PartyUI] 캐릭터 {characterId} 선택됨");

        // 이미 파티에 들어가 있는지 확인
        int previousIndex = FindCharacterInParty(characterId);

        // 만약 이전에 들어가 있던 슬롯이 있다면 제거
        if (previousIndex != -1)
        {
            Debug.Log($"[PartyUI] 캐릭터 {characterId} 이미 {previousIndex}번 슬롯에 있던 것을 제거함");
            SaveManager.Instance.CurrentData.partySet[previousIndex] = -1;
            slots[previousIndex].ClearSlot();
        }

        // 빈 슬롯 찾기
        int emptyIndex = FindEmptySlot();

        if (emptyIndex == -1)
        {
            Debug.LogWarning("[PartyUI] 빈 슬롯이 없습니다!");
            return;
        }

        // 빈 슬롯에 캐릭터 넣기
        SaveManager.Instance.CurrentData.partySet[emptyIndex] = characterId;
        slots[emptyIndex].SetCharacter(characterId);

        Debug.Log($"[PartyUI] 캐릭터 {characterId} → {emptyIndex}번 슬롯에 배치됨");

        // Firebase 저장
        SaveManager.Instance.SaveCurrentUser();
    }
    */
    private int FindCharacterInParty(int characterId)
    {
        var party = SaveManager.Instance.CurrentData.partySet;

        for (int i = 0; i < party.Length; i++)
        {
            if (party[i] == characterId)
                return i;
        }
        return -1;
    }

    private int FindEmptySlot()
    {
        var party = SaveManager.Instance.CurrentData.partySet;

        for (int i = 0; i < party.Length; i++)
        {
            if (party[i] == -1)
                return i;
        }
        return -1;
    }

    // 저장된 파티 불러오기
    public void LoadParty()
    {
        var party = SaveManager.Instance.CurrentData.partySet;

        for (int i = 0; i < slots.Length; i++)
        {
            if (party[i] == -1)
            {
                slots[i].ClearSlot();
            }
            else
            {
                slots[i].SetCharacter(party[i]);
            }
        }
    }
}
