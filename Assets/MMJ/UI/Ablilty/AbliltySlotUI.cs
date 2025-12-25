using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlotUI : MonoBehaviour
{
    public TMP_Text nameText;
    public Button lockButton;

    [Header("Lock Button Sprites")]
    [SerializeField] private Sprite unlockSprite; 
    [SerializeField] private Sprite lockSprite;   

    private Image lockButtonImage;

    private int slotIndex;
    private CharacterInstance inst;
    private CharacterModel model;
    private System.Action onChanged;

    private static readonly Color LOCKED_COLOR = new Color(1f, 0.55f, 0f); // 주황색
    private static readonly Color UNLOCKED_COLOR = Color.white;


    private void Awake()
    {
        lockButtonImage = lockButton.GetComponent<Image>();
    }

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

    public void Refresh()
    {
        int unlocked = inst.GetUnlockedAbilitySlotCount(model);

        // 아직 해방되지 않은 슬롯
        if (slotIndex >= unlocked)
        {
            nameText.text = "Locked";
            lockButton.interactable = false;
            return;
        }

        var slot = inst.abilitySlots[slotIndex];

        lockButton.interactable = true;

        RefreshLockButtonImage(slot.isLocked);

        if (slot.ability == null)
            nameText.text = "Empty";
        else
            nameText.text = AbilityNameProvider.GetName(slot.ability.abilityId);
    }

    private void RefreshLockButtonImage(bool isLocked)
    {
        lockButtonImage.sprite = isLocked ? lockSprite : unlockSprite;
        lockButtonImage.color = isLocked ? LOCKED_COLOR : UNLOCKED_COLOR;
    }

    private void OnClickLock()
    {
        var slot = inst.abilitySlots[slotIndex];
        slot.isLocked = !slot.isLocked;

        onChanged?.Invoke(); // → 상위에서 Refresh 다시 호출
    }
}
