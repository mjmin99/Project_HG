using System.Collections.Generic;

public static class GachaPolicies
{
    // 비용 정책 (일단 정직하게: n * 50)
    // 나중에 10연/100연 할인 넣고 싶으면 여기만 바꾸면 됨
    public static int GetGoldCost(int drawCount) => drawCount * 50;

    // rarity 가중치 (현재 너의 로직 유지)
    public static readonly Dictionary<int, float> RarityWeights = new()
    {
        { 1, 60f },
        { 2, 25f },
        { 3, 10f },
        { 4,  4f },
        { 5,  1f },
    };

    public static float GetWeight(int rarity)
        => RarityWeights.TryGetValue(rarity, out var w) ? w : 1f;
}
