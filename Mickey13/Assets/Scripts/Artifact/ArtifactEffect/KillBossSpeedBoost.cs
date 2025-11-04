using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBossSpeedBoost : RevertArtifactStat //보스처치시 일정시간 이동속도 증가
{
    private float duration = 5f; //지속시간
    private float speedMult = 1.5f; //속도배율

    public override void RegisterEvent()
    {
        base.RegisterEvent();
        EventManager.Instance.StartListening(EventType.BossKilled, OnBossKilled);
    }

    public override void UnregisterEvent()
    {
        EventManager.Instance.StopListening(EventType.BossKilled, OnBossKilled);
    }

    public void OnBossKilled()
    {
       owner.StartCoroutine(ActiveForSeconds(duration));
    }

    public override void OnActive()
    {     
        var s = owner.FinalStat;
        s.MoveSpeed *= speedMult;
        Debug.Log($"KillBossSpeedBoost : 보스를 처치하여 속도 증가!");

        SetRevert(() => s.MoveSpeed /= speedMult);
    }
}
