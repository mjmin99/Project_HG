using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public ObjectPool returnPool { get; set; }

    private float timer;

    protected void Update()
    {
        if (timer <= 0f) return;
        
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 0f;
            ReturnToPoolNow();
        }
    }

    protected void ReturnToPool(float returnDelay = 2.5f)
    {
        timer = returnDelay;
    }

    protected void ReturnToPoolNow()
    {
        timer = 0f;
        returnPool.ReturnObject(this);
    }
}
