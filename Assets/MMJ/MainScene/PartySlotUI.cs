using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartySlotUI : MonoBehaviour
{
    [Header("UI")]
    public Image icon;
    public TMP_Text nameText;

    [Header("슬롯 정보")]
    public int slotIndex;       // 0, 1, 2 이런 식으로 인스펙터에서 설정
    public PartyUI partyUI;     // 인스펙터에 연결 안 해도 자동으로 찾게 해둘거임

    private void Awake()
    {
        // PartyUI 자동 찾기 (인스펙터에 안 넣어도 동작하게)
        if (partyUI == null)
        {
            partyUI = FindFirstObjectByType<PartyUI>();
            if (partyUI == null)
            {
                Debug.LogError("[PartySlotUI] 씬에서 PartyUI를 찾을 수 없습니다!");
            }
        }

        // Button 컴포넌트 가져와서 클릭 이벤트 연결
        var btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnClickSlot);
        }
        else
        {
            Debug.LogWarning("[PartySlotUI] Button 컴포넌트가 없습니다.");
        }
    }

    private void OnClickSlot()
    {
        if (partyUI == null)
        {
            Debug.LogWarning("[PartySlotUI] partyUI가 null이라 슬롯 선택을 처리할 수 없습니다.");
            return;
        }

        partyUI.SelectSlot(slotIndex);
        Debug.Log($"[PartySlotUI] {slotIndex}번 슬롯 클릭됨 → PartyUI.SelectSlot 호출");
    }

    public void SetCharacter(int id)
    {
        var model = CharacterManager.Instance.models[id];

        nameText.enabled = true;
        icon.enabled = true;

        nameText.text = model.characterName;
        icon.sprite = model.Icon;
    }

    public void ClearSlot()
    {
        nameText.text = "";
        icon.sprite = null;

        nameText.enabled = false;
        icon.enabled = false;
    }
}
