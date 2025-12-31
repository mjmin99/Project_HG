public interface IAbility
{
    int AbilityId { get; }
    string Name { get; }
    AbilityRarity Rarity { get; }
    AbilityScope Scope { get; }

    // 장착 가능 여부(포지션/근원거리 제한)
    bool CanApplyTo(ICombatActor owner);
}

public interface IStatModifierAbility : IAbility
{
    // 스탯 계산 시점에 항상 적용
    void ModifyStats(ref CharacterStats stats, ICombatActor owner);
}

// 임시로 막아둠
// public interface IEventAbility : IAbility
// {
//     // 이벤트 발생 시 반응
//     void OnEvent(ref AbilityContext ctx);
// }