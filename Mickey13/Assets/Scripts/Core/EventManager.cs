using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType // 나중에 개별 스크립트 분리
{
    None,
    AddCharacterToInventory, // 캐릭터 추가
    UpdateCharacterToInventory, // 캐릭터 업데이트
    CharacterDead, // 캐릭터가 죽었을 때
    AllCharacterDead, // 모든 캐릭터가 죽었을 때, 이거 지금 아무것도 안되어있는데 연출추가하면 될 듯?
    AddArtifactToInventory, // 유물 추가
    UpdateArtifactToInventory, // 유물 업데이트
    AddRuneToInventory,// 룬 추가
    UpdateRuneToInventory, // 룬 업데이트
    StartReincarnate, // 환생하기
    Reincarnating, // 환생중
    EndReincarnate, // 환생끝났을때
    EnemyDied, // 적이 죽었을 때
    FormationChanged, // 편성 변경됨
    CharacterStatChanged, // 캐릭터 스탯 변경됨
    //유물용 이벤트Type

    BossKilled, //보스 처치시
    MiddleBossKilled, //중간보스 처치시
    EnemyKilled, //일반 몬스터 처치시

    BeforeAttack, //공격 직전
    CalculateDamage, //데미지 계산중
    SkillUsed, //스킬사용시

    OwnerHit, //플레이어 피격시
    OwnerDeath, //플레이어 사망시
    AllyDamaged, //아군이 피해를 받았을경우
    Tick, //주기적으로
    OnDamaged,
    OnHeal,
    AfterAttack,
    TargetDeath,
    OnRespawn,

    OnChangedUpgradeCount, // 강화 최대 횟수 변경 알림 이벤트
    ItemUpgraded, // 강화 실행 이벤트
}

public class EventManager : Singleton<EventManager>
{
    private Dictionary<EventType, Action> eventListeners = new();
    private Dictionary<EventType, Action<object>> eventListenersWithObjcet = new();


    #region 데이터 없는 이벤트
    /// <summary>
    /// 이벤트 구독
    /// </summary>
    public void StartListening(EventType eventName, Action listener)
    {
        // 이벤트가 존재하면 리스너 추가, 존재하지 않으면 새로 등록
        if(eventListeners.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent += listener;
            eventListeners[eventName] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            eventListeners.Add(eventName, thisEvent);
        }
    }
    /// <summary>
    /// 이벤트 구독 취소
    /// </summary>
    public void StopListening(EventType eventName, Action listener)
    {
        if (Instance == null) return;
        if(eventListeners.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent -= listener;
            eventListeners[eventName] = thisEvent;
        }
    }

    /// <summary>
    /// 이벤트 발행
    /// </summary>
    public void TriggerEvent(EventType eventName)
    {
        if(eventListeners.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent?.Invoke();
        }
    }

    #endregion

    #region 데이터를 포함하는 모든 이벤트

    /// <summary>
    /// 이벤트 구독
    /// </summary>
    public void StartListening(EventType eventName, Action<object> listener)
    {
        // 이벤트가 존재하면 리스너 추가, 존재하지 않으면 새로 등록
        if(eventListenersWithObjcet.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent += listener;
            eventListenersWithObjcet[eventName] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            eventListenersWithObjcet.Add(eventName, thisEvent);
        }
    }
    /// <summary>
    /// 이벤트 구독 취소
    /// </summary>
    public void StopListening(EventType eventName, Action<object> listener)
    {
        if (Instance == null) return;
        if(eventListenersWithObjcet.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent -= listener;
            eventListenersWithObjcet[eventName] = thisEvent;
        }
    }

    /// <summary>
    /// 이벤트 발행
    /// </summary>
    public void TriggerEvent(EventType eventName, object data)
    {
        if(eventListenersWithObjcet.TryGetValue(eventName, out Action<object> thisEvent))
        {
            thisEvent?.Invoke(data);
        }
    }

    #endregion
}
