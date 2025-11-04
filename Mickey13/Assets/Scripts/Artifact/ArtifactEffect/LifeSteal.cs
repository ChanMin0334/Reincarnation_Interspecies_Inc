using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeSteal : ArtifactEffect
{
    public override void RegisterEvent()
    {
        base.RegisterEvent();
        EventManager.Instance.StartListening(EventType.OnDamaged, OnDamageDealt);
    }

    public override void UnregisterEvent()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.StopListening(EventType.OnDamaged, OnDamageDealt);
        }
    }

    private void OnDamageDealt(object param)
    {
        if(param is DamageData dmg && dmg.Attacker == owner)
        {
            BigNumeric healAmount = dmg.Value * (so.Value / 100f);
            owner.Heal(healAmount);
            Debug.Log($"{owner.Name}이 {healAmount:F1}만큼 흡혈했습니다.");
        }
    }
}
