using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public GameObject lockIcon;

    private System.Action onToggleLock;

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

    public void OnClickLock()
    {
        onToggleLock?.Invoke();
    }
}
