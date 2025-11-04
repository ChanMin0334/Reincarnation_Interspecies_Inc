// UIToWorldAnchor.cs
using UnityEngine;

public class UIToWorldAnchor : MonoBehaviour
{
    public RectTransform uiTarget; // HUD의 GoldIcon
    public Camera uiCamera;        // Overlay면 null
    public Camera mainCamera;      // 보통 Camera.main
    public float worldZ = 0f;      // 코인이 있는 Z 값(보통 0)


    private void Start()
    {
        uiTarget = GameObject.Find("CurGold").GetComponent<RectTransform>(); ;
    }

    void LateUpdate()
    {
        if (!uiTarget || !mainCamera) return;

        // UI를 스크린 좌표로
        Vector2 scr = RectTransformUtility.WorldToScreenPoint(uiCamera, uiTarget.position);

        // 스크린 → 월드 (코인 Z 평면까지 투영)
        float zDepth = Mathf.Abs(worldZ - mainCamera.transform.position.z);
        if (zDepth < 0.001f) zDepth = 1f;

        Vector3 wp = mainCamera.ScreenToWorldPoint(new Vector3(scr.x, scr.y, zDepth));
        wp.z = worldZ;
        transform.position = wp;
    }
}