// 어빌리티가 전투 중 대상/자기 자신을 조작하기위한 최소한의 인터페이스

public interface ICombatActor
{
    int Id { get; }                // 캐릭터 ID 등
    CharacterRole Role { get; }    // Tank/Dealer/Healer(=Supporter로 쓸 수도)
    AttackType AttackType { get; } // Melee/Ranged

    bool IsBoss { get; }           // 대상이 보스인지 여부(보스 추가피해 등)
    bool IsAlive { get; }

    // 현재 체력/최대체력은 네 전투 구현에서 맞춰주면 됨
    float CurrentHP { get; }
    float MaxHP { get; }

    // 전투 조작(최소 기능만)
    void Heal(float amount);
    void TakeDamage(float amount, ICombatActor source = null, bool isCritical = false, bool isSkill = false);

    // 버프/디버프/상태이상(나중에 구현)
    void ApplyDot(float damagePerTick, float duration, float tickInterval, ICombatActor source = null);
    bool TryApplyStatus(StatusType type, float duration, ICombatActor source = null);
}
