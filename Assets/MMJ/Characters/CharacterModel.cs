using UnityEngine;

public class CharacterModel
{
    public int id;
    public string characterName;
    public int rarity; // 1~5
    public CharacterRole role;

    public int baseHP;
    public AttackType attackType;   // Melee / Ranged
    public int baseAttack;
    public int baseMagicAttack;
    public int baseDefense;

    public float baseAttackSpeed;
    public float baseCritRate;
    public float baseCritDamage;
    public float attackRange;

    public GameObject prefab;

    // 아이콘 캐싱용 (Resources.Load를 한 번만 실행하게 함)
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


    // 여기에 스킬 모델 추가 가능(지금은 생략)
    // public SkillModel passiveSkill;
    // public SkillModel activeSkill;
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

