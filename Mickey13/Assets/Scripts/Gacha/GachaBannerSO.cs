using UnityEngine;

public enum GachaType
{
    Standard,
    Event,
}

[CreateAssetMenu(fileName = "GachaBannerData", menuName = "Gacha/BannerData")]
public class GachaBannerSO : GameData
{
    [Header("UI 정보")]
    [SerializeField] private Sprite bannerSprite; // 배너 이미지 스프라이트
    //[SerializeField] private String bannerSpritePath; // 배너 이미지 스프라이트 위치. 나중에 엑셀로 SO를 만들때는 주소 필요

    [Header("가챠 머신 정보")]
    [SerializeField] private GachaType gachaType; // 뽑기 종류(상시, 이벤트)
    [SerializeField] private GachaMachine gachaMachine; // 뽑기 종류별 머신

    [Header("소비 재화 정보")]
    [SerializeField] CurrencyType currencyType;
    [SerializeField] private int gachaCost;

    public CurrencyType CurrencyType => currencyType;
    public int GachaCost => gachaCost;
    public Sprite BannerSprite => bannerSprite;
    //public String BannerSpritePath => bannerSpritePath;
    public GachaType GachaType => gachaType;
    public GachaMachine GachaMachine => gachaMachine;
}
