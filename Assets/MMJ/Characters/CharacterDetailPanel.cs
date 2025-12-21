using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterDetailPanel : UIPanel
{
    [Header("Header")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text roleText;

    [Header("Progress")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text shardText;

    [Header("Stats")]
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text atkText;
    [SerializeField] private TMP_Text matkText;
    [SerializeField] private TMP_Text defText;
    [SerializeField] private TMP_Text aspdText;
    [SerializeField] private TMP_Text critText;
    [SerializeField] private TMP_Text critDmgText;
    [SerializeField] private TMP_Text rangeText;

    [Header("Actions")]
    [SerializeField] private Button btnAssign;

    [Header("Abilities")]
    public Transform abilitySlotGroup;     // 슬롯 부모
    public GameObject abilitySlotPrefab;   // 슬롯 프리팹
    [SerializeField] private Button btnReroll;
    [SerializeField] private TMP_Text rerollCostText;

    private int currentCharacterId = -1;

    /// <summary>
    /// 외부에서 캐릭터 선택 시 호출
    /// </summary>
    public void SetCharacter(int characterId)
    {
        currentCharacterId = characterId;
        Refresh();
    }

    private void Refresh()
    {
        if (!CharacterManager.Instance.models.TryGetValue(currentCharacterId, out var model))
            return;

        if (!CharacterManager.Instance.instances.TryGetValue(currentCharacterId, out var inst))
            return;

        var stats = CharacterManager.Instance.GetStats(currentCharacterId);

        // ===== 기존 표시 로직 그대로 =====
        icon.sprite = model.Icon;
        nameText.text = model.characterName;
        roleText.text = model.role.ToString();

        levelText.text = $"Lv. {inst.level}";
        shardText.text = $"Shard: {inst.shard}";

        hpText.text = $"HP: {stats.hp:0}";
        atkText.text = $"ATK: {stats.attack:0}";
        matkText.text = $"MATK: {stats.magicAttack:0}";
        defText.text = $"DEF: {stats.defense:0}";
        aspdText.text = $"ASPD: {stats.attackSpeed:0.00}";
        critText.text = $"CRIT: {stats.critRate * 100:0.0}%";
        critDmgText.text = $"CRITDMG: {stats.critDamage * 100:0.0}%";
        rangeText.text = $"RANGE: {stats.attackRange:0.0}";

        RefreshAbilitySlots(model, inst); // 어빌리티 슬롯 생성
    }

    public override void OnOpen()
    {
        btnAssign.onClick.RemoveAllListeners();
        btnAssign.onClick.AddListener(OnClickAssign);

        btnReroll.onClick.RemoveAllListeners();
        btnReroll.onClick.AddListener(OnClickReroll);

        rerollCostText.text = "Cost: 10";
    }

    public override void OnClose()
    {
        currentCharacterId = -1;
        base.OnClose();
    }

    private void OnClickReroll()
    {
        if (currentCharacterId < 0)
            return;

        bool result = CharacterManager.Instance.TryRerollAbilities(currentCharacterId);

        if (!result)
        {
            Debug.Log("리롤 실패 (골드 부족 또는 오류)");
            return;
        }

        Refresh();
    }

    private void OnClickAssign()
    {
        if (currentCharacterId < 0)
        {
            Debug.LogWarning("[CharacterDetailPanel] characterId invalid");
            return;
        }

        PartyAssignmentContext.Begin(currentCharacterId);

        UIManager.Instance.OpenUI<PartySlotSelectPopup>("PartySlotSelectPopup");
    }

    private void RefreshAbilitySlots(CharacterModel model, CharacterInstance inst)
    {
        inst.SyncAbilitySlots(model);

        foreach (Transform child in abilitySlotGroup)
            Destroy(child.gameObject);

        int max = model.MaxAbilitySlotCount;

        for (int i = 0; i < max; i++)
        {
            var obj = Instantiate(abilitySlotPrefab, abilitySlotGroup);
            var ui = obj.GetComponent<AbilitySlotUI>();

            ui.Bind(model, inst, i, () =>
            {
                RefreshAbilitySlots(model, inst);
            });
        }
    }

    //void RefreshAbilitySlots(CharacterModel model, CharacterInstance inst)
    //{
    //    if (abilitySlotGroup == null || abilitySlotPrefab == null)
    //        return;

    //    // 슬롯 수 보정 (레벨 기준)
    //    inst.SyncAbilitySlots(model);

    //    foreach (Transform child in abilitySlotGroup)
    //        Destroy(child.gameObject);

    //    int maxSlots = model.MaxAbilitySlotCount;
    //    int unlockedSlots = inst.GetUnlockedAbilitySlotCount(model);

    //    for (int i = 0; i < maxSlots; i++)
    //    {
    //        var slotObj = Instantiate(abilitySlotPrefab, abilitySlotGroup);
    //        var slotUI = slotObj.GetComponent<AbilitySlotUI>();

    //        if (slotUI == null)
    //            continue;

    //        // 아직 레벨 부족 → 슬롯 자체가 잠김
    //        if (i >= unlockedSlots)
    //        {
    //            slotUI.SetLockedSlot();
    //            continue;
    //        }

    //        var slot = inst.abilitySlots[i];

    //        // 빈 슬롯
    //        if (slot.ability == null)
    //        {
    //            slotUI.SetEmptySlot();
    //        }
    //        else
    //        {
    //            slotUI.SetAbility(slot.ability, slot.isLocked);
    //        }

    //        // 슬롯 인덱스 전달 (잠금 토글용)
    //        int slotIndex = i;
    //        slotUI.SetOnLockToggle(() =>
    //        {
    //            slot.isLocked = !slot.isLocked;
    //            RefreshAbilitySlots(model, inst);
    //        });
    //    }
    //}
}
