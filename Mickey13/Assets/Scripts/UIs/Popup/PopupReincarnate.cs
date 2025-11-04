using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupReincarnate : PopupBase
{
    [SerializeField] Image image; // 환생 이미지
    [SerializeField] TextMeshProUGUI maxDistance; // 최대 도달 거리
    [SerializeField] TextMeshProUGUI getItSoulStoneAmount; // 획득 가능한 영혼석 양

    [SerializeField] Button reincanateBtn;
    [SerializeField] Button exitBtn;
    [SerializeField] Button adBtn;

    private void OnEnable()
    {
        ClearPopup();
        
        EventManager.Instance.StartListening(EventType.EndReincarnate,ClearPopup);
        reincanateBtn.onClick.AddListener(OnClickYes);
        exitBtn.onClick.AddListener(OnClickNo);
        adBtn.onClick.AddListener(OnClickAd);
        Setup();
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        EventManager.Instance.StopListening(EventType.EndReincarnate,ClearPopup);
        reincanateBtn.onClick.RemoveListener(OnClickYes);
        exitBtn.onClick.RemoveListener(OnClickNo);
        adBtn.onClick.RemoveListener(OnClickAd);
        ClearPopup();
        Time.timeScale = GameManager.Instance.SpeedLevel;
    }
    
    private void Setup()
    {
        //TODO 전투 중 최대 달성 거리 정보, 획득 가능한 영혼석 양 정보 가져오기
        //2025-10-23 해결
        maxDistance.text = $"최대 도달 거리  {User.Instance.ReincarnateData.MaxDistance} KM";                  
        // 여기서는 임시 값을 보여줌, 저장 X
        getItSoulStoneAmount.text = $" 획득 영혼석 : {User.Instance.soulStone_will_receved}";
    }

    private void OnClickYes()
    {
        // 환생 시스템 실행
        Debug.Log("환생 실행");
        EventManager.Instance.TriggerEvent(EventType.StartReincarnate);
        UIManager.Instance.Close<PopupReincarnate>();
        popupAnimation.PlayCloseAnimation(() =>
            UIManager.Instance.Open<PopupGameResult>()
                .ShowResult(User.Instance.ReincarnateData)); 
    }

    private void OnClickNo()
    {
        popupAnimation.PlayCloseAnimation(() =>
            UIManager.Instance.Close<PopupReincarnate>()); 
    }

    private void OnClickAd()
    {
        //todo 광고실행
        Debug.Log("광고실행");
    }

    private void ClearPopup()
    {
        maxDistance.text = $"최대 도달 거리 : 0 KM";                  
        getItSoulStoneAmount.text = $" 획득 영혼석 : 0";
    }
    
}
