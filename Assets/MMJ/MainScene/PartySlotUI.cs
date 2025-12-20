using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PartySlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;

    [Header("Slot")]
    [SerializeField] private int slotIndex; // 0,1,2 (프리팹에 숫자만 세팅)

    public int SlotIndex => slotIndex;

    // 외부(PartyUI)가 구독할 클릭 이벤트
    public event Action<int> OnSlotClicked;

    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
        if (btn == null)
        {
            Debug.LogError("[PartySlotUI] Button 컴포넌트가 없습니다.");
            return;
        }

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => OnSlotClicked?.Invoke(slotIndex));

        ClearSlot();
    }

    public void SetCharacter(string characterName, Sprite characterIcon)
    {
        if (nameText != null)
        {
            nameText.enabled = true;
            nameText.text = characterName;
        }

        if (icon != null)
        {
            icon.enabled = true;
            icon.sprite = characterIcon;
        }
    }

    public void ClearSlot()
    {
        if (nameText != null)
        {
            nameText.text = "";
            nameText.enabled = false;
        }

        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }
    }
}
