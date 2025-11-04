using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : Singleton<BuffManager>
{
    private Dictionary<EntityData, List<BuffData>> activeBuffs = new();

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        foreach (var buff in activeBuffs)
        {
            var entity = buff.Key;
            var buffs = buff.Value;

            for (int i = buffs.Count - 1; i >= 0; i--)
            {
                buffs[i].ElapsedTime += deltaTime;
                if (buffs[i].ElapsedTime >= buffs[i].Duration)
                {
                    RemoveBuff(entity, buffs[i]);
                    buffs.RemoveAt(i);
                }
            }
        }
    }

    public void ApplyBuff(EntityData target, BuffTypeEnum type, float value, float duration)
    {
        if (target == null) return;

        if(!activeBuffs.ContainsKey(target))
            activeBuffs[target] = new List<BuffData>();

        var buffs = activeBuffs[target];
        var existing = buffs.Find(b => b.Type == type);

        if(existing != null)
        {
            RemoveBuff(target, existing);
            existing.Value = value;
            existing.Duration = duration;
            existing.ElapsedTime = 0f;
            ApplyToStatModel(target, existing);
            Debug.Log($"{target.Name}에게 버프적용!");
        }
        else
        {
            var buff = new BuffData(type, value, duration);
            buffs.Add(buff);
            ApplyToStatModel(target, buff);
            Debug.Log($"{target.Name}에게 버프적용!");
        }
    }

    private void ApplyToStatModel(EntityData entity, BuffData buff)
    {
        var stat = entity.Buffs;

        switch (buff.Type)
        {
            case BuffTypeEnum.AtkUp:
                stat.Atk.value += buff.Value;
                break;

            case BuffTypeEnum.AtkDown:
                stat.Atk.value -= buff.Value;
                break;

            case BuffTypeEnum.AtkSpeedUp: //공속은 수치가 낮을수록 빠름
                stat.AtkCooldown -= buff.Value;
                break;

            case BuffTypeEnum.AtkSpeedDown:
                stat.AtkCooldown += buff.Value;
                break;

            case BuffTypeEnum.CritChanceUp:
                stat.CritChance += buff.Value;
                break;

            case BuffTypeEnum.CritChanceDown:
                stat.CritChance -= buff.Value;
                break;

            case BuffTypeEnum.CritDmgUp:
                stat.CritMult += buff.Value;
                break;

            case BuffTypeEnum.CritDmgDown:
                stat.CritMult -= buff.Value;
                break;

            case BuffTypeEnum.SkillCoolDown:
                stat.SkillCooldown += buff.Value;
                break;

            case BuffTypeEnum.DamageDealMultUp:
                stat.DamageDealMult += buff.Value;
                break;

            case BuffTypeEnum.DamageDealMultDown:
                stat.DamageDealMult -= buff.Value;
                break;

            case BuffTypeEnum.DamageTakenMultUp:
                stat.DamageTakenMult += buff.Value;
                break;

            case BuffTypeEnum.DamageTakenMultDown:
                stat.DamageTakenMult -= buff.Value;
                break;
        }
        
        entity.SetStatDirty();
    }

    private void RemoveBuff(EntityData entity, BuffData buff)
    {
        var stat = entity.Buffs;

        switch (buff.Type)
        {
            case BuffTypeEnum.AtkUp:
                stat.Atk.value -= buff.Value;
                break;

            case BuffTypeEnum.AtkDown:
                stat.Atk.value += buff.Value;
                break;

            case BuffTypeEnum.AtkSpeedUp:
                stat.AtkCooldown += buff.Value;
                break;

            case BuffTypeEnum.AtkSpeedDown:
                stat.AtkCooldown -= buff.Value;
                break;

            case BuffTypeEnum.CritChanceUp:
                stat.CritChance -= buff.Value;
                break;

            case BuffTypeEnum.CritChanceDown:
                stat.CritChance += buff.Value;
                break;

            case BuffTypeEnum.CritDmgUp:
                stat.CritMult -= buff.Value;
                break;

            case BuffTypeEnum.CritDmgDown:
                stat.CritMult += buff.Value;
                break;

            case BuffTypeEnum.SkillCoolDown:
                stat.SkillCooldown -= buff.Value;
                break;

            case BuffTypeEnum.DamageDealMultUp:
                stat.DamageDealMult -= buff.Value;
                break;

            case BuffTypeEnum.DamageDealMultDown:
                stat.DamageDealMult += buff.Value;
                break;

            case BuffTypeEnum.DamageTakenMultUp:
                stat.DamageTakenMult -= buff.Value;
                break;

            case BuffTypeEnum.DamageTakenMultDown:
                stat.DamageTakenMult += buff.Value;
                break;
        }
        entity.SetStatDirty();
    }
}
