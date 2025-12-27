using System;
using UnityEngine;

//임시로 닫아둠
// public class Ability_EveryThirdAttackBonus : AbilityBase, IEventAbility
// {
//     private readonly AbilityRarity rarity;
//     public Ability_EveryThirdAttackBonus(AbilityRarity r) { rarity = r; }
//     public override int AbilityId => AbilityIds.EveryThirdAttackBonus;
//     public override string Name => "매 3번째 공격 추가 데미지";
//     public override AbilityRarity Rarity => rarity;
//     public override AbilityScope Scope => AbilityScope.Dealer_Ranged;
// 
//     public void OnEvent(ref AbilityContext ctx)
//     {
//         if (ctx.eventType != BattleEventType.Attack) return;
//         if (ctx.target == null) return;
//         if (ctx.attackIndex <= 0) return;
// 
//         if (ctx.attackIndex % 3 != 0) return;
// 
//         float extraRate = AbilityTiers.StatRate(rarity, 0.30f, 0.60f, 1.00f);
//         float extra = Mathf.Max(1f, ctx.amount * extraRate);
// 
//         // 추가 피해는 즉시 별도 적용(기본 공격과 분리)
//         ctx.target.TakeDamage(extra, ctx.owner, isCritical: false, isSkill: false);
//     }
// }
