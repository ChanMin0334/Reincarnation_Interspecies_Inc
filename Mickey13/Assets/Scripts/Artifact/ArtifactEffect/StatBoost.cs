using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBoost : ArtifactEffect
{
    private enum BoostTypeEnum
    {
        Single,
        AllStat
    }

    private BoostTypeEnum type;
    private StatType statType;

    public override void RegisterEvent()
    {
        base.RegisterEvent();
        Debug.Log($"[Register] {owner?.Name}에게 {So.ID} 적용 시작 (count={data.count}, value={so.Value}, total={so.Value * data.count})");


        statType = so.StatType;
        float amount = so.Value * data.count;

        if (type == BoostTypeEnum.AllStat) ApplyAllStat(amount);
        else ApplySingleStat(amount);
    }

    public override void UnregisterEvent()
    {
        Debug.Log($"[Unregister] {owner?.Name}의 {So.ID} 효과 제거 (count={data.count})");

        if (owner == null) return;
        float amount = so.Value * data.count;

        if (type == BoostTypeEnum.AllStat)
            RemoveAllStat(amount);

        else 
            RemoveSingleStat(amount);
    }

    private void ApplySingleStat(float ratio)
    {
        Debug.Log($"[ApplySingleStat] {owner?.Name} {statType} += {ratio}");

        var artifacts = owner.Data.Artifacts;

        if (statType == StatType.None)
        {
            return;
        }

        switch (statType)
        {
            case StatType.HP: artifacts.HP.value += ratio; break;
            case StatType.Atk: artifacts.Atk.value += ratio; break;
            case StatType.Atk_Cooldown: artifacts.AtkCooldown += ratio; break;
            case StatType.Skill_Cooldown: artifacts.SkillCooldown += ratio; break;
            case StatType.Atk_Range: artifacts.AtkRange += ratio; break;
            case StatType.Crit_Chance: artifacts.CritChance += ratio; break;
            case StatType.Crit_Mult: artifacts.CritMult += ratio; break;
            case StatType.MoveSpeed: artifacts.MoveSpeed += ratio; break;
            case StatType.Atk_Area: artifacts.AtkArea += ratio; break;
            case StatType.Dmg_Mult: artifacts.DamageDealMult += ratio; break;
            case StatType.Taken_Mult: artifacts.DamageTakenMult += ratio; break;
            default:
                Debug.LogWarning($"{statType} 타입은 존재하지 않습니다.");
                break;
        }
    }

    private void RemoveSingleStat(float ratio)
    {
        Debug.Log($"[RemoveSingleStat] {owner?.Name} {statType} -= {ratio}");

        var artifacts = owner.Data.Artifacts;

        if (statType == StatType.None) return;

        switch (statType)
        {
            case StatType.HP: artifacts.HP.value -= ratio; break;
            case StatType.Atk: artifacts.Atk.value -= ratio; break;
            case StatType.Atk_Cooldown: artifacts.AtkCooldown -= ratio; break;
            case StatType.Skill_Cooldown: artifacts.SkillCooldown -= ratio; break;
            case StatType.Atk_Range: artifacts.AtkRange -= ratio; break;
            case StatType.Crit_Chance: artifacts.CritChance -= ratio; break;
            case StatType.Crit_Mult: artifacts.CritMult -= ratio; break;
            case StatType.MoveSpeed: artifacts.MoveSpeed -= ratio; break;
            case StatType.Atk_Area: artifacts.AtkArea -= ratio; break;
            case StatType.Dmg_Mult: artifacts.DamageDealMult -= ratio; break;
            case StatType.Taken_Mult: artifacts.DamageTakenMult -= ratio; break;
            default:
                break;
        }
    }

    private void ApplyAllStat(float ratio)
    {
        var artifacts = owner.Data.Artifacts;

        artifacts.HP.value += ratio;
        artifacts.Atk.value += ratio;
        artifacts.AtkCooldown += ratio;
        artifacts.SkillCooldown += ratio;
        artifacts.AtkRange += ratio;
        artifacts.CritChance += ratio;
        artifacts.CritMult += ratio;
        artifacts.AtkArea += ratio;
        artifacts.MoveSpeed += ratio;
        artifacts.DamageDealMult += ratio;
        artifacts.DamageTakenMult += ratio;

    }

    private void RemoveAllStat(float ratio)
    {
        var artifacts = owner.Data.Artifacts;

        artifacts.HP.value -= ratio;
        artifacts.Atk.value -= ratio;
        artifacts.AtkCooldown -= ratio;
        artifacts.SkillCooldown -= ratio;
        artifacts.AtkRange -= ratio;
        artifacts.CritChance -= ratio;
        artifacts.CritMult -= ratio;
        artifacts.AtkArea -= ratio;
        artifacts.MoveSpeed -= ratio;
        artifacts.DamageDealMult -= ratio;
        artifacts.DamageTakenMult -= ratio;
    }

    protected override void OnStackDelta(int delta)
    {
        if (owner == null || delta == 0) return;

        float amount = so.Value * delta;
        if(delta > 0)
        {
            Debug.Log($"[Delta+] {owner.Name}:{So.ID} +{amount} (cnt {data.count})");
            if (type == BoostTypeEnum.AllStat) ApplyAllStat(amount);

            else ApplySingleStat(amount);
        }

        else
        {
            Debug.Log($"[Delta-] {owner.Name}:{So.ID} -{-amount} (cnt {data.count})");
            if (type == BoostTypeEnum.AllStat) RemoveAllStat(-amount);

            else RemoveSingleStat(-amount);
        }
    }
}

