using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

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

    private static List<string[]> CsvRead(string csvFilePath)
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

    public static Dictionary<DialogKey, Dialog> ParseCsvToDialogs(string csvFilePath)
    {
        var rows = CsvRead(csvFilePath);
        if (rows == null || rows.Count == 0) return null;

        return rows
            .Skip(1)
            .Select(row => new DialogLine(row))
            .GroupBy(x => x.dialogId)
            .ToDictionary(
                g => g.Key, 
                g => new Dialog(g.Key, g.ToList()));
    }
    
    public static Tween FadeInImage(this Image image, float duration = 0.3f)
    {
        Color c = image.color;
        c.a = 0f;
        image.color = c;
        image.gameObject.SetActive(true);

        return image.DOFade(1f, duration)
            .SetEase(Ease.InQuad)
            .SetUpdate(true);
    }

    public static Tween FadeOutImage(this Image image, float duration = 0.3f)
    {
        return image.DOFade(0f, duration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true)
            .OnComplete(() => image.gameObject.SetActive(false));
    }
}
