using System;
// saveData에 넣기 위한 용도로 제작
// 수치나 효과를 저장하지 않고 ID, Tier를 저장
[Serializable]
public class AbilityInstance
{
    public int abilityId;
    public AbilityRarity rarity;

    public bool isUnlocked;

    public AbilityInstance(int id, AbilityRarity rarity)
    {
        this.abilityId = id;
        this.rarity = rarity;
        this.isUnlocked = true;
    }
}
