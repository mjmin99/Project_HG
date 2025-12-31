// 임시로 닫아둠
// public interface IHealPipeline
// {
//     void EnableHealCrit(bool enabled);
// }
// 
// public class Ability_HealCanCrit : AbilityBase, IEventAbility
// {
//     private readonly AbilityRarity rarity;
//     public Ability_HealCanCrit(AbilityRarity r) { rarity = r; }
//     public override int AbilityId => AbilityIds.HealCanCrit;
//     public override string Name => "회복/보호막에 치명타 적용";
//     public override AbilityRarity Rarity => rarity;
//     public override AbilityScope Scope => AbilityScope.Supporter;
// 
//     public void OnEvent(ref AbilityContext ctx)
//     {
//         if (ctx.eventType != BattleEventType.BattleStart) return;
//         if (ctx.owner is IHealPipeline hp)
//             hp.EnableHealCrit(true);
//     }
// }
