using UnityEngine;

public struct StageId
{
    public int world;
    public int stage;

    public StageId(int world, int stage)
    { 
        this.world = world;
        this.stage = stage;
    }

    public override string ToString()
    {
        return $"{world}-{stage}";
    }
}
