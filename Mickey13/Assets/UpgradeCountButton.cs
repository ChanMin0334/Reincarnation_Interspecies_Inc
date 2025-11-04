using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpgradeCountButton : MonoBehaviour
{
    [SerializeField] private string[] countTexts = { "x1", "x10", "x100", "Max" };
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI Count_text;

    private int countTextsIndex = 0;
    private void Start()
    {
        if (UpgradeManager.Instance != null)
        {
            string initialCount = UpgradeManager.Instance.CurrentUpgradeCount;
            for (int i = 0; i < countTexts.Length; i++)
            {
                if (countTexts[i] == initialCount)
                {
                    countTextsIndex = i;
                    break;
                }
            }
        }
        button.onClick.AddListener(ChangeCount);
        UpdateText(false);
    }

    public void ChangeCount()
    {
        countTextsIndex++;
        countTextsIndex %= countTexts.Length;

        UpdateText(true);
    }

    private void UpdateText(bool isUpgraded)
    {
        var currentCount = countTexts[countTextsIndex];
        if (Count_text != null && countTexts.Length > 0)
        {
            Count_text.text = currentCount;
        }
        if (isUpgraded)
        {
            UpgradeManager.Instance.SetUpgradeCount(currentCount);
        }
    }
}
