using System;

[Serializable]
public class GachaDrawConfig
{
    public int drawCount;
    public int guaranteeMinRarity; // 0이면 보장 없음

    public GachaDrawConfig(int drawCount, int guaranteeMinRarity)
    {
        this.drawCount = drawCount;
        this.guaranteeMinRarity = guaranteeMinRarity;
    }
}
