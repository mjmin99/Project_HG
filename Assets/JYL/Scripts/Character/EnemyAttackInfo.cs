using System;
public struct EnemyAttackInfo
{
    public float atk;
    public bool isPoison;

    public EnemyAttackInfo(float atk, bool isPoison)
    {
        this.atk = atk;
        this.isPoison = isPoison;
    }
}
