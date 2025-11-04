using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class UICanvasResolution : MonoBehaviour
{
    private CanvasScaler canvasScaler;

    // 목표 비율 (9:16)
    private const float targetAspect = 9.0f / 16.0f;

    int lastWidth = 0;
    int lastHeight = 0;

    private void Awake()
    {
        canvasScaler = GetComponent<CanvasScaler>();
        SetMatchMode();
    }

    private void Update()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            SetMatchMode();
        }
    }

    void SetMatchMode()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;

        float screenAspect = (float)Screen.width / Screen.height;

        canvasScaler.matchWidthOrHeight = (screenAspect > targetAspect) ? 1.0f : 0.0f;
    }
}