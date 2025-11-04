using System;
using UnityEngine;

public enum EffectTriggerTypeEnum
{
    Target, //적에게 바로 이펙트 생성 //Ex) 메이플의 매직클로
    OwnerToTarget, //Effect Owner -> Target //Ex) 투사체, 발사체
    Owner //Owner한테 이펙트 생성 //Ex) 자버프 계열
}

[Serializable]
public class EffectData
{
    public GameObject effectPrefab; //이펙트프리팹
    public EffectTriggerTypeEnum triggerType;
    public float duration = 0f; //0이면 애니메이션 한사이클 돌면 끝. 값이 있으면 지속시간 따라가게

    [Header("애니메이션 속도")]
    [Range(0.1f, 5f)] public float animSpeed = 1f;

    [Header("OwnerToTarget Type일때만 사용")]
    public float moveSpeed = 10f;
}
