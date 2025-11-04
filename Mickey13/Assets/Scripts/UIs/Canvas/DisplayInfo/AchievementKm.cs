using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AchievementKm : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI CurAchievementKm;
    [SerializeField] private TextMeshProUGUI maxAchievementKm;

    private void OnEnable()
    {
        User.Instance.OnAchievementKmChanged += UpdateUI;
        UpdateUI();
    }

    private void OnDisable()
    {
        User.Instance.OnAchievementKmChanged -= UpdateUI;
    }

    private void UpdateUI( )
    {
        int curDistance = User.Instance.CurAchievementKm;
        int maxDistance = User.Instance.ReincarnateData.MaxDistance;
        
        CurAchievementKm.text = $"{curDistance} KM";
        maxAchievementKm.text = $"{maxDistance} KM";
    }
}
