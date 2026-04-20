using System.Collections.Generic;
using UnityEngine;

public class AllHeal : Skill
{
    public override void Init(Stack<Skill> skillPool)
    {
        base.Init(skillPool);
        skillType = SkillType.AllHeal;
    }

    public override void SkillEffect()
    {
        base.SkillEffect();
        Manager.Audio.PlaySfx("Heal");
    }
}