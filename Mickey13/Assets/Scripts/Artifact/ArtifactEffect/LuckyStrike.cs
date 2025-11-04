using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckyStrike : ArtifactEffect
{
    public override void RegisterEvent()
    {
        base.RegisterEvent();
        EventManager.Instance.StartListening(EventType.CalculateDamage, OnCalculateDamage);
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

        float chance = so.Value / 100f;
        if(Random.value <= chance)
        {
            dmg.Value *= 10f; //데미지 10배
            Debug.Log($"[LuckyStrike] {owner.name}의 공격이 LuckyStrike로 10배 피해 가한 피해량 {dmg.Value}");
        }
    }
}
