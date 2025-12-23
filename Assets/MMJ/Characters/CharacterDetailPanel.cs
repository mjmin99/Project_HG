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
        if (!Manager.Character.models.TryGetValue(currentCharacterId, out var model))
            return;

        if (!Manager.Character.instances.TryGetValue(currentCharacterId, out var inst))
            return;

        var stats = Manager.Character.GetStats(currentCharacterId);

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
    }

    public override void OnOpen()
    {
        btnAssign.onClick.RemoveAllListeners();
        btnAssign.onClick.AddListener(OnClickAssign);
    }

    public override void OnClose()
    {
        currentCharacterId = -1;
        base.OnClose();
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
}
