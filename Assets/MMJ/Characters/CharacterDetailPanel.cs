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

    [Header("Enhance")]
    public Button enhanceButton;
    public TMP_Text enhanceCostText;

    private const int ENHANCE_COST = 10;
    private int currentCharacterId = -1;

    private void Awake()
    {
        panelRoot.SetActive(false);
        enhanceButton.onClick.AddListener(OnClickEnhance);
    }

    public void Show(int characterId)
    {
        currentCharacterId = characterId;

        if (!CharacterManager.Instance.models.TryGetValue(characterId, out var model))
        {
            Debug.LogError($"[CharacterDetailPanel] 모델 ID {characterId} 없음");
            return;
        }

        if (!CharacterManager.Instance.instances.TryGetValue(characterId, out var inst))
        {
            Debug.LogError($"[CharacterDetailPanel] 인스턴스 ID {characterId} 없음");
            return;
        }

        var stats = CharacterManager.Instance.GetStats(characterId);

        panelRoot.SetActive(true);

        icon.sprite = model.Icon;
        nameText.text = model.characterName;
        roleText.text = model.role.ToString();

        for (int i = 0; i < starGroup.childCount; i++)
            starGroup.GetChild(i).gameObject.SetActive(i < model.rarity);

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

        enhanceCostText.text = $"{ENHANCE_COST} Gold";
        UpdateEnhanceButtonState();
    }

    void UpdateEnhanceButtonState()
    {
        int gold = SaveManager.Instance.CurrentData.gold;
        enhanceButton.interactable = (gold >= ENHANCE_COST);
    }

    public void OnClickEnhance()
    {
        if (currentCharacterId < 0)
            return;

        if (!SaveManager.Instance.TrySpendGold(ENHANCE_COST))
        {
            Debug.Log("[CharacterDetailPanel] 골드 부족");
            return;
        }

        CharacterManager.Instance.AddExp(currentCharacterId, 5);
        SaveManager.Instance.SaveCurrentUser();

        RefreshCurrentCharacterUI();
    }

    void RefreshCurrentCharacterUI()
    {
        Show(currentCharacterId);
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
        currentCharacterId = -1;
    }
}