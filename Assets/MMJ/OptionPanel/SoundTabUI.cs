using UnityEngine;
using UnityEngine.UI;

public class SoundTabUI : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider ambientSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider voiceSlider;

    private const string MASTER_V = "MasterVolume";
    private const string BGM_V = "BGMVolume";
    private const string AMBIENT_V = "AmbientVolume";
    private const string SFX_V = "SFXVolume";
    private const string VOICE_V = "VoiceVolume";

    private void OnEnable()
    {
        InitSliders();
        BindEvents();
    }

    private void OnDisable()
    {
        UnbindEvents();
    }

    private void InitSliders()
    {
        masterSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(MASTER_V, 1f));
        bgmSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(BGM_V, 1f));
        ambientSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(AMBIENT_V, 1f));
        sfxSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(SFX_V, 1f));
        voiceSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(VOICE_V, 1f));
    }

    private void BindEvents()
    {
        masterSlider.onValueChanged.AddListener(v =>
            Manager.Audio.SetMixerVolume(MASTER_V, v));

        bgmSlider.onValueChanged.AddListener(v =>
            Manager.Audio.SetMixerVolume(BGM_V, v));

        ambientSlider.onValueChanged.AddListener(v =>
            Manager.Audio.SetMixerVolume(AMBIENT_V, v));

        sfxSlider.onValueChanged.AddListener(v =>
            Manager.Audio.SetMixerVolume(SFX_V, v));

        voiceSlider.onValueChanged.AddListener(v =>
            Manager.Audio.SetMixerVolume(VOICE_V, v));
    }

    private void UnbindEvents()
    {
        masterSlider.onValueChanged.RemoveAllListeners();
        bgmSlider.onValueChanged.RemoveAllListeners();
        ambientSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        voiceSlider.onValueChanged.RemoveAllListeners();
    }
}
