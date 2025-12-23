using UnityEngine;

// 지금 당장은 가정으로 만든것임.. 스킬을 사용하는 player어에 이 인터페이스 구현
public interface ISkillUser
{
    void ResetLastUsedSkillCooldown();
}

public class Ability_SkillCooldownReset : AbilityBase, IEventAbility
{
    private readonly AbilityRarity rarity;
    public Ability_SkillCooldownReset(AbilityRarity r) { rarity = r; }
    public override int AbilityId => AbilityIds.SkillCooldownResetOnUse;
    public override string Name => "스킬 사용 시 쿨타임 초기화";
    public override AbilityRarity Rarity => rarity;
    public override AbilityScope Scope => AbilityScope.Common;

    public void OnEvent(ref AbilityContext ctx)
    {
        if (ctx.eventType != BattleEventType.SkillUsed) return;

        float chance = AbilityTiers.Chance(rarity, 0.10f, 0.20f, 0.35f);
        if (Random.value > chance) return;

        if (ctx.owner is ISkillUser user)
            user.ResetLastUsedSkillCooldown();
    }
}


