using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

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

    [Header("Visual")]
    [SerializeField] private Image standingImage;

    [Header("Currency")]
    [SerializeField] private TMP_Text goldText;

    private int currentCharacterId = -1;


    public void SetCharacter(int characterId)
    {
        currentCharacterId = characterId;
        Refresh();
    } // 외부에서 캐릭터 선택 시 호출

    private void Refresh()
    {
        if (!Manager.Character.models.TryGetValue(currentCharacterId, out var model))
            return;

        if (!Manager.Character.instances.TryGetValue(currentCharacterId, out var inst))
            return;

        var stats = Manager.Character.GetStats(currentCharacterId);

        // ===== 기존 표시 로직 그대로 =====
        icon.sprite = model.Icon;
        nameText.text = model.characterName;
        roleText.text = model.role.ToString();

        RefreshStars(model);

        levelText.text = $"LV. {inst.level}";
        shardText.text = $"{inst.shard}";

        hpText.text = $"{stats.hp:0}";
        atkText.text = $"{stats.attack:0}";
        matkText.text = $"{stats.magicAttack:0}";
        defText.text = $"{stats.defense:0}";
        aspdText.text = $"{stats.attackSpeed:0.00}";
        critText.text = $"{stats.critRate * 100:0.0}%";
        critDmgText.text = $"{stats.critDamage * 100:0.0}%";
        rangeText.text = $"{stats.attackRange:0.0}";

        RefreshAbilitySlots(model, inst); // 어빌리티 슬롯 생성

        // 업데이트에 할 필요 없는 이유가 여기에 두면 슬롯 버튼 누를 때 호출됨
        int cost = GetRerollCost(inst);
        rerollCostText.text = $"비용 : {cost}";

        int requiredExp = Manager.Character.RequiredExp(inst.level);

        expSlider.minValue = 0;
        expSlider.maxValue = requiredExp;
        expSlider.value = inst.exp;

        expText.text = $"{inst.exp} / <color=#AAAAAA>{requiredExp}</color>";
        enhanceCostText.text = "비용 : 100";

        standingImage.sprite = model.StandingSprite;
        standingImage.preserveAspect = true;
        standingImage.SetNativeSize();

        RefreshGold();
        RefreshEnhanceCost();
        RefreshShard();
    } // 디테일 패널의 모든 값 갱신

    private void RefreshStars(CharacterModel model)
    {
        int rarity = Mathf.Clamp(model.rarity, 1, stars.Length);

        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].gameObject.SetActive(i < rarity);
        }
    } // 등급 갱신
 
    private void RefreshGold()
    {
        int gold = Manager.Save.CurrentData.gold;
        goldText.text = $"Gold: {gold}";
    } // 돈 갱신

    private void RefreshShard()
    {
        if (!Manager.Character.instances
            .TryGetValue(currentCharacterId, out var inst))
            return;

        shardText.text = $"Shard: {inst.shard}";
    } // 강화석 갱신

    public override void OnOpen()
    {
        base.OnOpen();
        btnAssign.onClick.RemoveAllListeners();
        btnAssign.onClick.AddListener(OnClickAssign);

        btnReroll.onClick.RemoveAllListeners();
        btnReroll.onClick.AddListener(OnClickReroll);

        btnEnhance.onClick.RemoveAllListeners();
        btnEnhance.onClick.AddListener(OnClickEnhance);

        rerollCostText.text = "비용: 10";
    } // 패널 오픈

    public override void OnClose()
    {   
        currentCharacterId = -1;
        base.OnClose();
    } // 패널 클로즈

    private void OnClickReroll()
    {
        if (currentCharacterId < 0)
            return;

        bool result = Manager.Character.TryRerollAbilities(currentCharacterId);

        if (!result)
        {
            Debug.Log("리롤 실패 (골드 부족 또는 오류)");
            UIManager.Instance.ShowToast("SimpleToast","조각이 부족합니다");
            return;
        }
        Manager.Audio.PlaySfx("SFX_UseCoin");
        Refresh();
        PlayShardChangedAnim();
    } // 어빌리티 재설정

    private void OnClickAssign()
    {
        if (currentCharacterId < 0)
        {
            Debug.LogWarning("[CharacterDetailPanel] characterId invalid");
            return;
        }

        PartyAssignmentContext.Begin(currentCharacterId);

        UIManager.Instance.OpenUI<PartySlotSelectPopup>("PartySlotSelectPopup");
    } // 파티 편성

    private void RefreshAbilitySlots(CharacterModel model, CharacterInstance inst)
    {
        inst.SyncAbilitySlots(model);

        foreach (Transform child in abilitySlotGroup)
            Destroy(child.gameObject);

        var setBonusIds = inst.GetSetBonusAbilityIds();
        var counts = inst.GetAbilityCounts();

        int max = model.MaxAbilitySlotCount;
        int unlocked = inst.abilitySlots.Count;

        for (int i = 0; i < max; i++)
        {
            var obj = Instantiate(abilitySlotPrefab, abilitySlotGroup);
            var ui = obj.GetComponent<AbilitySlotUI>();

            ui.Bind(model, inst, i, () =>
            {
                Refresh();
            });

           // model.MaxAbilitySlotCount 기준으로 UI를 만들었는데
           // inst.abilitySlots 리스트의 실제 길이가 그보다 짧은 상태에서
           // inst.abilitySlots[i]를 접근해서 터진 것

            // 버그 수정 접근 보호
            if (i >= unlocked)
            {
                // 아직 해방 안 된 슬롯 → AbilitySlotUI에서 Locked 처리됨
                ui.SetSetBonusEffect(SetBonusLevel.None, Color.white);
                continue;
            }

            // 세트 보너스 연출 판단
            var slot = inst.abilitySlots[i];

            // 실제 ability 있을 때만 강도 판단
            if (slot.ability == null)
            {
                ui.SetSetBonusEffect(SetBonusLevel.None, Color.white);
                continue;
            }

            int abilityId = slot.ability.abilityId;

            SetBonusLevel level = GetSetBonusLevel(counts, abilityId);
            Color glowColor = GetGlowColorByAbility(abilityId);

            ui.SetSetBonusEffect(level, glowColor);
        }
    } // 어빌리티 슬롯 갱신

    private int GetRerollCost(CharacterInstance inst)
    {
        int locked = 0;
        foreach (var slot in inst.abilitySlots)
        {
            if (slot.isLocked)
                locked++;
        }

        return 10 + locked * 10;
    } // 어빌리티 재설정 재화 계산

    private void OnClickEnhance()
    {
        if (currentCharacterId < 0)
            return;

        bool result = Manager.Character
            .TryEnhanceCharacter(currentCharacterId);

        if (!result)
        {
            UIManager.Instance.ShowToast("SimpleToast","골드가 부족합니다");
            Debug.Log("강화 실패 : 골드 부족");
            return;
        }
        Manager.Audio.PlaySfx("SFX_UseCoin");
        Refresh();
        PlayGoldChangedAnim();
    } // 강화하기

    private void RefreshEnhanceCost()
    {
        int cost = Manager.Character
            .GetEnhanceCost(currentCharacterId);

        enhanceCostText.text = $"비용 : {cost}";
    } // 강화 재화 계산

    private void PlayShardChangedAnim()
    {
        shardText.transform.localScale = Vector3.one;
        shardText.transform
            .DOScale(1.2f, 0.1f)
            .SetLoops(2, LoopType.Yoyo);
    } // 강화석 소모 애니메이션

    private void PlayGoldChangedAnim()
    {
        goldText.transform.localScale = Vector3.one;
        goldText.transform
            .DOScale(1.2f, 0.1f)
            .SetLoops(2, LoopType.Yoyo);
    } // 돈 소모 애니메이션

    private SetBonusLevel GetSetBonusLevel(Dictionary<int, int> counts, int abilityId)
    {
        if (abilityId <= 0) return SetBonusLevel.None;
        if (!counts.TryGetValue(abilityId, out int cnt)) return SetBonusLevel.None;

        if (cnt >= 4) return SetBonusLevel.Quad;
        if (cnt >= 3) return SetBonusLevel.Triple;
        if (cnt >= 2) return SetBonusLevel.Pair;
        return SetBonusLevel.None;
    } // 세트 보너스 강도 판단 함수

    private Color GetGlowColorByAbility(int abilityId)
    {
        return abilityId switch
        {
            AbilityIds.AttackSpeedUp => new Color(0.3f, 1f, 0.9f), // 청록
            AbilityIds.MaxHPUp => new Color(0.4f, 1f, 0.4f), // 초록
            AbilityIds.DefenseUp => new Color(0.4f, 0.6f, 1f), // 파랑
            AbilityIds.AttackUp => new Color(1f, 0.4f, 0.4f), // 빨강
            AbilityIds.MagicAttackUp => new Color(0.8f, 0.4f, 1f), // 보라
            _ => Color.white
        };
    } // 발광 색 판단
}
