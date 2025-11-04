using DG.Tweening;
using System;
using UnityEngine;

public class PopupAnimation : MonoBehaviour
{
    [Header("애니메이션 설정")]
    [SerializeField] private GameObject popupGroup;
    [SerializeField] private float startOffsetY = 50f;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease moveEase = Ease.OutBack; // 통통 튀는 느낌

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    
    private void Awake()
    {
        canvasGroup = popupGroup.GetComponent<CanvasGroup>();
        rectTransform = popupGroup.GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition; // 원래 위치 저장
    }
    
    private void OnEnable()
    {
        PlayOpenAnimation();
    }

    private void PlayOpenAnimation()
    {
        // 초기 설정
        canvasGroup.alpha = 0f; // 투명도 0
        rectTransform.anchoredPosition = originalPosition + new Vector2(0, startOffsetY); // 생성될 위치로 팝업 위치

        // 기존 실행 애니매이션 정지
        rectTransform.DOKill();
        canvasGroup.DOKill();

        // 애니매이션 실행
        canvasGroup.DOFade(1f, duration).SetEase(Ease.OutQuad).SetUpdate(true); // 페이드인
        rectTransform.DOAnchorPos(originalPosition, duration).SetEase(moveEase).SetUpdate(true); // 원래 위치로 이동
    }

    public void PlayCloseAnimation(Action onCompleteAction)
    {
        rectTransform.DOKill();
        canvasGroup.DOKill();
        
        rectTransform.DOAnchorPos(originalPosition - new Vector2(0, startOffsetY), duration).SetEase(moveEase);
        canvasGroup.DOFade(0f, duration/2)
            .SetEase(Ease.OutBack)
            .SetUpdate(true)
            .OnComplete(() => onCompleteAction?.Invoke()); // 애니매이션이 끝난 뒤 팝업 비활성화
    }
}
