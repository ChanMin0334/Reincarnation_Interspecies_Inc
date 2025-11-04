using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class IconSizer : MonoBehaviour
{
    [Range(0.01f, 1.0f)]
    public float widthPercentage = 0.15f; // 화면 너비의 15%를 차지하도록 설정

    private RectTransform rectTransform;
    private RectTransform parentTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if(this.transform.parent != null)
        {
            parentTransform = this.transform.parent.GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        if (parentTransform == null) return;

        // 부모의 현재 너비를 기준으로 아이콘의 목표 너비를 계산
        float partneWidth = parentTransform.GetComponent<RectTransform>().rect.width;
        float targetWidth = partneWidth * widthPercentage;

        // RectTransform의 크기를 설정합니다.
        // AspectRatioFitter가 Height를 자동으로 조절해 줄 것이므로 Width만 바꿔줍니다.
        rectTransform.sizeDelta = new Vector2(targetWidth, rectTransform.sizeDelta.y);
    }
}
