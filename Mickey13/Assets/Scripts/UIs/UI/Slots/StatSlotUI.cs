using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatSlotUI : SlotBase<StatDefinition>
{
    [SerializeField] TextMeshProUGUI statName; // 스탯 이름
    [SerializeField] TextMeshProUGUI statValue; // 스탯 수치
    [SerializeField] Image statIcon; // 스탯 아이콘


    public void Setup(StatDefinition definition, float value)
    {
        base.Setup(definition);
        statName.text = definition.statName;
        statValue.text = ShowStatValue(definition, value);
        statIcon.sprite = definition.statIcon;
        statIcon.gameObject.SetActive(definition.statIcon != null);
    }

    public void Setup(StatDefinition definition, BigNumericWrapper value)
    {
        base.Setup(definition);
        statName.text = definition.statName;
        statValue.text = ShowStatValue(definition, value);
        statIcon.sprite = definition.statIcon;
        statIcon.gameObject.SetActive(definition.statIcon != null);
    }

    private string ShowStatValue(StatDefinition definition, float value)
    {
        string formattedValue;
        switch (definition.type)
        {
            case StatType.Atk_Area:
            case StatType.Atk_Range:
                formattedValue = value.ToString("N0"); break; // N

            case StatType.Atk_Cooldown:
            case StatType.Skill_Cooldown:
                formattedValue = value.ToString("F1"); break;

            case StatType.Crit_Chance:
            case StatType.Crit_Mult:
                formattedValue = value.ToString("0.0") + "%"; break; // N %
            default:
                formattedValue = value.ToString();
                break;
        }
        return formattedValue;
    }

    private string ShowStatValue(StatDefinition definition, BigNumericWrapper value)
    {
        return value.ToString();
    }

    protected override void UpdateUI()
    {
        //statName = _data.FinalStat.
    }
}
