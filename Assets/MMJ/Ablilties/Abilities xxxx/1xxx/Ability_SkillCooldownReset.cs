using UnityEngine;

public class Ability_SkillCooldownReset : AbilityBase, IEventAbility
{
    private readonly AbilityRarity rarity;

    public Ability_SkillCooldownReset(AbilityRarity rarity)
    {
        this.rarity = rarity;
    }

    public override int AbilityId => AbilityIds.SkillCooldownResetOnUse;
    public override string Name => "스킬 사용 시 쿨타임 초기화";
    public override AbilityRarity Rarity => rarity;
    public override AbilityScope Scope => AbilityScope.Common;

    public void OnEvent(ref AbilityContext ctx)
    {
        if (ctx.eventType != BattleEventType.SkillUsed)
            return;

        float chance = GetChance();

        if (Random.value > chance)
            return;

        // 🔑 핵심:
        // 실제 스킬 시스템을 몰라도,
        // "쿨타임 초기화 요청"만 던짐
        if (ctx.target is ISkillUser skillUser)
        {
            skillUser.ResetLastUsedSkillCooldown();
        }
    }

    private float GetChance()
    {
        return rarity switch
        {
            AbilityRarity.Tier1 => 0.10f, // 10%
            AbilityRarity.Tier2 => 0.20f, // 20%
            AbilityRarity.Tier3 => 0.35f, // 35%
            _ => 0f
        };
    }
}

// 지금 당장은 가정으로 만든것임.. 스킬을 사용하는 player어에 이 인터페이스 구현
public interface ISkillUser
{
    void ResetLastUsedSkillCooldown();
}

