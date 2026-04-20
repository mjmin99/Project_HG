using System.Collections.Generic;
using UnityEngine;

public class StrongAttack : Skill
{
    public override void Init(Stack<Skill> skillPool)
    {
        base.Init(skillPool);
        skillType = SkillType.StrongAttack;
    }

    public override void SkillEffect()
    {
        base.SkillEffect();
        Manager.Audio.PlaySfx("Strike");
    }
}