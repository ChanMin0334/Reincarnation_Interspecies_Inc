using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceMaxHP : ArtifactEffect
{
    private float distance = 0f;

    private float lastBonus = 0f;

    public override void RegisterEvent()
    {
        Debug.Log("DisTanceMaxHP : RegisterEvent 실행");
        base.RegisterEvent();
        EventManager.Instance.StartListening(EventType.Tick, OnTick);
    }

    public override void UnregisterEvent()
    {
        OnDeactive();
        if(EventManager.Instance != null)
            EventManager.Instance.StopListening(EventType.Tick, OnTick);
    }

    public void OnTick(object obj)
    {
        if (owner == null)
        {
            Debug.Log("DistanceMaxHP의 owner이 null입니다.");
            return;
        }

        distance = User.Instance.ReincarnateData.MaxDistance;

        float increaseRate = distance * so.Value;

        owner.Data.Artifacts.HP.value -= lastBonus;
        owner.Data.Artifacts.HP.value += increaseRate;
        lastBonus = increaseRate;

        Debug.Log($"DistanceMaxHP : 거리 {distance} HP + {increaseRate}% 증가");
    }

    public override void OnDeactive()
    {
        if (owner == null) return;

        owner.Data.Artifacts.HP.value -= lastBonus;
        Debug.Log($"DistanceMaxHP : 비활성화됨 → HP 보정 {lastBonus} 제거");

        lastBonus = 0f;
    }
}
