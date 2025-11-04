using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    private Camera _camera;
    int lastWidth = 0;
    int lastHeight = 0;
    private void Awake()
    {
        _camera = GetComponent<Camera>();
        SetLetterBox();
    }

    private void Update()
    {
        if(Screen.width != lastWidth ||  Screen.height != lastHeight)
        {
            SetLetterBox();
        }
    }

    void SetLetterBox()
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float targetAspect = 9.0f / 16.0f;

        float scaleHeight = screenAspect / targetAspect;

        Rect rect = new Rect(0, 0, 1, 1);

        if (scaleHeight < 1.0f) // 화면이 목표보다 세로로 길쭉할 때 (상하 레터박스 필요)
        {
            rect.height = scaleHeight;
            rect.y = (1.0f - scaleHeight) / 2.0f;
        }
        else // 화면이 목표보다 가로로 넓을 때 (좌우 레터박스 필요)
        {
            float scaleWidth = 1.0f / scaleHeight;
            rect.width = scaleWidth;
            rect.x = (1.0f - scaleWidth) / 2.0f;
        }

        _camera.rect = rect;
        lastHeight = Screen.height;
        lastWidth = Screen.width;
    }

    void OnPreCull()
    {
        if (_camera.rect.width < 1.0f || _camera.rect.height < 1.0f)
        {
            GL.Clear(true, true, Color.black);
        }
    }
}