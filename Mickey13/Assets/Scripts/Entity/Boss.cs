using UnityEngine;

public class Boss : Enemy
{
    protected override void Awake()
    {
        base.Awake();
        canMove = false; // 보스는 이동하지 않음
    }

    protected override void Update()
    {
        base.Update();
        // 보스 전용 추가 로직이 필요하면 여기에 작성
    }

    //private void OnDestroy()
    //{
    //    EventManager.Instance.TriggerEvent(EventType.BossKilled);
    //    EventManager.Instance.TriggerEvent(EventType.EnemyDied, this);
    //    EnemyManager.Instance.OnDieBoss(this);
    //    // 드롭 테이블에 따른 아이템 드롭 처리
    //}

    protected override void Die()
    {
        var dmgData = new DamageData(lastAttacker, this, 0f);
        EventManager.Instance.TriggerEvent(EventType.BossKilled);
        EventManager.Instance.TriggerEvent(EventType.EnemyDied, dmgData);
        EventManager.Instance.TriggerEvent(EventType.OnRespawn); // 보스 처치하면 캐릭터 리스폰
        base.Die();
    }
}
