using UnityEngine;

// 임시로 닫아둠
// public class Ability_DotOnHit : AbilityBase, IEventAbility
// {
//     private readonly AbilityRarity rarity;
//     public Ability_DotOnHit(AbilityRarity r) { rarity = r; }
//     public override int AbilityId => AbilityIds.DotOnHit;
//     public override string Name => "공격 시 도트 피해 부여";
//     public override AbilityRarity Rarity => rarity;
//     public override AbilityScope Scope => AbilityScope.Dealer_Ranged;
// 
//     public void OnEvent(ref AbilityContext ctx)
//     {
//         if (ctx.eventType != BattleEventType.Attack) return;
//         if (ctx.target == null) return;
// 
//         float dur = AbilityTiers.Duration(rarity, 3f, 4f, 5f);
//         float tick = 1f;
// 
//         // 틱당 피해: (이번 공격 피해량 비례)로 잡음
//         float dpsRate = AbilityTiers.StatRate(rarity, 0.08f, 0.12f, 0.18f);
//         float damagePerTick = Mathf.Max(1f, ctx.amount * dpsRate);
// 
//         ctx.target.ApplyDot(damagePerTick, dur, tick, ctx.owner);
//     }
// }
