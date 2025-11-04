using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[Serializable]
public class SkillInstance
{
    private SkillSO data;
    private Entity owner;
    private float lastUseTime = 0f;

    //추가
    private bool lastHitKilled = false; //이스킬로 적을 마무리했는가?

    public SkillSO Data => data;

    public event Action<float> OnSkillUse;

    public SkillInstance(SkillSO so, Entity owner)
    {
        data = so;
        this.owner = owner;

        if (owner == null || owner.Data == null)
        {
            return;
        }

        if (data.Type == SkillTypeEnum.Passive)
        {
            //InitPassive();
        }
    }

    public float GetCoolDown()
    {
        if (data == null || owner == null || owner.Data == null || owner.Data.FinalStat == null)
        {
            return 0;
        }

        float coolDownStatMult = 1f + (owner.Data.FinalStat.SkillCooldown / 100f);
        float effectiveCoolDown = data.CoolDown / coolDownStatMult;
        
        return Mathf.Max(0.1f , effectiveCoolDown);
    }
    
    public void ResetCooldown()
    {
        lastUseTime = Time.time - data.CoolDown;
    }

    public bool CanUse()
    {
        if (data == null || owner == null || owner.Data == null || owner.Data.FinalStat == null)
        {
            return false;
        }

        if (data.Type == SkillTypeEnum.Passive)
            return false;

        float coolDownStatMult = 1f + (owner.Data.FinalStat.SkillCooldown / 100f);

        float effectiveCoolDown = data.CoolDown / coolDownStatMult;

        return Time.time - lastUseTime >= effectiveCoolDown;
    }

    public void UseSkill(Entity target)
    {
        if (!CanUse()) return;

        lastUseTime = Time.time;

        //추가
        lastHitKilled = false;
        
        OnSkillUse?.Invoke(GetCoolDown()); // 쿨타임 감소 이벤트 발행

        foreach (var effect in data.Effects)
        {
            if (effect.condition == SkillConditionTypeEnum.None)
            {
                ApplyEffect(effect, target);
            }
        }

        foreach (var effect in data.Effects)
        {
            if (effect.condition != SkillConditionTypeEnum.None && CheckCondition(effect, target))
            {
                ApplyEffect(effect, target);
            }
        }
    }



    private void ApplyEffect(SkillEffect effect, Entity target)
    {
        if (effect == null) return;

        switch (effect.effectType)
        {
            case SkillEffectTypeEnum.Damage:
                ApplyDamageEffect(effect, target);
                break;

            case SkillEffectTypeEnum.Heal:
                ApplyHealEffect(effect, target);
                break;

            case SkillEffectTypeEnum.Buff:
            case SkillEffectTypeEnum.Debuff:
                ApplyBuffEffect(effect, target);
                break;

            case SkillEffectTypeEnum.Special:
                ApplySpecialEffect(effect, target);
                break;

            case SkillEffectTypeEnum.TickDamage:
                ApplyDotEffect(effect, target);
                break;
        }
    }

    /// <summary>
    /// 조건부 효과 체크용
    /// </summary>
    private bool CheckCondition(SkillEffect effect, Entity target)
    {
        switch (effect.condition)
        {
            case SkillConditionTypeEnum.None:
                return true;

            case SkillConditionTypeEnum.TargetHPBelow:
                {
                    if (target == null) return false;
                    float hpPercent = (target.Data.curHP.ToFloat() / target.Data.FinalStat.HP.ToFloat()) * 100f;
                    return hpPercent <= effect.conditionValue;
                }

            case SkillConditionTypeEnum.SelfHPBelow:
                {
                    if (owner == null) return false;
                    float hpPercent = (owner.Data.curHP.ToFloat() / owner.Data.FinalStat.HP.ToFloat()) * 100f;
                    return hpPercent <= effect.conditionValue;
                }
            case SkillConditionTypeEnum.OnKill:
                {
                    return lastHitKilled;
                }


            case SkillConditionTypeEnum.RandomChance:
                {
                    return UnityEngine.Random.value < effect.conditionValue / 100f;
                }

            default:
                {
                    return false;
                }
        }
    }

