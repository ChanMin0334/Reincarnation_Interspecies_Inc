using System.Collections.Generic;
using System.Collections;
using TS.PageSlider;
using UnityEngine;

public class GachaPanel : MonoBehaviour
{
    [Header("MainGachaBanner")]
    [SerializeField] PageSlider pageSlider;
    [SerializeField] PageScroller pageScroller;
    [SerializeField] GachaBanner bannerPrefab;

    private List<GachaBannerSO> bannerSOs;
    private PopupGachaResult curGachaResultView; // 현재 뽑기 결과창 캐싱
    private GachaBannerSO curBannerSO; // 현재 배너SO 캐싱
    private int lastGachaCount;  // 현재 뽑기 횟수 캐싱s
    
    private bool isInitialized = false;
    
    private void OnEnable()
    {
        if (isInitialized) return; 
        
        bannerSOs = GachaManager.Instance.GetActiveGachaBanners();
        
        StartCoroutine(SetupPanelRoutine());

        isInitialized = true;
        GameManager.Instance.Tutorial.GachaTutorial();
    }

    private IEnumerator SetupPanelRoutine() // 실행 순서 null오류 대응 코루틴
    {
        yield return null; 
        
        pageSlider.Clear();
        CreateGachaBanner(); 

        pageScroller.OnPageChangeEnded.AddListener(OnBannerChanged);

        if (bannerSOs != null && bannerSOs.Count > 0)
        {
            UpdateBanner(0);
        }
    }

    private void CreateGachaBanner()
    {
        // 배너 생성 위치
        var contentParent = pageScroller.Content;

        foreach (var data in bannerSOs)
        {
            // 배너 생성
            var banner = Instantiate(bannerPrefab, contentParent);
            banner.name = $"Banner_{data.GachaType}"; // Hierarchy에서 알아보기 쉽게 이름 변경

            // 배너 데이터 주입 및 이벤트 등록
            banner.InitGachaBanner();
            banner.SetBanner(data);
            banner.OnGachaBtnClicked += HandleGachaBtnClicked;

            //page 추가
            pageSlider.AddPage(banner.GetComponent<RectTransform>());
        }
    }
    private void UpdateBanner(int index) // 메인배너 업데이트
    {
        if (index < 0 || index >= bannerSOs.Count) return;

        curBannerSO = bannerSOs[index];
        // PageSlider 페이지 이동
        pageScroller.SetPage(index);

    }

    private void OnBannerChanged(int from, int to)
    {
        if (to >= 0 && to < bannerSOs.Count)
        {
            curBannerSO = bannerSOs[to];
        }
            
    }

    // 뽑기 결과 UI 표시
    private void HandleGachaBtnClicked(GachaBannerSO bannerSO, int count)
    {
        lastGachaCount = count; // 누른 버튼의 뽑기 횟수 캐싱(다시뽑기 횟수때 사용)

        var gachaResults = GachaManager.Instance.ReturnGachaResult(bannerSO, count);
        
        if(curGachaResultView != null)
            curGachaResultView.OnRetryClicked -= OnRetryGacha;
        
        if (gachaResults != null && gachaResults.Count > 0)
        {
            curGachaResultView = UIManager.Instance.Open<PopupGachaResult>();
            curGachaResultView.ShowResults(gachaResults);
            curGachaResultView.OnRetryClicked += OnRetryGacha;
        }
    }

    private void OnRetryGacha() // 다시뽑기 버튼 클릭시 실행
    {
        if (bannerSOs == null) return;
        
        var newResults = GachaManager.Instance.ReturnGachaResult(curBannerSO, lastGachaCount);
        if (newResults.Count > 0)
        {
            curGachaResultView.RefreshResults(newResults);
        }
    }
}
