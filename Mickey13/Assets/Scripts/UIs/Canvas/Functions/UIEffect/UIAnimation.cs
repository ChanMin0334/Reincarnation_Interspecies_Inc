using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    [SerializeField] private Ease easeType = Ease.OutFlash;
    [SerializeField] private float openDuration = 2f;
    [SerializeField] private float closeDuration = 1f;
    [SerializeField] private Image fadeImage;
    
    // private CanvasGroup canvasGroup;
    // private void Awake()
    // {
    //     // canvasGroup = GetComponentInChildren<CanvasGroup>();
    // }
    
    private void OnEnable()
    {
        PlayOpenAnimation();
    }
    
    private void PlayOpenAnimation()
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(0,0,0,1);
        fadeImage.DOFade(0f, openDuration)
            .SetEase(easeType)
            .SetUpdate(true)
            .OnComplete(() =>fadeImage.gameObject.SetActive(false));
    }

    public void PlayCloseAnimation(Action onCompleteAction)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.DOFade(1f, closeDuration)
            .SetEase(easeType)
            .SetUpdate(true)
            .OnComplete(() => onCompleteAction?.Invoke()); // 애니매이션이 끝난 뒤 다음 UI 활성화
    }
}
