using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;
using DG;

public class AudioManager : MonoBehaviour
{
    [Header("Set Audio Database")]
    [SerializeField] private AudioDatabase audioDatabase;
    
    // 믹서와 그룹
    private AudioMixer mixer;    
    private AudioMixerGroup masterMixerGroup;
    private AudioMixerGroup bgmMixerGroup;
    private AudioMixerGroup ambientMixerGroup;
    private AudioMixerGroup sfxMixerGroup;
    private AudioMixerGroup voiceMixerGroup;
    
    // 오디오 소스
    private AudioSource bgmSource;
    private AudioSource ambientSource;
    private AudioSource voiceSource;
    private AudioSource uiSource;
    
    // 오디오 딕셔너리
    private readonly Dictionary<string, AudioData> audioDict = new(); 
    private readonly Dictionary<string, AudioData> voiceDict = new();

    // 볼륨값
    public float MasterVolume { get; private set; }
    public float BGMVolume { get; private set; }
    public float AmbientVolume { get; private set; }
    public float SFXVolume { get; private set; }
    public float VoiceVolume { get; private set; }
    
    // 스트링 해시
    private string masterV = "MasterVolume";
    private string bgmV = "BGMVolume";
    private string ambientV = "AmbientVolume";
    private string sfxV = "SFXVolume";
    private string voiceV = "VoiceVolume";
    
    
    void Awake()
    {
        SetAudioDictionary();
        SetMixerAndSource();
        SetVolumes();
    }

    void Start()
    {
        
    }
    // 딕셔너리 세팅
    private void SetAudioDictionary()
    {
        audioDict.Clear();
        voiceDict.Clear();
        
        foreach (AudioData audio in audioDatabase.audios)
        {
            switch (audio.audioType)
            {
                case AudioClipType.None:
                    Debug.Log($"타입을 지정해주세요: {audio.name}");
                    break;
                case AudioClipType.BGM:
                case AudioClipType.Ambient:
                case AudioClipType.SFX:
                    if (!audioDict.TryAdd(audio.clipName, audio))
                    {
                        Debug.LogWarning($"이미 오디오 딕셔너리에 있는 오디오 클립 이름입니다. : {audio.clipName}");
                    }
                    break;
                case AudioClipType.Voice:
                    if (!voiceDict.TryAdd(audio.clipName, audio))
                    {
                        Debug.LogWarning($"이미 보이스 딕셔너리에 있는 오디오 클립 이름입니다. : {audio.clipName}");
                    }
                    break;
            }
        }
    }

    // 믹서그룹, 오디오 소스 설정
    private void SetMixerAndSource()
    {
        // 믹서, 그룹 설정
        mixer = Resources.Load<AudioMixer>("Audio/" + nameof(AudioMixer));
        masterMixerGroup = mixer.FindMatchingGroups("Master")[0];
        bgmMixerGroup = mixer.FindMatchingGroups("Master/BGM")[0];
        ambientMixerGroup = mixer.FindMatchingGroups("Master/Ambient")[0];
        sfxMixerGroup = mixer.FindMatchingGroups("Master/SFX")[0];
        voiceMixerGroup = mixer.FindMatchingGroups("Master/Voice")[0];
        
        // 소스 설정
        bgmSource = gameObject.GetOrAddComponent<AudioSource>();
        bgmSource.outputAudioMixerGroup = bgmMixerGroup;
        ambientSource = gameObject.AddComponent<AudioSource>();
        ambientSource.outputAudioMixerGroup = ambientMixerGroup;
        voiceSource = gameObject.AddComponent<AudioSource>();
        voiceSource.outputAudioMixerGroup = voiceMixerGroup;
        uiSource = gameObject.AddComponent<AudioSource>();
        uiSource.outputAudioMixerGroup = sfxMixerGroup;
    }
    
    // 초기 볼륨 세팅
    private void SetVolumes()
    {
        MasterVolume = PlayerPrefs.GetFloat(masterV, 1f);
        BGMVolume = PlayerPrefs.GetFloat(bgmV, 1f);
        AmbientVolume = PlayerPrefs.GetFloat(ambientV, 1f);
        SFXVolume = PlayerPrefs.GetFloat(sfxV, 1f);
        VoiceVolume = PlayerPrefs.GetFloat(voiceV, 1f);

        mixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(MasterVolume, 0.0001f, 1f)) * 20);
        mixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Clamp(BGMVolume, 0.0001f, 1f)) * 20);
        mixer.SetFloat("AmbientVolume", Mathf.Log10(Mathf.Clamp(AmbientVolume, 0.0001f, 1f)) * 20);
        mixer.SetFloat("SFXVolume",  Mathf.Log10(Mathf.Clamp(SFXVolume, 0.0001f, 1f)) * 20);
        mixer.SetFloat("VoiceVolume",  Mathf.Log10(Mathf.Clamp(VoiceVolume, 0.0001f, 1f)) * 20);
    }


    // 오디오 전환
    public void SwapClip(AudioClipType clipType, string clipName)
    {
        AudioSource source;
        AudioData data;
        
        switch (clipType)
        {
            case AudioClipType.None:
                Debug.LogWarning("클립 타입이 지정안되어있음");
                return;
            case AudioClipType.BGM:
                source = bgmSource;
                break;
            case AudioClipType.Ambient:
                source = ambientSource;
                break;
            default:
                Debug.LogWarning("클립 타입을 확인해주세요");
                return;
        }
        
        data = audioDict[clipName];
        if (data.audioType != clipType)
        {
            Debug.LogWarning($"입력한 오디오 클립 타입과 오디오 클립이 서로 맞지 않습니다. 입력타입: {clipType}, 데이터 타입: {data.audioType}");
            return;
        }
        
        DOTween.Kill(source); // 기존 Tween 종료 (중복 수행 방지)
        
        // 플레이 중일 때 전환
        if (source.isPlaying)
        {
            Sequence seq = DOTween.Sequence();
            seq.AppendCallback(() => FadeOut(source))
                .AppendCallback(() =>
                {
                    source.clip = data.clipSource;
                    source.loop = data.loop;
                })
                .AppendCallback(() => FadeIn(source, data.volume))
                .SetEase(Ease.Linear);
        }
        // 플레이 중이 아니라면
        else
        {
            FadeIn(source,data.volume);
        }
    }
    
    public void SetMixerVolume(string key, float value)
    {
        float tmp = Mathf.Log10(Mathf.Clamp(value,0.0001f, 1f));
        mixer.SetFloat(key, tmp);
    }
    
    // 소리 전부 죽이기
    public void KillSound()
    {
        FadeOut(bgmSource);
        FadeOut(ambientSource);
        FadeOut(voiceSource);
    }
    
    // 페이드 인-아웃
    private void FadeIn(AudioSource source, float setVolume, float duration = 1f)
    {
        source.DOFade(setVolume, duration).SetEase(Ease.OutSine);
    }

    private void FadeOut(AudioSource source, float duration = 1f)
    {
        source.DOFade(0f, duration).SetEase(Ease.InSine);
    }

}
