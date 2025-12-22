using UnityEngine;

public struct TestTimeInfo
{
    public Vector3 position;
    public float hp;
    public float shield;

    public TestTimeInfo(Vector3 position,float hp, float shield)
    {
        this.position = position;
        this.hp = hp;
        this.shield = shield;
    }
}
