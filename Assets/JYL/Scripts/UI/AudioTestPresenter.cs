using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class AudioTestPresenter : MonoBehaviour
{
    [Header("Set Manager")]
    [SerializeField] private AudioManager audioManager;
    
    [Header("Set UI References")]
    [SerializeField] private Slider[]  sliders;
    [SerializeField] private TMP_Dropdown[]  dropdowns;
    [SerializeField] private TMP_Text[] valueTexts;
    [SerializeField] private Button[] buttons;

    private AudioData[] audioArr;

    private string masterV = "MasterVolume";
    private string bgmV = "BGMVolume";
    private string ambV = "AmbientVolume";
    private string sfxV = "SFXVolume";
    private string voiceV = "VoiceVolume";
    
    private void Start()
    {
        audioArr = Resources.LoadAll<AudioData>("Audio/");
        
        for (int i = 0; i < 5; i++)
        {
            int index = i;
            switch (i)
            {
                case 0:
                    sliders[i].value = PlayerPrefs.GetFloat(masterV, 1f);
                    break;
                case 1:
                    sliders[i].value = PlayerPrefs.GetFloat(bgmV, 1f);
                    break;
                case 2:
                    sliders[i].value = PlayerPrefs.GetFloat(ambV, 1f);
                    break;
                case 3:
                    sliders[i].value = PlayerPrefs.GetFloat(sfxV, 1f);
                    break;
                case 4:
                    sliders[i].value = PlayerPrefs.GetFloat(voiceV, 1f);
                    break;
            }
            valueTexts[i].text = (sliders[i].value * 100).ToString("N0");
            sliders[i].onValueChanged.AsObservable().Skip(1).Subscribe(x =>
            {
                SetAudioVolume(index, x);
                valueTexts[index].text = (x*100).ToString("N0");
            }).AddTo(this);
            if (i > 3) break;
            buttons[i].OnClickAsObservable().Subscribe(x => _ = OnClickButton(index)).AddTo(this);
        }
        buttons[4].OnClickAsObservable().Subscribe(x => _ = audioManager.KillSound()).AddTo(this);
        SetDropdowns();
    }

    private void UpdateUI()
    {
        SetDropdowns();
    }

    private void SetDropdowns()
    {
        foreach (var dropdown in dropdowns)
        {
            dropdown.ClearOptions();
        }
        
        foreach (var t in audioArr)
        {
            var option = new TMP_Dropdown.OptionData
            {
                text = t.clipName
            };
            switch (t.audioType)
            {
                case AudioClipType.BGM:
                    dropdowns[0].options.Add(option);
                    break;
                case AudioClipType.Ambient:
                    dropdowns[1].options.Add(option);
                    break;
                case AudioClipType.SFX:
                    dropdowns[2].options.Add(option);
                    break;
                case AudioClipType.Voice:
                    dropdowns[3].options.Add(option);
                    break;
                case AudioClipType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (var d in dropdowns)
            {
                if (d.options.Count > 0) d.captionText.text = d.options[d.value].text;
            }
        }
    }

    private void SetAudioVolume(int index, float volume)
    {
        switch (index)
        {
            case 0:
                audioManager.SetMixerVolume(masterV, volume);
                break;
            case 1:
                audioManager.SetMixerVolume(bgmV, volume);
                break;
            case 2:
                audioManager.SetMixerVolume(ambV, volume);
                break;
            case 3:
                audioManager.SetMixerVolume(sfxV, volume);
                break;
            case 4:
                audioManager.SetMixerVolume(voiceV, volume);
                break;
        }
    }

    private async UniTask OnClickButton(int index)
    {
        int valueIndex = dropdowns[index].value;
        string clipName = dropdowns[index].options[valueIndex].text;
        
        switch (index)
        {
            case 0:
                await audioManager.SwapClip(AudioClipType.BGM, clipName);
                break;
            case 1:
                await audioManager.SwapClip(AudioClipType.Ambient, clipName);
                break;
            case 2:
                audioManager.PlaySFX(clipName);
                break;
            case 3:
                audioManager.PlayVoice(clipName);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
