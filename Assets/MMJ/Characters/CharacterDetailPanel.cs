using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterDetailPanel : MonoBehaviour
{
    [Header("Root")]
    public GameObject panelRoot;

    [Header("Top")]
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text roleText;

    [Header("Rarity Stars")]
    public Transform starGroup;

    [Header("Progress")]
    public TMP_Text levelText;
    public TMP_Text starText;
    public TMP_Text shardText;

    [Header("Stats")]
    public TMP_Text hpText;
    public TMP_Text atkText;
    public TMP_Text matkText;
    public TMP_Text defText;
    public TMP_Text aspdText;
    public TMP_Text critText;
    public TMP_Text critDmgText;
    public TMP_Text rangeText;

    private void Awake()
    {
        // 처음엔 닫혀있어야 함
        panelRoot.SetActive(false);
    }

    public void Show(int characterId)
    {
        var model = CharacterManager.Instance.models[characterId];
        var inst = CharacterManager.Instance.instances[characterId];
        var stats = CharacterManager.Instance.GetStats(characterId);

        panelRoot.SetActive(true);

        icon.sprite = model.Icon;
        nameText.text = model.characterName;
        roleText.text = model.role.ToString();

        // rarity 별 표시
        for (int i = 0; i < starGroup.childCount; i++)
            starGroup.GetChild(i).gameObject.SetActive(i < model.rarity);

        levelText.text = $"Lv. {inst.level}";
        starText.text = $"★ {inst.star}";
        shardText.text = $"Shard: {inst.shard}";

        hpText.text = $"HP {stats.hp}";
        atkText.text = $"ATK {stats.attack}";
        matkText.text = $"MATK {stats.magicAttack}";
        defText.text = $"DEF {stats.defense}";
        aspdText.text = $"ASPD {stats.attackSpeed:0.00}";
        critText.text = $"CRIT {stats.critRate:0.0}%";
        critDmgText.text = $"CRIT DMG {stats.critDamage:0.0}%";
        rangeText.text = $"RANGE {model.attackRange}";
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
    }
}
