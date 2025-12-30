// 임시로 막아둠
// BattleEventType에 추가 권장: HealApplyAttempt, ShieldApplyAttempt
//
//public class Ability_ShieldAndHealBoost : AbilityBase, IEventAbility
//{
//    private readonly AbilityRarity rarity;
//    public Ability_ShieldAndHealBoost(AbilityRarity r) { rarity = r; }
//    public override int AbilityId => AbilityIds.ShieldAndHealBoost;
//    public override string Name => "보호막 & 회복량 증가";
//    public override AbilityRarity Rarity => rarity;
//    public override AbilityScope Scope => AbilityScope.Tanker;
//
//    public void OnEvent(ref AbilityContext ctx)
//    {
//        // ctx.owner = 대상(탱커 자신)이라고 가정
//        if (ctx.eventType != BattleEventType.Tick && ctx.eventType != BattleEventType.BattleStart)
//        {
//            // 여기선 치트 방지: 실제로는 Heal/Shield 이벤트에서 amount를 뻥튀기하면 됨.
//        }
//    }
//
//    public void Boost(ref float amount)
//    {
//        float rate = AbilityTiers.StatRate(rarity, 0.10f, 0.20f, 0.35f);
//        amount *= (1f + rate);
//    }
//}
