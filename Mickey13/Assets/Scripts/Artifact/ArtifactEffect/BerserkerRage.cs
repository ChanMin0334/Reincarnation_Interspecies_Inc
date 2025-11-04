using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerRage : ArtifactEffect //체력이 낮을수록 공격력이 증가 value는 최대로 올라갈수 있는 공격력
{
    private float baseAtkBonus = 0f;
    public override void RegisterEvent()
    {
        base.RegisterEvent();

        EventManager.Instance.StartListening(EventType.Tick, OnTick);
    }

    public override void UnregisterEvent()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.StopListening(EventType.Tick, OnTick);
    }

    private void OnTick(object obj)
    {
        if (owner == null || owner.Equals(null)) return;

        if (owner.Data == null) return;

        BigNumeric curHP = owner.Data.curHP.value;
        BigNumeric maxHP = owner.Data.MaxHP.value;

        if (maxHP <= 0f) return;

        float ratio = curHP.Ratio(maxHP);
        ratio = Mathf.Clamp01(ratio);

        float bonus = Mathf.Lerp(0f, so.Value, 1f - ratio);

        var s = owner.Data.Artifacts;

        s.Atk.value -= baseAtkBonus;
        s.Atk.value += bonus;

        baseAtkBonus = bonus;
        Debug.Log($"BerserkerRage : HP {ratio:P0} → Atk 보너스 {bonus:F1}%");
    }
}
