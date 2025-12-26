using System.Collections.Generic;

public class Ability_DamageReduction : AbilityBase, IEventAbility
{
    private readonly AbilityRarity rarity;
    public Ability_DamageReduction(AbilityRarity r) { rarity = r; }
    public override int AbilityId => AbilityIds.DamageReduction;
    public override string Name => "받는 피해 감소";
    public override AbilityRarity Rarity => rarity;
    public override AbilityScope Scope => AbilityScope.Tanker;

    public void OnEvent(ref AbilityContext ctx)
    {
        if (ctx.eventType != BattleEventType.Damaged) return;
        if (ctx.amount <= 0f) return;

        float rate = AbilityTiers.StatRate(rarity, 0.08f, 0.14f, 0.22f);
        ctx.amount *= (1f - rate);
    }
}

public interface IPartyProvider
{
    IEnumerable<ICombatActor> GetAllies(ICombatActor self);
}

public class Ability_TakeDamageForAlly : AbilityBase, IEventAbility // 기사의 맹세인데 이건 구현이 좀 어려울 수도 있겠다
{
    private readonly AbilityRarity rarity;
    public Ability_TakeDamageForAlly(AbilityRarity r) { rarity = r; }
    public override int AbilityId => AbilityIds.TakeDamageForAlly;
    public override string Name => "파티원 대신 피해 받기(기사의 맹세)";
    public override AbilityRarity Rarity => rarity;
    public override AbilityScope Scope => AbilityScope.Tanker;

    public void OnEvent(ref AbilityContext ctx)
    {
        // "누군가가 피해를 받기 직전" 브로드캐스트로 이 어빌리티도 호출된다고 가정
        if (ctx.eventType != BattleEventType.Damaged) return;

        // ctx.owner = 피해자, 탱커는 이 Ability의 실제 소유자(AbilityRunner.Owner)여야 함
        // 네 구조상 OnEvent에 owner가 들어오므로, 여기서는 "탱커 자신인지" 비교가 필요함.
        // => 해결: 전투 브로드캐스트에서 각 runner 별로 ctx.owner를 runner.Owner로 세팅하지 말고,
        //    ctx.owner는 '피해자', ctx.target은 '공격자', 그리고 별도 field에 abilityOwner가 필요.
        //
        // 그래서 현실적으로는: "파티 브로드캐스트 이벤트" 타입을 따로 만들거나
        // AbilityContext에 "abilityOwner"를 추가하는 게 정석.
        //
        // 여기서는 최소 수정으로: ctx.redirectTarget을 사용하기 위해
        // 전투 코드에서 탱커의 runner를 호출할 때 ctx.owner를 탱커로 넣고,
        // 피해 대상은 ctx.target에 넣는 방식으로 쓰자.

        // 기대 컨벤션:
        // - 탱커 runner 호출 시: ctx.owner = 탱커, ctx.target = 실제 피해자(아군)
        // - ctx.amount = 피해량
        if (ctx.target == null) return; // 실제 피해자
        if (!ctx.target.IsAlive) return;
        if (!ctx.owner.IsAlive) return;

        float takeRate = AbilityTiers.StatRate(rarity, 0.40f, 0.55f, 0.70f); // 대신받는 비율
        float redirected = ctx.amount * takeRate;

        // 피해자 피해를 줄이고, 탱커가 대신 받도록 지시
        ctx.amount -= redirected;
        ctx.owner.TakeDamage(redirected, source: null, isCritical: false, isSkill: false);
    }
}
