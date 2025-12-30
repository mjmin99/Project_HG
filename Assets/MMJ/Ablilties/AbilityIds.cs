// 숫자 규칙
// 1xxx: 공용
// 2xxx: 딜러
// 3xxx: 탱커
// 4xxx: 서포터
public static class AbilityIds
{
    // ===== 공용 =====
    public const int MaxHPUp = 1001;
    public const int AttackUp = 1002;
    public const int MagicAttackUp = 1003;
    public const int SkillDamageUp = 1004;
    public const int CritRateUp = 1005;
    public const int AttackRangeUp = 1006;
    public const int AttackSpeedUp = 1007;
    public const int CritDamageUp = 1008;
    public const int DefenseUp = 1009;

    public const int SkillCooldownResetOnUse = 1101;
    public const int SkillBuffAfterUse = 1102;

    // ===== 딜러 근거리 =====
    
    public const int LifeStealOnAttack = 2002;
    public const int BossDamageUp = 2003;

    // ===== 딜러 원거리 =====
    public const int RangedAttackUp = 2101;
    public const int EveryThirdAttackBonus = 2102;
    public const int DotOnHit = 2103;

    // ===== 탱커 =====
    public const int DamageReduction = 3001;
    public const int TakeDamageForAlly = 3002;
    public const int StatusResistance = 3003;
    public const int ShieldAndHealBoost = 3004;

    // ===== 서포터 =====
    public const int HealAura = 4001;
    public const int HealCanCrit = 4002;
    public const int HealAndShieldIncrease = 4003;
}
