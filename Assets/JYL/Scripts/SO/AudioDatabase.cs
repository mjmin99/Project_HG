using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioDB", menuName = "Audio/Audio Database")]
public class AudioDatabase : ScriptableObject
{
    [Header("Set AudioData Path")]
    [SerializeField] private string audioPath = "Audio/";

    public AudioData[] audios;
    
    [ContextMenu("Set Database")]
    public void SetDatabase()
    {
        audios = Resources.LoadAll<AudioData>(audioPath);
    }
}
