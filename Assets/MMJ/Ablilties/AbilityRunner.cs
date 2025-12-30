using UnityEngine;
// 스탯 계산 파이프라인 + 이벤트 디스패치를 실제로 수행하는 클래스
// 전투 유닛 하나당 1개
public class AbilityRunner
{
    public ICombatActor Owner { get; }
    public AbilitySet Set { get; } = new AbilitySet();

    private int attackCounter = 0;

    public AbilityRunner(ICombatActor owner)
    {
        Owner = owner;
    }

    // 1) 스탯 계산 단계: baseStats -> modifiers 적용 -> finalStats
    public CharacterStats ApplyStatModifiers(CharacterStats baseStats)
    {
        var stats = baseStats;

        foreach (var a in Set.StatAbilities)
        {
            a.ModifyStats(ref stats, Owner);
        }

        return stats;
    }

    // 2) 이벤트 발생 단계
    public void RaiseEvent(ref AbilityContext ctx)
    {
        if (Owner == null) return;

        // (선택) 공격 카운터 자동 기록
        if (ctx.eventType == BattleEventType.Attack)
        {
            attackCounter++;
            ctx.attackIndex = attackCounter;
        }

        // foreach (var a in Set.EventAbilities)
        // {
        //     a.OnEvent(ref ctx);
        // }
    }

    public void ResetAttackCounter() => attackCounter = 0;
}
