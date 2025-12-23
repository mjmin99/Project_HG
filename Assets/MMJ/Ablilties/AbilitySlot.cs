using System;

[Serializable]
public class AbilitySlot
{
    public AbilityInstance ability; // null 가능 (빈 슬롯)
    public bool isLocked;
}
