using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishDamage : ArtifactEffect //상대체력이 일정 이하일때 추가피해
{
    private float hpPercent = 0.20f;
    private float bonusPercent;

    public override void RegisterEvent()
    {
        base.RegisterEvent();
        EventManager.Instance.StartListening(EventType.CalculateDamage, OnCalculateDamage);

        bonusPercent = data.count * so.Value;
    }

    public override void UnregisterEvent()
    {
        if(EventManager.Instance != null)
            EventManager.Instance.StopListening(EventType.CalculateDamage, OnCalculateDamage);
    }

    private void OnCalculateDamage(object param)
    {
        if (param is not DamageData dmg) return;
        if (dmg.Attacker != owner) return;

        var target = dmg.Target;
        if (target == null || target.Data == null) return;

        float curHP = target.Data.curHP.ToFloat();
        float maxHP = target.Data.FinalStat.HP.ToFloat();
        if (maxHP <= 0f) return;

        float hpRatio = curHP / maxHP;

        if (hpRatio >= hpPercent) return;

        float multiplier = 1f + (bonusPercent / 100f);
        dmg.Value *= multiplier;

        Debug.Log($"[MercyFinish] {target.Name} HP {hpRatio * 100f:F1}% → 피해량 {bonusPercent}% 증가 (최종 {dmg.Value:F1})");
    }
}
