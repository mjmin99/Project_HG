using System.Collections.Generic;
using UnityEngine;

public static class CharacterCSVLoader
{
    public static List<CharacterModel> Load()
    {
        // Resources/characters.csv
        TextAsset csvFile = Resources.Load<TextAsset>("characters");

        if (csvFile == null)
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다! 경로: Resources/characters.csv");
            return new List<CharacterModel>();
        }

        string[] lines = csvFile.text.Split('\n');
        List<CharacterModel> list = new List<CharacterModel>();

        for (int i = 1; i < lines.Length; i++) // header 제외
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var tokens = line.Split(',');
            if (tokens.Length < 12) continue;

            CharacterModel m = new CharacterModel();
            m.id = int.Parse(tokens[0]);
            m.characterName = tokens[1];
            m.rarity = int.Parse(tokens[2]);
            m.role = System.Enum.Parse<CharacterRole>(tokens[3]);

            m.baseHP = int.Parse(tokens[4]);
            m.baseAttack = int.Parse(tokens[5]);
            m.baseMagicAttack = int.Parse(tokens[6]);
            m.baseDefense = int.Parse(tokens[7]);

            m.baseAttackSpeed = float.Parse(tokens[8]);
            m.baseCritRate = float.Parse(tokens[9]);
            m.baseCritDamage = float.Parse(tokens[10]);
            m.attackRange = float.Parse(tokens[11]);

            list.Add(m);
        }

        Debug.Log($"CSV 로드 성공! 캐릭터 수 = {list.Count}");
        return list;
    }
}