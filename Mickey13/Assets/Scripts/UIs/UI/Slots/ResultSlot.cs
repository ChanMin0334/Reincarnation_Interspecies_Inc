using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ResultSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultTitle;
    [SerializeField] TextMeshProUGUI resultValue;

    public void Setup(string title)
    {
        resultTitle.text = title;
        resultValue.text = string.Empty;
    }

    public IEnumerator AnimateValueText(string textToAnimate, float charAnimationDuration)
    {
        StringBuilder stringBuilder = new StringBuilder();
        resultValue.text = ""; // 텍스트 초기화
        for (int i = 0; i < textToAnimate.Length; i++)
        {
            stringBuilder.Append(textToAnimate[i]);
            resultValue.text = stringBuilder.ToString();
            yield return new WaitForSeconds(charAnimationDuration);
        }
    }

    public void Clear()
    {
        resultTitle.text = string.Empty;
        resultValue.text = string.Empty;
    }
}
