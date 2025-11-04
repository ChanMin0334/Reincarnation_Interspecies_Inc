using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritOnFullHP : ArtifactEffect //상대 체력이 100%일때 확정 치명타
{
    public override void RegisterEvent()
    {
        base.RegisterEvent();
        EventManager.Instance.StartListening(EventType.BeforeAttack, OnBeforeAttack);
    }

    public override void UnregisterEvent()
    {
        if(EventManager.Instance != null)
            EventManager.Instance.StopListening(EventType.BeforeAttack, OnBeforeAttack);
    }

    private void OnBeforeAttack(object param)
    {
        if (param is not DamageData dmg) return;
        if (dmg.Attacker == null || dmg.Target == null) return;
        if (dmg.Attacker != owner) return;

        float ratio = dmg.Target.Data.curHP.value.Ratio(dmg.Target.Data.MaxHP);

        if(ratio >= 0.999f)
        {
            dmg.IsCritical = true;
            var debugDamage = dmg.Attacker.FinalStat.Atk.value * (dmg.Attacker.FinalStat.CritMult / 100f);
            Debug.Log($"[CritOnFullHP] {dmg.Target.name}은 풀피 상태! {owner.name}의 공격이 확정 치명타{debugDamage} 피해로 적용됩니다!");
        }
    }
}
