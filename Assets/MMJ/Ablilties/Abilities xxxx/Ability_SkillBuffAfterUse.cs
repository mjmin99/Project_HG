public struct TempStatBuff
{
    public float duration;
    public float atkMul;
    public float aspdMul;
    public float critAdd;
}

public interface ITempBuffReceiver
{
    void AddTempBuff(TempStatBuff buff);
}

public class Ability_SkillBuffAfterUse : AbilityBase, IEventAbility
{
    private readonly AbilityRarity rarity;
    public Ability_SkillBuffAfterUse(AbilityRarity r) { rarity = r; }
    public override int AbilityId => AbilityIds.SkillBuffAfterUse;
    public override string Name => "스킬 사용 후 일정시간 능력치 증가";
    public override AbilityRarity Rarity => rarity;
    public override AbilityScope Scope => AbilityScope.Common;

    public void OnEvent(ref AbilityContext ctx)
    {
        if (ctx.eventType != BattleEventType.SkillUsed) return;

        float dur = AbilityTiers.Duration(rarity, 3f, 4f, 5f);
        float atk = AbilityTiers.StatRate(rarity, 0.06f, 0.10f, 0.16f);
        float aspd = AbilityTiers.StatRate(rarity, 0.06f, 0.10f, 0.16f);
        float crit = AbilityTiers.StatRate(rarity, 0.02f, 0.03f, 0.05f);

        if (ctx.owner is ITempBuffReceiver recv)
        {
            recv.AddTempBuff(new TempStatBuff
            {
                duration = dur,
                atkMul = atk,
                aspdMul = aspd,
                critAdd = crit
            });
        }
    }
}
