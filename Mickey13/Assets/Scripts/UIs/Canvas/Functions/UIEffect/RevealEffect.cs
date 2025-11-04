using UnityEngine;
using DG.Tweening;

public enum RevealType
{
    Slide,
    Roll,
}

public class RevealEffect : MonoBehaviour
{
    [SerializeField] GameObject backImage; // 뒷면
    [SerializeField] GameObject frontImage; // 앞면

    [SerializeField] RectTransform maskRect; // 슬라이드
    [SerializeField] RectTransform background; // 롤

    [SerializeField] float duration = 0.5f;
    [SerializeField] Ease easeType = Ease.InOutQuad;
    [SerializeField] RevealType revelType = RevealType.Slide;

    private bool isRevealed = false;

    private Vector2 initMaskSize; // 초기 사이즈 캐싱
    private Vector2 initbackgroundSize; // 초기 사이즈 캐싱

    private void Awake()
    {
        initMaskSize = maskRect.sizeDelta;
        initbackgroundSize = background.localScale;
    }

    public void Reveal()
    {
        if (isRevealed) return;
        isRevealed = true;

        switch (revelType)
        {
            case RevealType.Slide:
                DoSlideReveal();
                break;
            case RevealType.Roll:
                DoRollReveal();
                break;
        }
    }
    public void ResetEffect()
    {
        isRevealed = false;

        if (maskRect != null) DOTween.Kill(maskRect);
        if (background != null) DOTween.Kill(background);

        maskRect.sizeDelta = initMaskSize;
        maskRect.localScale = initbackgroundSize;

        backImage.SetActive(true);
        frontImage.SetActive(false);

    }

    private void DoSlideReveal()
    {
        maskRect.DOSizeDelta(new Vector2(0, maskRect.sizeDelta.y), duration)
            .SetEase(easeType)
            .OnComplete(ShowFront);
    }

    private void DoRollReveal()
    {
        background.DOScaleX(0, duration)
            .SetEase(easeType)
            .OnComplete(ShowFront);  
    }   
    
    private void ShowFront()
    {
        backImage.SetActive(false);
        frontImage.SetActive(true);
    }
}


