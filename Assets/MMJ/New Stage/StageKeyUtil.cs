using UnityEngine;

public static class StageKeyUtil
{
    // "W01-S005"
    public static string ToKey(int world, int stage) => $"W{world:00}-S{stage:000}";
}

