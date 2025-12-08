using System.Collections.Generic;
using UnityEngine;

public static class CharacterCSVLoader
{
    public static List<CharacterModelRuntime> LoadFromCSV()
    {
        TextAsset csv = Resources.Load<TextAsset>("characters");
        var lines = csv.text.Split('\n');

        List<CharacterModelRuntime> list = new List<CharacterModelRuntime>();

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();

            // 빈 줄이면 스킵
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var row = line.Split(',');

            // 최소 5개의 필드가 없으면 스킵 (보호)
            if (row.Length < 5)
                continue;

            int id;
            int maxHP;
            int attack;

            // 숫자가 아니면 스킵 (에러 방지)
            if (!int.TryParse(row[0], out id) ||
                !int.TryParse(row[2], out maxHP) ||
                !int.TryParse(row[3], out attack))
            {
                Debug.LogError($"CSV 파싱 오류: 숫자 형식 아님 → {line}");
                continue;
            }

            var data = new CharacterModelRuntime()
            {
                id = id,
                name = row[1].Trim(),
                maxHP = maxHP,
                attack = attack,
                prefab = Resources.Load<GameObject>("Characters/" + row[4].Trim())
            };

            list.Add(data);
        }

        return list;
    }
}
