using System;
//이벤트가 발생할 때, 어빌리티에게 “상황 정보”를 전달하는 컨텍스트

public struct AbilityContext
{
    public BattleEventType eventType;

    public ICombatActor owner;   // 어빌리티 보유자
    public ICombatActor target;  // 대상(공격/피격/스킬 대상)

    public float amount;         // 피해량/회복량/스킬피해량 등 범용
    public bool isCritical;
    public bool isSkill;

    public bool targetIsBoss;
    public StatusType statusType;

    public int attackIndex;      // (예: 매 3번째 공격) 카운트 기반용
    public float deltaTime;      // Tick 이벤트에서 사용

    public static AbilityContext Create(
        BattleEventType type,
        ICombatActor owner,
        ICombatActor target = null)
    {
        return new AbilityContext
        {
            eventType = type,
            owner = owner,
            target = target,
            amount = 0,
            isCritical = false,
            isSkill = false,
            targetIsBoss = target != null && target.IsBoss,
            statusType = StatusType.None,
            attackIndex = 0,
            deltaTime = 0
        };
    }
}
