using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class GachaBanner : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bannerName; // 배너 이름
    [SerializeField] TextMeshProUGUI pityText; // 천장
    [SerializeField] Image bannerImage; // 배너 이미지
    [SerializeField] Button singleGachaBtn; // 1회 뽑기
    [SerializeField] Button multiGachaBtn; // 10회 뽑기
    [SerializeField] Button probabilityBtn; // 확률 확인 버튼

    [SerializeField] Image singleCurrencyIcon;
    [SerializeField] Image multiCurrencyIcon;
    [SerializeField] Sprite diamond;
    [SerializeField] Sprite soulStone;

    private GachaBannerSO _bannerSO;
    private GachaType currentGachaType;

    public event Action<GachaBannerSO, int> OnGachaBtnClicked; // 뽑기 이벤트 추가 <뽑기 종류, 뽑기 횟수, 뽑기 머신>

    public void InitGachaBanner()
    {
        singleGachaBtn.onClick.AddListener(OnClickSingle);
        multiGachaBtn.onClick.AddListener(OnlickMulti);
        probabilityBtn.onClick.AddListener(OnClickProbability);

        if (GachaManager.Instance != null)
        {
            GachaManager.Instance.OnPityCountUpdated -= HandlePityUpdate;
            GachaManager.Instance.OnPityCountUpdated += HandlePityUpdate;
        }
    }

    public void SetBanner(GachaBannerSO bannerSO)
    {
        _bannerSO = bannerSO;

        bannerName.text = bannerSO.Name;
        bannerImage.sprite = bannerSO.BannerSprite;
        UpdatePityCount();

        switch (bannerSO.CurrencyType)
        {
            case CurrencyType.SoulStone:
                singleCurrencyIcon.sprite = soulStone;
                multiCurrencyIcon.sprite = soulStone;
                break;
            case CurrencyType.Diamond:
                singleCurrencyIcon.sprite = diamond;
                multiCurrencyIcon.sprite = diamond;
                break;
        }
        currentGachaType = bannerSO.GachaType;


        //bannerImage.sprite = Resources.Load<Sprite>(data.BannerSpritePath); // Resources 폴더에 배너 이미지가 위치해있는 경로
        //gachaType = bannerSO.GachaType;
        //gachaMachine = bannerSO.GachaMachine;
    }

    private void HandlePityUpdate(GachaType type)
    {
        if(type == this.currentGachaType)
            UpdatePityCount();
    }
    
    private void UpdatePityCount()
    {
        int pityLimit = _bannerSO.GachaMachine.PityLimit;
        int currentPityCount = GachaManager.Instance.GetCurrentPityCount(_bannerSO.GachaType);
        pityText.text = $"확정 3등급까지 남은 횟수 : {currentPityCount} / {pityLimit}";
    }

    private void OnClickSingle() // 1회 뽑기
    {
        UIManager.Instance.Open<PopupConfirm>()
            .ShowConfirm(" 1회 뽑기", "1회 뽑기를 실행하시겠습니까?", () =>
                {
                    Debug.Log($"1회 뽑기 실행");
                    OnGachaBtnClicked?.Invoke(_bannerSO, 1);
                    UpdatePityCount();
                });
    }

    private void OnlickMulti() // 10회 뽑기
    {
        UIManager.Instance.Open<PopupConfirm>()
            .ShowConfirm("10회 뽑기", "10회 뽑기를 실행하시겠습니까?", () =>
            {
                Debug.Log($"{_bannerSO.GachaType} 10회 뽑기 실행");
                OnGachaBtnClicked?.Invoke(_bannerSO, 10);
                UpdatePityCount();
            });
    }

    private void OnClickProbability() // 확률 정보 확인
    {
        Debug.Log($"{_bannerSO.GachaType} 확률 정보 표시)");
        // TODO : 확률정보 확인할 수 있는 무언가 실행
    }


}