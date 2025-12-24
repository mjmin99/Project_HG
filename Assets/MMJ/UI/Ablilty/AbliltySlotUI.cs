using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlotUI : MonoBehaviour
{
    public TMP_Text nameText;
    public Button lockButton;
    [SerializeField] private TMP_Text lockButtonText;
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
        nameText.text = ability.abilityId.ToString();
        lockIcon.SetActive(isLocked);
    }

    public void SetEmptySlot()
    {
        nameText.text = "Empty";
        lockIcon.SetActive(false);
    }

    public void SetLockedSlot()
    {
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
            nameText.text = "Locked";
            lockIcon.SetActive(true);
            lockButton.interactable = false;
            return;
        }

        var slot = inst.abilitySlots[slotIndex];

        lockButton.interactable = true;
        lockIcon.SetActive(slot.isLocked);

        RefreshLockButton(slot.isLocked);

        if (slot.ability == null)
        {
            nameText.text = "Empty";
        }
        else
        {
            nameText.text = AbilityNameProvider.GetName(slot.ability.abilityId);
        }
    }

    private void OnClickLock()
    {
        var slot = inst.abilitySlots[slotIndex];
        slot.isLocked = !slot.isLocked;

        onChanged?.Invoke();
    }

    private void RefreshLockButton(bool isLocked)
    {
        lockButtonText.text = isLocked ? "풀기" : "잠그기";
    }
}
