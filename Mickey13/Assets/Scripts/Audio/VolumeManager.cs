using DG.Tweening;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mixer;
    public AudioMixer Mixer => mixer;

    public const string MASTER_VOLUME = "MasterVolume";
    public const string BGM_VOLUME = "BGMVolume";
    public const string SFX_VOLUME = "SFXVolume";
    public const string CHARACTER_VOLUME = "CharacterVolume";

    public float MasterVolume { get; private set; }
    public float BgmVolume { get; private set; }
    public float SfxVolume { get; private set; }
    public float CharacterVolume { get; private set; }

    public event Action<float> OnMasterVolumeChanged;
    public event Action<float> OnBgmVolumeChanged;
    public event Action<float> OnSfxVolumeChanged;
    public event Action<float> OnCharacterVolumeChanged;

    private void Start()
    {
        LoadAllVolume();
    }

    private void LoadAllVolume()
    {
        MasterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME, 1.0f);
        BgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME, 0.75f);
        SfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME, 0.75f);
        CharacterVolume = PlayerPrefs.GetFloat(CHARACTER_VOLUME, 0.75f);

        mixer.SetFloat(MASTER_VOLUME, ConvertToDecibel(MasterVolume));
        mixer.SetFloat(BGM_VOLUME, ConvertToDecibel(BgmVolume));
        mixer.SetFloat(SFX_VOLUME, ConvertToDecibel(SfxVolume));
        mixer.SetFloat(CHARACTER_VOLUME, ConvertToDecibel(CharacterVolume));
    }

    public void SetMasterVolume(float volume)
    {
        MasterVolume = volume;
        SetVolume(MASTER_VOLUME, volume);
        OnMasterVolumeChanged?.Invoke(MasterVolume);
    }
    public void SetBgmVolume(float volume)
    {
        BgmVolume = volume;
        SetVolume(BGM_VOLUME, volume);
        OnBgmVolumeChanged?.Invoke(BgmVolume);
    }
    public void SetSfxVolume(float volume)
    {
        SfxVolume = volume;
        SetVolume(SFX_VOLUME, volume);
        OnSfxVolumeChanged?.Invoke(SfxVolume);
    }
    public void SetCharacterVolume(float volume)
    {
        CharacterVolume = volume;
        SetVolume(CHARACTER_VOLUME, volume);
        OnCharacterVolumeChanged?.Invoke(CharacterVolume);
    }

    private void SetVolume(string key, float volume) // 볼륨 값 저장
    {
        mixer.SetFloat(key, ConvertToDecibel(volume));
        PlayerPrefs.SetFloat(key, volume);
    }

    private float ConvertToDecibel(float volume)
    {
        return volume <= 0.0001f ? -80f : Mathf.Log10(volume) * 20;
    }
}
