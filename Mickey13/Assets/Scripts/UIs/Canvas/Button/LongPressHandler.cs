using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LongPressHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Tooltip("롱클릭 판정 시간")]
    [SerializeField] private float holdTime = 0.3f;
    [Tooltip("롱클릭 유지시 코루틴 반복 간격 증가")]
    [SerializeField] private float repeatDelay = 0.1f;

    public UnityEvent onClick;
    public UnityEvent onLongPressStart; // 롱클릭 시작
    public UnityEvent onLongPressSustain; // 롱클릭 유지

    private bool isPressed = false; // 버튼을 누르고 있는지 여부
    private bool isLongPressTrigger = false;

    private Coroutine longPressCoroutine;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        isLongPressTrigger = false ;

        if(longPressCoroutine != null)
        {
            StopCoroutine(longPressCoroutine);
        }
        longPressCoroutine = StartCoroutine(LongPressCheck());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Stop();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(!isLongPressTrigger)
        {
            onClick.Invoke();
        }

        Stop();
    }

    private void Stop()
    {
        isPressed = false;
        if (longPressCoroutine != null)
        {
            StopCoroutine(longPressCoroutine);
            longPressCoroutine = null;
        }
    }

    private IEnumerator LongPressCheck()
    {
        yield return new WaitForSeconds(holdTime);

        if(isPressed)
        {
            isLongPressTrigger = true;
            Debug.Log("롱클릭 실행");
            onLongPressStart?.Invoke();

            float currentDelay = repeatDelay; // 현재 반복 간격
            float minDelay = 0.01f; // 최소 반복 간격

            while (isPressed)
            {
                onLongPressSustain?.Invoke();
                yield return new WaitForSeconds(repeatDelay);

                if (currentDelay > minDelay)
                {
                    currentDelay = Mathf.Max(minDelay, currentDelay * 0.9f);
                }
            }
        }
    }

    private void OnDisable()
    {
        Stop();
    }
}
