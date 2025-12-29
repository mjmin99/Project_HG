using System.Collections.Generic;
using UnityEngine;

public static class CharacterCSVLoader
{
    public static List<CharacterModel> Load()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("CSV/characters");

        if (csvFile == null)
        {
            Debug.LogError("CSV 파일을 찾을 수 없습니다! 경로: Resources/characters.csv");
            return new List<CharacterModel>();
        }

        string[] lines = csvFile.text.Split('\n');
        List<CharacterModel> list = new List<CharacterModel>();

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var tokens = line.Split(',');

            if (tokens.Length < 13)
            {
                Debug.LogWarning($"CSV 라인 {i} 파싱 실패: 컬럼 수 부족 ({tokens.Length}/13)");
                continue;
            }

            try
            {
                CharacterModel m = new CharacterModel();

                m.id = int.Parse(tokens[0].Trim());
                m.characterName = tokens[1].Trim();
                m.rarity = int.Parse(tokens[2].Trim());
                m.role = System.Enum.Parse<CharacterRole>(tokens[3].Trim());

                m.baseHP = int.Parse(tokens[4].Trim());
                m.baseAttack = int.Parse(tokens[5].Trim());
                m.baseMagicAttack = int.Parse(tokens[6].Trim());
                m.baseDefense = int.Parse(tokens[7].Trim());

                m.baseAttackSpeed = float.Parse(tokens[8].Trim());
                m.baseCritRate = float.Parse(tokens[9].Trim());
                m.baseCritDamage = float.Parse(tokens[10].Trim());
                m.attackRange = float.Parse(tokens[11].Trim());

                m.attackType = System.Enum.Parse<AttackType>(tokens[12].Trim());

                list.Add(m);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"CSV 라인 {i} 파싱 실패: {ex.Message}");
            }
        }

        Debug.Log($"CSV 로드 성공! 캐릭터 수 = {list.Count}");
        return list;
    }
}