public static class AbilityTiers
{
    public static float StatRate(AbilityRarity r, float t1, float t2, float t3)
        => r switch { AbilityRarity.Tier1 => t1, AbilityRarity.Tier2 => t2, AbilityRarity.Tier3 => t3, _ => t1 };

    public static float Chance(AbilityRarity r, float t1, float t2, float t3)
        => StatRate(r, t1, t2, t3);

    public static float Duration(AbilityRarity r, float t1, float t2, float t3)
        => StatRate(r, t1, t2, t3);
}
