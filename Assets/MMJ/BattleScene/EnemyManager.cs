using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private List<Enemy> enemies = new();

    private void Awake()
    {
        Instance = this;
    }

    public void Register(Enemy e)
    {
        if (!enemies.Contains(e))
        {
            enemies.Add(e);
            Debug.Log("Enemy 등록됨 → 현재 적 수: " + enemies.Count);
        }
    }

    public void Unregister(Enemy e)
    {
        enemies.Remove(e);
    }

    public Enemy GetClosestEnemy(Vector3 fromPos)
    {
        float minDist = float.MaxValue;
        Enemy closest = null;

        foreach (var e in enemies)
        {
            if (e == null) continue;
            float dist = Vector3.Distance(fromPos, e.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = e;
            }
        }

        return closest;
    }

    public List<Enemy> GetAllEnemies()
    {
        return enemies;
    }
}
