using System;
using UnityEngine;
using UnityEngine.U2D;

[Serializable]
public class RuneData : ISlotUIData, IInventoryData
{
    public string id;
    public int count;

    public RuneSO Definition => DataManager.Instance.GetData<RuneSO>(id);

    public string ID => id;

    public string Name => Definition.Name;

    public RarityEnum Rarity => Definition.Rarity;

    public bool IsNew { get; set; }
    
    public float EffectValue  => Definition.EffectValue * count;
    
    public RuneData(string id, int count = 1)
    {
        this.id = id;
        var maxCount = Definition.MaxStack;
        this.count = Mathf.Min(count, maxCount);
    }

    public StatModel GetFinalStat()
    {
        StatModel stat = StatModel.Zero();

        float value = this.EffectValue;

        switch(Definition.EffectType)
        {
            case StatType.HP:
                stat.HP = value;
                break;
            case StatType.Atk:
                stat.Atk = value;
                break;
            case StatType.Atk_Cooldown:
                stat.AtkCooldown = value;
                break;
            case StatType.Skill_Cooldown:
                stat.SkillCooldown = value;
                break;
            case StatType.Atk_Range:
                stat.AtkRange = value;
                break;
            case StatType.Crit_Chance:
                stat.CritChance = value;
                break;
            case StatType.Crit_Mult:
                stat.CritMult = value;
                break;
            case StatType.Atk_Area:
                stat.AtkArea = value;
                break;
            case StatType.MoveSpeed:
                stat.MoveSpeed = value;
                break;
        }        
        return stat;
    }

    public Sprite GetSprite(SlotImageType imageType)
    {
        return Definition.Sprite;
    }
}
