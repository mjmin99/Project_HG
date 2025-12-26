using System;
using System.Collections.Generic;

[Serializable]
public class GachaSummary
{
    public int drawCount;
    public int totalCost;
    public List<GachaResult> results = new();

    public GachaSummary(int drawCount, int totalCost, List<GachaResult> results)
    {
        this.drawCount = drawCount;
        this.totalCost = totalCost;
        this.results = results;
    }
}
