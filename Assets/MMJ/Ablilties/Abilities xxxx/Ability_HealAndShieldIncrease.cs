public class Ability_HealAndShieldIncrease : AbilityBase, IEventAbility
{
    private readonly AbilityRarity rarity;
    public Ability_HealAndShieldIncrease(AbilityRarity r) { rarity = r; }
    public override int AbilityId => AbilityIds.HealAndShieldIncrease;
    public override string Name => "회복/보호막 증가량 추가 증가";
    public override AbilityRarity Rarity => rarity;
    public override AbilityScope Scope => AbilityScope.Supporter;

    public void OnEvent(ref AbilityContext ctx)
    {
        // 실제 구현은 Heal/Shield 이벤트에서 ctx.amount *= (1+rate)
        // 현재 전투 이벤트에 Heal/Shield 이벤트가 없으면, 다음 단계에서 붙이면 됨.
    }

    public float GetRate() => AbilityTiers.StatRate(rarity, 0.10f, 0.20f, 0.35f);
}
