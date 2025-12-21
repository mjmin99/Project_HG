using System;
using UnityEngine;

public struct AttackInfo
{
    public readonly LayerMask layer; 
    public readonly float atk;
    public readonly bool isPoison;

    public AttackInfo(LayerMask layer, float atk, bool isPoison)
    {
        this.layer = layer;
        this.atk = atk;
        this.isPoison = isPoison;
    }
}
