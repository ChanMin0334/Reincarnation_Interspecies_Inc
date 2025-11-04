using Microsoft.SqlServer.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveOnce : ArtifactEffect //1회부활
{
    public override void RegisterEvent()
    {
        base.RegisterEvent();
        EventManager.Instance.StartListening(EventType.OwnerDeath, OnOwnerDeath);
    }

    public override void UnregisterEvent()
    {
        EventManager.Instance.StopListening(EventType.OwnerDeath, OnOwnerDeath);
    }

    private void OnOwnerDeath(object param)
    {
        if (param is not Entity entity || entity != owner) return;
        if (data.isUsed) return;

        data.isUsed = true;
        owner.SetHP(owner.Data.MaxHP);
        Debug.Log("부활 유물 발동");    
    }
}
