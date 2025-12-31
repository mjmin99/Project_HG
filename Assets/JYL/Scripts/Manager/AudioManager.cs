using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class AudioManager : Singleton<AudioManager>
{
    /// <summary>
    /// SwapClip : BGM, Ambient 전환 시 사용. Fade 기능 적용됨
    /// PlaySFX : 효과음 재생 시 사용
    /// PlayVoice : 음성 재생 시 사용
    /// SetMixerVolume : 설정 창에서 믹서 볼륨 조절에 사용
    /// KillSound : 모든 소리 재생 종료
    /// </summary>
    private AudioDatabase audioDatabase;

    [Header("Set Ease Type")] 
    [SerializeField] private Ease easeInType = Ease.InSine;
    [SerializeField] private Ease easeOutType = Ease.OutSine;
    
    // 믹서와 그룹
    private AudioMixer mixer;    
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
    private float MasterVolume { get; set; }
    private float BGMVolume { get; set; }
    private float AmbientVolume { get; set; }
    private float SfxVolume { get; set; }
    private float VoiceVolume { get; set; }
    
    // 볼륨 스트링 캐싱
    private const string MASTER_V = "MasterVolume";
    private const string BGM_V = "BGMVolume";
    private const string AMBIENT_V = "AmbientVolume";
    private const string SFX_V = "SFXVolume";
    private const string VOICE_V = "VoiceVolume";

    #region 라이프 사이클
    protected override void Awake()
    {
        base.Awake();
        audioDatabase = Resources.Load<AudioDatabase>("Audio/AudioDB");
        SetAudioDictionary();
        SetMixerAndSource();
        SetVolumes();
    }

    void Start()
    {
        SetMixerVolume();
        Manager.Audio.SwapClip(AudioClipType.BGM, "MainBGM").Forget();
    }
    #endregion
    
    #region 초기화
    // 딕셔너리 세팅
    private void SetAudioDictionary()
    {
        audioDict.Clear();
        voiceDict.Clear();
        
        foreach (AudioData audioData in audioDatabase.audios)
        {
            switch (audioData.audioType)
            {
                case AudioClipType.None:
                    Debug.Log($"타입을 지정해주세요: {audioData.name}");
                    break;
                case AudioClipType.BGM:
                case AudioClipType.Ambient:
                case AudioClipType.SFX:
                    if (!audioDict.TryAdd(audioData.clipName, audioData))
                    {
                        Debug.LogWarning($"이미 오디오 딕셔너리에 있는 오디오 클립 이름입니다. : {audioData.clipName}");
                    }
                    break;
                case AudioClipType.Voice:
                    if (!voiceDict.TryAdd(audioData.clipName, audioData))
                    {
                        Debug.LogWarning($"이미 보이스 딕셔너리에 있는 오디오 클립 이름입니다. : {audioData.clipName}");
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
        MasterVolume = PlayerPrefs.GetFloat(MASTER_V, 1f);
        BGMVolume = PlayerPrefs.GetFloat(BGM_V, 1f);
        AmbientVolume = PlayerPrefs.GetFloat(AMBIENT_V, 1f);
        SfxVolume = PlayerPrefs.GetFloat(SFX_V, 1f);
        VoiceVolume = PlayerPrefs.GetFloat(VOICE_V, 1f);
    }

    private void SetMixerVolume()
    {
        mixer.SetFloat(MASTER_V, Mathf.Log10(Mathf.Clamp(MasterVolume, 0.0001f, 1f)) * 20);
        mixer.SetFloat(BGM_V, Mathf.Log10(Mathf.Clamp(BGMVolume, 0.0001f, 1f)) * 20);
        mixer.SetFloat(AMBIENT_V, Mathf.Log10(Mathf.Clamp(AmbientVolume, 0.0001f, 1f)) * 20);
        mixer.SetFloat(SFX_V, Mathf.Log10(Mathf.Clamp(SfxVolume, 0.0001f, 1f)) * 20);
        mixer.SetFloat(VOICE_V, Mathf.Log10(Mathf.Clamp(VoiceVolume, 0.0001f, 1f)) * 20);
    }
    #endregion
    
    // 오디오 전환
    public async UniTask SwapClip(AudioClipType clipType, string clipName)
    {
        AudioSource source;

        switch (clipType)
        {
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

        if (!audioDict.TryGetValue(clipName, out var data))
        {
            Debug.LogWarning("클립이 없음");
            return;
        }

        if (source.isPlaying && source.clip == data.clipSource) return;
        if (DOTween.IsTweening(source)) return;
        DOTween.Kill(source); // 기존 Tween 종료 (중복 수행 방지)
        
        // 플레이 중일 때 전환
        // FadeOut => 클립교체 => FadeIn
        if (source.isPlaying)
        {
            await FadeOut(source);
            
            source.clip = data.clipSource;
            source.loop = data.loop;
            source.Play();
                    
            await FadeIn(source,data.volume);
        }
        // 플레이 중이 아니라면
        else
        {
            source.clip = data.clipSource;
            source.loop = data.loop;
            source.volume = 0f;
            await FadeIn(source,data.volume);
        }
    }
    
    // 효과음 재생
    public float PlaySfx(string clipName)
    {
        if (!audioDict.TryGetValue(clipName, out AudioData data))
        {
            Debug.LogWarning($"딕셔너리 안에 해당 클립이 없음.{clipName}");
            return 0f;
        }
        
        GameObject go = new GameObject(clipName);
        AudioSource source = go.AddComponent<AudioSource>();
        
        source.clip = data.clipSource;
        source.loop = data.loop;
        source.volume = data.volume;
        source.outputAudioMixerGroup = sfxMixerGroup;
        
        source.Play();
        if(!data.loop) Destroy(go, data.clipSource.length);
        return data.clipSource.length;
    }
    
    // 음성 재생
    public void PlayVoice(string clipName)
    {
        if (!voiceDict.TryGetValue(clipName, out var data))
        {
            Debug.LogWarning($"딕셔너리안에 해당 클립이 없음:{clipName}");
            return;
        }

        if (voiceSource.isPlaying && voiceSource.clip == data.clipSource) return;
        
        voiceSource.clip = data.clipSource;
        voiceSource.loop = data.loop;
        voiceSource.volume = data.volume;
        
        voiceSource.Play();
        
    }

    public void StopVoice()
    {
        voiceSource.Stop();
    }
    
    // 믹서 볼륨 조절
    public void SetMixerVolume(string key, float value)
    {
        float tmp = Mathf.Log10(Mathf.Clamp(value,0.0001f, 1f)) * 20;
        mixer.SetFloat(key, tmp);
        PlayerPrefs.SetFloat(key, value);
    }
    
    // 소리 전부 죽이기
    public async UniTask KillSound()
    {
        await UniTask.WhenAll(
            FadeOut(bgmSource),
            FadeOut(ambientSource),
            FadeOut(voiceSource));
    }
    
    #region 내부 기능
    // 페이드 인-아웃
    private async UniTask FadeIn(AudioSource source, float setVolume, float duration = 1f)
    {
        source.Play();
        await source.DOFade(setVolume, duration)
            .SetEase(easeInType)
            .AsyncWaitForCompletion();
    }

    private async UniTask FadeOut(AudioSource source, float duration = 1f)
    {
        await source.DOFade(0f, duration)
            .SetEase(easeOutType)
            .AsyncWaitForCompletion();
        source.Stop();
    }
    #endregion
}
