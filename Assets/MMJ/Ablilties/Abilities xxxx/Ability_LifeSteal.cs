// 임시로 닫아둠
// public class Ability_LifeSteal : AbilityBase, IEventAbility
// {
//     private readonly AbilityRarity rarity;
//     public Ability_LifeSteal(AbilityRarity r) { rarity = r; }
//     public override int AbilityId => AbilityIds.LifeStealOnAttack;
//     public override string Name => "공격 시 흡혈";
//     public override AbilityRarity Rarity => rarity;
//     public override AbilityScope Scope => AbilityScope.Dealer_Melee;
// 
//     public void OnEvent(ref AbilityContext ctx)
//     {
//         if (ctx.eventType != BattleEventType.Attack) return;
//         if (ctx.amount <= 0f) return;
// 
//         float rate = AbilityTiers.StatRate(rarity, 0.03f, 0.06f, 0.08f);
//         float heal = ctx.amount * rate;
//         ctx.owner.Heal(heal);
//     }
// }

// 임시로 닫아둠
// public class Ability_BossDamageUp : AbilityBase, IEventAbility
// {
//     private readonly AbilityRarity rarity;
//     public Ability_BossDamageUp(AbilityRarity r) { rarity = r; }
//     public override int AbilityId => AbilityIds.BossDamageUp;
//     public override string Name => "보스에게 가하는 피해 증가";
//     public override AbilityRarity Rarity => rarity;
//     public override AbilityScope Scope => AbilityScope.Dealer_Melee; // 원하면 Dealer 공통으로 바꿔도 됨
// 
//     public void OnEvent(ref AbilityContext ctx)
//     {
//         if (ctx.eventType != BattleEventType.Attack) return;
//         if (!ctx.targetIsBoss) return;
// 
//         float rate = AbilityTiers.StatRate(rarity, 0.10f, 0.20f, 0.35f);
//         ctx.amount *= (1f + rate);
//     }
// }
