using System;
using UnityEngine;

public struct AttackInfo
{
    public readonly LayerMask layer; 
    public readonly float atk;

    public AttackInfo(LayerMask layer, float atk)
    {
        this.layer = layer;
        this.atk = atk;
    }
}
