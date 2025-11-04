using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButtonScale : CustomButtonBase
{
    private const float OriginalScale = 1.0f;
    [SerializeField] private float toScale = 0.9f;
    [SerializeField] private float duration = 0.1f;
    
    private Sequence punchSequence;
    
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        // 실행중인 시퀀스 중지
        punchSequence?.Kill();
        // 시퀀스 생성
        punchSequence = DOTween.Sequence();

        punchSequence.Append(transform.DOScale(toScale, duration / 2)); 
        punchSequence.Append(transform.DOScale(OriginalScale, duration / 2)); 
    }
}
