using UniRx;
using UnityEngine;

public struct SkillInfo
{
    public readonly int charId;
    public readonly SkillType type;
    public readonly ReactiveProperty<int> skillCount;
    public readonly Sprite skillIcon;
    
    public readonly float skillCooldown;

    public SkillInfo(int characterId, Sprite skillIcon, SkillType type = SkillType.StrongAttack,  int skillCount = 0, float skillCooldown = 0)
    {
        charId = characterId;
        this.skillIcon = skillIcon;
        this.type = type;
        this.skillCount = new ReactiveProperty<int>();
        this.skillCount.Value = skillCount;
        this.skillCooldown = skillCooldown;
    }
}
