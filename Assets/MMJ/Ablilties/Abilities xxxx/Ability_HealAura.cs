// 임시로 닫아둠
// public class Ability_HealAura : AbilityBase, IEventAbility
// {
//     private readonly AbilityRarity rarity;
//     private float acc;
// 
//     public Ability_HealAura(AbilityRarity r) { rarity = r; }
//     public override int AbilityId => AbilityIds.HealAura;
//     public override string Name => "아군 지속 회복 오라";
//     public override AbilityRarity Rarity => rarity;
//     public override AbilityScope Scope => AbilityScope.Supporter;
// 
//     public void OnEvent(ref AbilityContext ctx)
//     {
//         if (ctx.eventType != BattleEventType.Tick) return;
//         if (ctx.owner == null) return;
// 
//         // Tick 누적해서 1초마다 한번
//         acc += ctx.deltaTime;
//         if (acc < 1f) return;
//         acc -= 1f;
// 
//         if (ctx.owner is not IPartyProvider party) return;
// 
//         float healRate = AbilityTiers.StatRate(rarity, 0.008f, 0.012f, 0.018f); // 최대체력 비율/초
//         foreach (var ally in party.GetAllies(ctx.owner))
//         {
//             if (ally == null || !ally.IsAlive) continue;
//             float heal = ally.MaxHP * healRate;
//             ally.Heal(heal);
//         }
//     }
// }
