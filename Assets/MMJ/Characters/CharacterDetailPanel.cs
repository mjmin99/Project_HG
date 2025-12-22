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

    [Header("Stars")]
    [SerializeField] private Image[] stars; // size = 5

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

    [Header("Exp")]
    [SerializeField] private Slider expSlider;
    [SerializeField] private TMP_Text expText;

    [Header("Enhance")]
    [SerializeField] private Button btnEnhance;
    [SerializeField] private TMP_Text enhanceCostText;

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

        RefreshStars(model);

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

        // 업데이트에 할 필요 없는 이유가 여기에 두면 슬롯 버튼 누를 때 호출됨
        int cost = GetRerollCost(inst);
        rerollCostText.text = $"Cost: {cost}";

        int requiredExp = CharacterManager.Instance.RequiredExp(inst.level);

        expSlider.minValue = 0;
        expSlider.maxValue = requiredExp;
        expSlider.value = inst.exp;

        expText.text = $"{inst.exp} / {requiredExp}";
        enhanceCostText.text = "Cost: 100G";
    }

    private void RefreshStars(CharacterModel model)
    {
        int rarity = Mathf.Clamp(model.rarity, 1, stars.Length);

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].gameObject.SetActive(i < rarity);
        }
    }

    public override void OnOpen()
    {
        btnAssign.onClick.RemoveAllListeners();
        btnAssign.onClick.AddListener(OnClickAssign);

        btnReroll.onClick.RemoveAllListeners();
        btnReroll.onClick.AddListener(OnClickReroll);

        btnEnhance.onClick.RemoveAllListeners();
        btnEnhance.onClick.AddListener(OnClickEnhance);

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
                //RefreshAbilitySlots(model, inst);
                Refresh();
            });
        }
    }

    private int GetRerollCost(CharacterInstance inst)
    {
        int locked = 0;
        foreach (var slot in inst.abilitySlots)
        {
            if (slot.isLocked)
                locked++;
        }

        return 10 + locked * 10;
    }

    private void OnClickEnhance()
    {
        if (currentCharacterId < 0)
            return;

        bool result = CharacterManager.Instance
            .TryEnhanceCharacter(currentCharacterId);

        if (!result)
        {
            Debug.Log("강화 실패: 골드 부족");
            return;
        }

        Refresh();
    }
}
