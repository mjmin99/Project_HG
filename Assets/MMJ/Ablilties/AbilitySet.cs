using System.Collections.Generic;

// 한 캐릭터(전투 유닛)가 가진 어빌리티 묶음을 관리하는 런타임 컨테이너
public class AbilitySet
{
    private readonly List<IStatModifierAbility> statAbilities = new();
    private readonly List<IEventAbility> eventAbilities = new();

    public IReadOnlyList<IStatModifierAbility> StatAbilities => statAbilities;
    public IReadOnlyList<IEventAbility> EventAbilities => eventAbilities;

    public void Clear()
    {
        statAbilities.Clear();
        eventAbilities.Clear();
    }

    public void Add(IAbility ability, ICombatActor owner)
    {
        if (ability == null) return;
        if (!ability.CanApplyTo(owner)) return;

        if (ability is IStatModifierAbility s)
            statAbilities.Add(s);

        if (ability is IEventAbility e)
            eventAbilities.Add(e);
    }
}
