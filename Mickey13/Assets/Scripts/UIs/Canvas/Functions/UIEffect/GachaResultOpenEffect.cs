using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GachaResultOpenEffect : MonoBehaviour
{
    [SerializeField] CanvasGroup coverGroup; // 뒷면
    [SerializeField] CanvasGroup resultGroup; // 앞면
    [SerializeField] Image stampImage;

    [Header("Animation Settings")]
    [SerializeField] private float stampFadeInDuration = 0.2f;
    [SerializeField] private float stampScaleDuration = 0.1f;
    [SerializeField] private float revealDuration = 0.5f;
    [SerializeField] private Ease stampEase = Ease.OutQuad;
    [SerializeField] private Ease revealEase = Ease.InOutSine;

    [SerializeField] bool isRevealed = false; // 디버깅용 직렬화
    public bool IsRevealed => isRevealed;

    private Vector3 initialStampScale;

    private void Awake()
    {
        // 초기 크기 저장(스케일 초기화)
        initialStampScale = stampImage.transform.localScale;
    }

    // 연출 재생
    public void PlayEffect(Action onCompleteCallback = null, bool playSound = true )
    {
        if (isRevealed) return;
        isRevealed = true;
        coverGroup.interactable = false;
        Sequence revealSequence = DOTween.Sequence();
        revealSequence.SetId(this);

        // 도장 연출
        revealSequence.Append(stampImage.DOFade(1f, stampFadeInDuration).SetId(this));
        revealSequence.Append(stampImage.transform.DOScale(initialStampScale, stampScaleDuration).SetEase(stampEase).SetId(this));
        if (playSound)
        {
            revealSequence.AppendCallback(() => AudioManager.Instance.PlaySFX(SfxType.Stamp));
        }
        revealSequence.Append(stampImage.transform.DOScale(initialStampScale * 0.8f, stampScaleDuration).SetId(this));

        // 커버 페이드 아웃, 결과 페이드 인
        revealSequence.AppendInterval(0.1f); // 도장이 찍히고 잠시 멈춤
        revealSequence.Append(coverGroup.DOFade(0f, revealDuration).SetEase(revealEase).SetId(this));
        revealSequence.Join(resultGroup.DOFade(1f, revealDuration).SetEase(revealEase).SetId(this));

        // 애니메이션 종료 신호
        revealSequence.OnComplete(() => {onCompleteCallback?.Invoke(); });
    }

    // 연출 초기화(오브젝트 풀링)
    public void ResetEffect()
    {
        // 연출 중지
        DOTween.Kill(this);

        isRevealed = false;

        // 초기화
        coverGroup.interactable = true;
        coverGroup.alpha = 1f;
        resultGroup.alpha = 0f;
        stampImage.color = new Color(stampImage.color.r, stampImage.color.g, stampImage.color.b, 0); // 투명하게
        stampImage.transform.localScale = initialStampScale;
    }
}
