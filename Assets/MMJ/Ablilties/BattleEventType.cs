public enum BattleEventType
{
    // 전투 라이프사이클
    BattleStart,
    BattleEnd,
    Tick,               // 오라/지속회복 같은 주기 처리

    // 공격/피격
    Attack,             // 공격이 발생했을 때 (가해자 기준)
    Damaged,            // 피해를 받았을 때 (피해자 기준)
    Critical,           // 치명타가 발생했을 때 (공격자 기준)

    // 스킬
    SkillUsed,          // 스킬 사용 직후

    // 상태이상(기절/넉백 등)
    StatusApplyAttempt, // 상태이상 적용 시도(저항 확률 처리)
    StatusApplied       // 상태이상 적용 완료
}
