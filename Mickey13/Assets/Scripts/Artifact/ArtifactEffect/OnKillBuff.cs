using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnKillBuff : ArtifactEffect
{
    public override void RegisterEvent()
    {
        base.RegisterEvent();
        Debug.Log($"[OnKillBuff] {owner.name} : EnemyDied 구독 완료");
        EventManager.Instance.StartListening(EventType.EnemyDied, OnKill);
    }

    public override void UnregisterEvent()
    {
        EventManager.Instance.StopListening(EventType.EnemyDied, OnKill);
    }

     private void OnKill(object param)
    {
        if (param is DamageData dmg && dmg.Attacker == owner)
        {
            owner.Data.Artifacts.Atk.value += 1f;
            Debug.Log($"{owner.Name}보유자가 적을처치해 공격력 1증가했습니다.");
        }
    }
}
