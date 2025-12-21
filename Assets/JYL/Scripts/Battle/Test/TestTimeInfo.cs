using UnityEngine;

public struct TestTimeInfo
{
    public Vector3 position;
    public Quaternion rotation;
    public float hp;
    public float shield;

    public TestTimeInfo(Transform tr, float hp, float shield)
    {
        position = tr.position;
        rotation = tr.rotation;
        this.hp = hp;
        this.shield = shield;
    }
}
