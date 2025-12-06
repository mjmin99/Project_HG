using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string SAVE_FILE = "save.json";

    public SaveData CurrentData { get; private set; }

    private void Awake()
    {
        Load();
    }

    public void Load()
    {
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILE);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            CurrentData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            CurrentData = new SaveData();
            Save();
        }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(CurrentData, true);
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILE);
        File.WriteAllText(path, json);
    }
}
