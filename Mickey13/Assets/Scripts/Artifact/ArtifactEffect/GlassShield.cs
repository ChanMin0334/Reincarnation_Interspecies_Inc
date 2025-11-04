using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassShield : ArtifactEffect //유리방패 Atk 0.5배 체력 1.5배
{
    public override void RegisterEvent()
    {
        base.RegisterEvent();
        if(data != null && data.isUsed)
        {
            Debug.Log($"GlassShield : {so.Name}은 이미 적용되어있습니다.");
            return;
        }
        OnActive();
    }

    public override void UnregisterEvent()
    {
        OnDeactive();
    }

    public override void OnActive()
    {
        var stat = owner.Data.Artifacts;
        stat.Atk.value -= 50f;
        stat.HP.value += 50f;

        data.isUsed = true;
    }

    public override void OnDeactive()
    {
        var stat = owner.Data.Artifacts;
        stat.Atk.value += 50;
        stat.HP.value -= 50f;

        data.isUsed = false;
    }
}
