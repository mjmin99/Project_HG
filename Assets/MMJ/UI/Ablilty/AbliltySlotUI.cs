using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public Button lockButton;
    public GameObject lockIcon;

    private int slotIndex;
    private CharacterInstance inst;
    private CharacterModel model;
    private System.Action onChanged;

    private System.Action onToggleLock;

    public void Bind(
        CharacterModel model,
        CharacterInstance inst,
        int index,
        System.Action onChanged)
    {
        this.model = model;
        this.inst = inst;
        this.slotIndex = index;
        this.onChanged = onChanged;

        lockButton.onClick.RemoveAllListeners();
        lockButton.onClick.AddListener(OnClickLock);

        Refresh();
    }

    public void SetAbility(AbilityInstance ability, bool isLocked)
    {
        icon.enabled = true;
        icon.sprite = AbilityIconProvider.GetIcon(ability.abilityId);
        nameText.text = ability.abilityId.ToString();
        lockIcon.SetActive(isLocked);
    }

    public void SetEmptySlot()
    {
        icon.enabled = false;
        nameText.text = "Empty";
        lockIcon.SetActive(false);
    }

    public void SetLockedSlot()
    {
        icon.enabled = false;
        nameText.text = "Locked";
        lockIcon.SetActive(true);
    }

    public void SetOnLockToggle(System.Action action)
    {
        onToggleLock = action;
    }

    public void Refresh()
    {
        int unlocked = inst.GetUnlockedAbilitySlotCount(model);

        // 아직 해방되지 않은 슬롯
        if (slotIndex >= unlocked)
        {
            icon.enabled = false;
            nameText.text = "Locked";
            lockIcon.SetActive(true);
            lockButton.interactable = false;
            return;
        }

        var slot = inst.abilitySlots[slotIndex];

        lockButton.interactable = true;
        lockIcon.SetActive(slot.isLocked);

        if (slot.ability == null)
        {
            icon.enabled = false;
            nameText.text = "Empty";
        }
        else
        {
            icon.enabled = true;
            icon.sprite = AbilityIconProvider.GetIcon(slot.ability.abilityId);
            nameText.text = slot.ability.abilityId.ToString();
        }
    }

    private void OnClickLock()
    {
        var slot = inst.abilitySlots[slotIndex];
        slot.isLocked = !slot.isLocked;

        onChanged?.Invoke();
    }
}
