using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenShield : ArtifactEffect //피해량 막아주는 쉴드 유물
{
    private bool shieldActive = false;
    private float timer = 0f;

    public override void RegisterEvent()
    {
        base.RegisterEvent();
        EventManager.Instance.StartListening(EventType.Tick, OnTick);
        EventManager.Instance.StartListening(EventType.CalculateDamage, OnActiveShield);
    }

    public override void UnregisterEvent()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.StopListening(EventType.Tick, OnTick);
            EventManager.Instance.StopListening(EventType.CalculateDamage, OnActiveShield);
        }
    }

    private void OnTick(object obj)
    {
        if (owner == null) return;

        timer += 0.5f; //틱이 0.5초마다 돌기때문에

        if(timer >= so.Value)
        {
            timer = 0f;
            shieldActive = true;
            Debug.Log($"[RegenShield] {owner.name} 보호막 재생 완료!");
        }
    }

    private void OnActiveShield(object param)
    {
        if (param is not DamageData dmg) return;
        if (dmg.Target != owner) return;

        if(shieldActive)
        {
            shieldActive = false;
            dmg.Value = 0;
            Debug.Log($"[RegenShield] {owner.name}의 보호막이 공격을 무효화했습니다!");
        }
    }
}
