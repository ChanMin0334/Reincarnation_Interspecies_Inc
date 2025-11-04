using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetSkill : ArtifactEffect //버그 Fix 완
{
    public override void RegisterEvent()
    {
        base.RegisterEvent();
        EventManager.Instance.StartListening(EventType.CalculateDamage, OnSkillUsed);
    }

    public override void UnregisterEvent()
    {
        if(EventManager.Instance != null)
            EventManager.Instance.StopListening(EventType.CalculateDamage, OnSkillUsed);
    }

    private void OnSkillUsed(object obj)
    {
        if (owner == null)
        {
            return;
        }
   
        var dmg = obj as DamageData;

        if (dmg == null) return;

        if (dmg.Attacker != owner || !dmg.IsSkill) return;

        float chance = so.Value;
        if (Random.value > chance / 100f) return;

        (owner as Character)?.ResetSkillCooldown();

        Debug.Log($"ResetSkill : {owner.Name}이 스킬공격 {chance}%로 쿨타임이 초기화 됐습니다.");
    }
}
