using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class Util
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (comp == null)
        {
            comp = go.AddComponent<T>();
        }

        return comp;
    }

    public static List<string[]> CsvRead(string csvFilePath)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(csvFilePath);
        if (csvFile == null)
        {
            Debug.LogWarning($"CSV 파일을 찾을 수 없음: {csvFilePath}");
            return null;
        }

        List<string[]> rows = new();
        string[] lines = csvFile.text.Split('\n');
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            rows.Add(line.Trim().Split(','));
        }
        return rows;
    }

    public static async UniTask<Dictionary<string,Dialog>> ParseCsvToDialogs(string csvFilePath)
    {
        return await UniTask.Run(() =>
        {
            var rows = CsvRead(csvFilePath);
            if (rows == null || rows.Count == 0) return null;

            return rows
                .Skip(1)
                .Select(row => new DialogLine(row))
                .GroupBy(x => x.DialogId)
                .ToDictionary(
                    g => g.Key, 
                    g => new Dialog(g.Key, g.ToList()));
        });
    }
}
