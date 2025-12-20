using UnityEngine;

public class CharacterModel
{
    public int id;
    public string characterName;
    public int rarity; // 1~5 (고정, 태생부터 결정)
    public CharacterRole role;

    public int baseHP;
    public int baseAttack;
    public int baseMagicAttack;
    public int baseDefense;

    public float baseAttackSpeed;
    public float baseCritRate;
    public float baseCritDamage;
    public float attackRange;

    public int MaxAbilitySlotCount // 어빌리티 슬롯 카운트
    {
        get
        {
            // rarity: 1~5
            return Mathf.Clamp(rarity, 1, 5);
        }
    }

    public AttackType attackType; // Melee / Ranged

    public GameObject prefab;

    private Sprite _icon;
    public Sprite Icon
    {
        get
        {
            if (_icon == null)
                _icon = Resources.Load<Sprite>($"Icons/{characterName}");
            return _icon;
        }
    }
}

public enum CharacterRole
{
    Tank,
    Dealer,
    Healer
}

public enum AttackType
{
    Melee,
    Ranged
}