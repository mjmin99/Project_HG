using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "AudioData", menuName = "Audio/AudioData")]
public class AudioData : ScriptableObject
{
    public AudioClip clipSource;
    public string clipName;
    [Range(0f,1f)] public float volume = 0.9f;
    public AudioClipType audioType;
    public bool loop = false;
}
