using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamHealOnKill : ArtifactEffect //어쩌피 각각캐릭터가 이걸 갖고 킬을 할때 힐을한다면 팀 전체 힐이랑 똑같으므로 개개인 구독
{
    public override void RegisterEvent()
    {
        base.RegisterEvent();
        EventManager.Instance.StartListening(EventType.EnemyDied, OnEnemyKilled);
    }

    public override void UnregisterEvent()
    {
        EventManager.Instance.StopListening(EventType.EnemyDied, OnEnemyKilled);
    }

    private void OnEnemyKilled(object obj)
    {
        if (owner == null) return;

        float ratio = so.Value;
        var healAmount = owner.Data.MaxHP.value * (ratio / 100f);

        owner.Heal(healAmount);

        Debug.Log($"TeamHealOnKill : 적을 처치하여 {healAmount}만큼 체력을 회복했습니다.");
    }
}
