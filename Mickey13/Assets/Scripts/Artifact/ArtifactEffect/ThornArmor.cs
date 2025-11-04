using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornArmor : ArtifactEffect //가시갑옷
{
    public override void RegisterEvent()
    {
        base.RegisterEvent();
        EventManager.Instance.StartListening(EventType.OnDamaged, OnDamaged);
    }

    public override void UnregisterEvent()
    {
        if(EventManager.Instance != null)
            EventManager.Instance.StopListening(EventType.OnDamaged, OnDamaged);
    }

    private void OnDamaged(object param)
    {
        if (param is not DamageData dmg) return;

        if (dmg.Target == null || dmg.Attacker == null) return;

        if(dmg.Target == owner)
        {
            BigNumeric revengeDmg = dmg.Value * (so.Value / 100f);
            Debug.Log($"[RevengeStrike] {owner.name}이 {dmg.Attacker.name}에게 {revengeDmg:F1} 피해 반사");

            var revengeData = new DamageData
            {
                Attacker = owner,
                Target = dmg.Attacker,
                Value = revengeDmg,
                IsCritical = false,
                IsSkill = false,
                Type = DamageTypeEnum.Normal
            };

            dmg.Attacker.ApplyDamage(revengeData);
        }
    }
}