    private void ApplyDamageEffect(SkillEffect effect, Entity target)
    {
        var targets = GetTargets(effect.targetType, target);
        if (targets == null) return;

        foreach (var t in targets)
        {
            if (t == null || t.IsDead) continue;

            BigNumeric dmgValue = owner.Data.FinalStat.Atk.value * (effect.power / 100f);
            owner.DealDamage(t, dmgValue, isSkill: true);

            if (t.IsDead)
                lastHitKilled = true;

            EffectUtiity.PlayEffect(effect.effectData, owner.transform, t.transform);
        }
    }


    private void ApplyHealEffect(SkillEffect effect, Entity target)
    {
        var targets = GetTargets(effect.targetType, target);
        if (targets == null) return;

        foreach (var t in targets)
        {
            if (t == null || t.IsDead) continue;

            var allies = CharacterManager.Instance.BattleCharacterDict.Values;

            Entity lowestHP = null;
            float lowestRatio = 1f;

            foreach (var ally in allies)
            {
                if (ally.IsDead) continue;

                float ratio = ally.Data.curHP.value.Ratio(ally.Data.MaxHP);
                if (ratio < lowestRatio)
                {
                    lowestRatio = ratio;
                    lowestHP = ally;
                }
            }

            if (lowestHP == null)
                lowestHP = owner;

            BigNumeric heal = owner.Data.FinalStat.Atk.value * (effect.power / 100f);
            lowestHP.Heal(heal);

            EffectUtiity.PlayEffect(effect.effectData, owner.transform, t.transform);
        }
    }

    private void ApplyBuffEffect(SkillEffect effect, Entity target)
    {
        var targets = GetTargets(effect.targetType, target);
        foreach (var t in targets)
        {
            if (t == null || t.IsDead) continue;

            BuffManager.Instance.ApplyBuff(t.Data, effect.buffType, effect.power, effect.duration);

            EffectUtiity.PlayEffect(effect.effectData, owner.transform, t.transform);
        }
    }

    private void ApplySpecialEffect(SkillEffect effect, Entity target)
    {

    }

    private void ApplyDotEffect(SkillEffect effect, Entity target)
    {
        foreach (var t in GetTargets(effect.targetType, target))
        {
            if (t == null || t.IsDead) continue;
            owner.StartCoroutine(ApplyTickDamage(effect, t));
            EffectUtiity.PlayEffect(effect.effectData, owner.transform, t.transform);
        }
    }

    private IEnumerable<Entity> GetTargets(SkillTargetTypeEnum targetType, Entity target)
    {
        switch (targetType)
        {
            case SkillTargetTypeEnum.Self:
                return new[] { owner };

            case SkillTargetTypeEnum.Ally: //추가
                {
                    var allies = new List<Entity>(CharacterManager.Instance.BattleCharacterDict.Values);
                    allies.RemoveAll(a => a == null || a.IsDead);

                    if (allies.Count == 0)
                        return Array.Empty<Entity>();

                    var randomTarget = allies[UnityEngine.Random.Range(0, allies.Count)];
                    return new[] { randomTarget };
                }

            case SkillTargetTypeEnum.AllAllies:
                return CharacterManager.Instance.BattleCharacterDict.Values;

            case SkillTargetTypeEnum.Enemy:
                return new[] { target };

            case SkillTargetTypeEnum.AllEnemies:
                {
                    float range = owner.Data.FinalStat.AtkRange;
                    var enemies = CharacterTeam.ReturnDetectedEnemies(range);

                    if (enemies == null || enemies.Count == 0)
                        return System.Array.Empty<Entity>(); // null일 경우 비어있는 컬렉션 반환
                        // return null;

                    return enemies;
                }

            default:
                Debug.LogError("GetTargets 실패");
                return System.Array.Empty<Entity>();
        }
    }
    
    //추가
    private IEnumerator ApplyTickDamage(SkillEffect effect , Entity target)
    {
        float elapsed = 0f;

        while(elapsed < effect.duration)
        {
            if (target == null || target.IsDead)
                yield break;

            BigNumeric tickDamage = owner.Data.FinalStat.Atk.value * (effect.power / 100f);

            owner.DealDamage(target, tickDamage, isSkill: true);

            if (target.IsDead)
                lastHitKilled = true;

            yield return new WaitForSeconds(effect.tickInterval);
            elapsed += effect.tickInterval;
        }
    }
}
