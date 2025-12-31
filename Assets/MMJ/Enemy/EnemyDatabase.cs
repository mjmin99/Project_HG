using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Build.Pipeline.Utilities;
#endif

[CreateAssetMenu(menuName = "Enemy/Enemy Database", fileName = "EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{
    [SerializeField] private List<Enemy> enemies = new List<Enemy>();

    public Enemy Get(int id)
    {
        return enemies.Find(e => e.id == id);
    }

#if UNITY_EDITOR
    // CSV → Enemy Model 최신화
    [ContextMenu("Refresh From CSV")]
    private void RefreshFromCSV()
    {
        enemies.Clear();

        // 예시 CSV 위치
        TextAsset csv = Resources.Load<TextAsset>("CSV/EnemyTable");
        if (csv == null)
        {
            Debug.LogError("[EnemyDatabase] CSV not found");
            return;
        }

        string[] lines = csv.text.Split('\n');

        // 0번째 줄은 헤더
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] cols = lines[i].Split(',');

            Enemy enemy = new Enemy
            {
                id = int.Parse(cols[0]),
                enemyName = cols[1].Trim(),
                attack = float.Parse(cols[2]),
                magicAttack = float.Parse(cols[3]),
                maxHP = float.Parse(cols[4]),
                attackSpeed = float.Parse(cols[5]),

                attackRange = float.Parse(cols[6]),

                attackType = ParseAttackType(cols[7]),
                defense = float.Parse(cols[8]),
                isBoss = bool.Parse(cols[9])
            };

            enemies.Add(enemy);
        }

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();

        Debug.Log($"[EnemyDatabase] CSV Refresh 완료 ({enemies.Count} enemies)");
    }
#endif

#if UNITY_EDITOR
    private AttackType ParseAttackType(string value)
    {
        value = value.Trim();

        if (Enum.TryParse(value, out AttackType type))
            return type;

        Debug.LogWarning(
            $"[EnemyDatabase] Unknown AttackType '{value}', defaulting to Melee"
        );

        return AttackType.Melee;
    }
#endif
}
