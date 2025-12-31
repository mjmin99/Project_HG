using System;
using UnityEngine;

public struct AttackInfo
{
    public readonly LayerMask layer; 
    public float atk;
    public bool isCritical;

    public AttackInfo(LayerMask layer, float atk,  bool isCritical)
    {
        this.layer = layer;
        this.atk = atk;
        this.isCritical = isCritical;
    }
}
