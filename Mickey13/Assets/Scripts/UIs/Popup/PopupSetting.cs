using UnityEngine;
using UnityEngine.UI;

public class PopupSetting : PopupBase
{
    [Header("오디오")]
    [SerializeField] Slider masterVol;
    [SerializeField] Slider sfxVol;
    [SerializeField] Slider bgmVol;
    [SerializeField] Slider charVol;

    [Header("토글 버튼")]
    [SerializeField] ToggleSwitch effectToggle; // 이펙트 토글
    [SerializeField] ToggleSwitch DamageToggle; // 데미지 토글

    [Header("시스템")]
    [SerializeField] Button closeBtn; // 닫기버튼
    public override void Init()
    {
        base.Init();
        closeBtn.onClick.AddListener(OnCloseBtn);
    }

    private void OnEnable()
    {
        var _volumeManager = AudioManager.Instance.VolumeManager;

        //OnValueChanged를 호출하지 않고 슬라이더 값 설정 가능
        masterVol.SetValueWithoutNotify(_volumeManager.MasterVolume);
        bgmVol.SetValueWithoutNotify(_volumeManager.BgmVolume);
        sfxVol.SetValueWithoutNotify(_volumeManager.SfxVolume);
        charVol.SetValueWithoutNotify(_volumeManager.CharacterVolume);

        masterVol.onValueChanged.AddListener(_volumeManager.SetMasterVolume);
        bgmVol.onValueChanged.AddListener(_volumeManager.SetBgmVolume);
        sfxVol.onValueChanged.AddListener(_volumeManager.SetSfxVolume);
        charVol.onValueChanged.AddListener(_volumeManager.SetCharacterVolume);

    }

    private void OnDisable()
    {
        var _volumeManager = AudioManager.Instance.VolumeManager;

        masterVol.onValueChanged.RemoveListener(_volumeManager.SetMasterVolume);
        bgmVol.onValueChanged.RemoveListener(_volumeManager.SetBgmVolume);
        sfxVol.onValueChanged.RemoveListener(_volumeManager.SetSfxVolume);
        charVol.onValueChanged.RemoveListener(_volumeManager.SetCharacterVolume);
    }

    void OnCloseBtn()
    {
        popupAnimation.PlayCloseAnimation(() =>
            UIManager.Instance.Close<PopupSetting>()); 
    }
}
