using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupGachaResult : PopupBase
{
    [Header("SlotUI")]
    [SerializeField] GachaResultSlot gachaResultPrefab; // 가챠 결과를 보여줄 개별슬롯 프리팹
    [SerializeField] Transform content; // 슬롯이 나타날 위치

    [Header("Buttons")]
    [SerializeField] Button skipBtn; // 뽑기결과 건너뛰기
    [SerializeField] Button exitBtn; // 결과창 종료
    [SerializeField] Button retryBtn; // 다시 뽑기

    private List<GachaResultSlot> slots = new List<GachaResultSlot>(); // 임시 생성한 슬롯 저장용
    private int revealCount; //뽑기 결과 확인 갯수 캐싱

    public event Action OnRetryClicked; // 다시뽑기 버튼 클릭 이벤트
    
    private void OnEnable()
    {
        skipBtn.onClick.AddListener(OnClickSkip); // 건너뛰기 버튼
        exitBtn.onClick.AddListener(OnClickExit); // 나가기 버튼
        retryBtn.onClick.AddListener(OnClickRetry); // 다시뽑기 버튼
    }

    private void OnDisable()
    {
        skipBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.RemoveAllListeners();
        retryBtn.onClick.RemoveAllListeners(); 
    }

    public void ShowResults(List<ISlotUIData> results) //뽑기 결과 표시
    {
        ClearSlots(); // 슬롯 초기화
        revealCount = 0;
        exitBtn.interactable = false;
        retryBtn.interactable = false;
        skipBtn.interactable = true;

        foreach (var result in results)
        {
            // 뽑기 슬롯 오브젝트풀
            GameObject obj = PoolingManager.Instance.Get(gachaResultPrefab.gameObject, content);
            var slot = obj.GetComponent<GachaResultSlot>(); // 캐릭터 슬롯 생성
            slots.Add(slot);
            slot.Setup(result, OnSlotRevealed);
        }
    }

    public void RefreshResults(List<ISlotUIData> newResults) //새 뽑기 결과 표시
    {
        for (int i = 0; i < newResults.Count; i++)
        {
            if (i < slots.Count)
            {
                slots[i].ClearUI();
                slots[i].Setup(newResults[i], OnSlotRevealed);
            }
        }
    }

    private void ClearSlots() // 뽑기 목록 초기화
    {
        if(slots.Count == 0) return;
        foreach( var slot in slots)
        {
            slot.ClearUI();
            PoolingManager.Instance.Release(slot.gameObject);
        }
        slots.Clear();
    }

    private void OnClickRetry()
    {
        revealCount = 0;
        skipBtn.interactable = true;
        exitBtn.interactable = false;
        retryBtn.interactable = false;
        OnRetryClicked?.Invoke();
        Debug.Log("다시 뽑기 시작");
    }

    private void OnClickExit()
    {
        popupAnimation.PlayCloseAnimation(() =>
            UIManager.Instance.Close<PopupGachaResult>()); 
    }

    private void OnClickSkip()
    {
        Debug.Log("뽑기 결과 건너뛰기");
        skipBtn.interactable = false;
        
        bool hasPlayedSkipSound = false; // 이미 소리가 재생됐는지 여부 체크용
        
        foreach (var slot in slots)
        {
            if(slot.IsAlreadyRevealed) continue; // 슬롯이 열려있으면 스킵

            bool playSoundForThisSlot = false;  // 사운드 재생 키워드
            
            // 사운드가 플레이된적 없다면 플레이
            if (!hasPlayedSkipSound)
            {
                playSoundForThisSlot = true;
                hasPlayedSkipSound = true;
            }
            slot.Reveal(playSoundForThisSlot);
        }
    }

    private void OnSlotRevealed()
    {
        revealCount++;
        if(revealCount >= slots.Count )
        {
            exitBtn.interactable = true;
            skipBtn.interactable = false;
            retryBtn.interactable = true;
        }
    }

}
