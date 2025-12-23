using System;
// saveData에 넣기 위한 용도로 제작
// 수치나 효과를 저장하지 않고 ID, Tier를 저장
[Serializable]
public class AbilityInstance
{
    public int abilityId;
    public AbilityRarity rarity;

    public bool isUnlocked = true; // 어빌리티 자체 보유 여부
    public bool isLocked = false;  // 슬롯에서 잠금 여부

    public AbilityInstance(int id, AbilityRarity rarity)
    {
        this.abilityId = id;
        this.rarity = rarity;
        this.isUnlocked = true;
    }
}
