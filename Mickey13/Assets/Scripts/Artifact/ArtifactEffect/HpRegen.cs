using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpRegen : ArtifactEffect //체력 리젠 value = 소숫점으로
{
    private float tickTimer = 0f;

    [Header("Regen CoolDown")]
    private float regenCoolDown = 5f;

    public override void RegisterEvent()
    {
        base.RegisterEvent();
        EventManager.Instance.StartListening(EventType.Tick, OnTick);
    }

    public override void UnregisterEvent()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.StopListening(EventType.Tick, OnTick);
        }
    }
    
    private void OnTick(object obj)
    {
        if (owner == null) return;

        tickTimer += 0.5f; //Event의 Tick이 0.5초마다 한번 호출되기 때문에

        if(tickTimer >= regenCoolDown)
        {
            tickTimer = 0f;

            float healRatio = so.Value / 100f;
            BigNumeric healAmount = owner.Data.MaxHP.value * healRatio;
            owner.Heal(healAmount);

            Debug.Log($"[HpRegen] {owner.name}이 {healAmount:F1} 만큼 체력을 회복했습니다!");
        }
    }
}
