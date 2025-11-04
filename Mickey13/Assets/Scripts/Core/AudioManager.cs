using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Controllers")]
    [SerializeField] private BGMController bgmController;
    [SerializeField] private SFXController sfxController;

    [Header("Volume Manager")]
    [SerializeField] private VolumeManager volumeManager;
    public VolumeManager VolumeManager => volumeManager;

    [Header("Audio Data")]
    [SerializeField] private AudioData audioData;

    private AudioClipLoader audioLoader;
    protected override void Awake()
    {
        audioLoader = new AudioClipLoader();
        base.Awake();
        bgmController.Init(audioLoader, audioData);
        sfxController.Init(audioLoader, audioData);
    }

    // BGM 재생 / 정지
    public void PlayBGM(BgmType type) => bgmController.Play(type);
    public void StopBGM(BgmType type) => bgmController.Stop(type);
    public void StopAllBGM() => bgmController.StopAll();

    // SFX 재생
    public void PlaySFX(SfxType type) => sfxController.Play(type);
}
