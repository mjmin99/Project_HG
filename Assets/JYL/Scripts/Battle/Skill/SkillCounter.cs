using UniRx;
using UnityEngine;

public struct SkillCounter
{
    public readonly int charId;
    public readonly SkillType type;
    public ReactiveProperty<int> skillCount;

    public SkillCounter(int characterId, SkillType type = SkillType.StrongAttack,  int skillCount = 0)
    {
        charId = characterId;
        this.type = type;
        this.skillCount = new();
        this.skillCount.Value = skillCount;
    }
}
