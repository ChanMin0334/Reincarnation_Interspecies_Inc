using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum SkillTypeEnum
{
    Attack,
    Support,
    Magic,
    Passive,
    Utility,
}

public enum SkillTargetTypeEnum
{
    Self,
    Ally,
    AllAllies,
    Enemy,
    AllEnemies,
}

public enum SkillEffectTypeEnum
{
    Damage,
    Heal,
    Buff,
    Debuff,
    Special, //?? 쓸지안쓸지 모르겠음
    TickDamage, //지속피해

}

public enum SkillConditionTypeEnum
{
    None, 
    TargetHPBelow, //대상 체력이 일정 이하일때
    SelfHPBelow, //자신 체력이 일정 이하일때
    OnKill, //적을 처치했을때
    OnCrit, //치명타 발생했을때
    RandomChance //랜덤확률
}

[Serializable]
public class SkillEffect
{
    public SkillEffectTypeEnum effectType;
    public SkillTargetTypeEnum targetType;
    public float power;
    public float duration;
    public BuffTypeEnum buffType = BuffTypeEnum.None;
    [TextArea] public string effectDescription;

    [Header("부과효과 조건")]
    public SkillConditionTypeEnum condition = SkillConditionTypeEnum.None;
    public float conditionValue;

    [Header("지속피해 스킬")]
    public float tickInterval; //틱 간격

    public EffectData effectData; //스킬 연출 Gameobject
}

public class SkillSO : GameData, ISlotUIData
{
    [Header("공통데이터")]
    [SerializeField] private float coolDown;
    [SerializeField] private SkillTypeEnum type;
    [SerializeField] private bool isPassive;
    [SerializeField] private EventType triggerEvent;
    [SerializeField] private List<SkillEffect> effects = new();


    #region 프로퍼티
    public float CoolDown => coolDown;
    public SkillTypeEnum Type => type;
    public bool IsPassive => isPassive;
    public EventType TriggerEvent => triggerEvent;
    public List<SkillEffect> Effects
    {
        get { return effects; }
        set { effects = value; }
    }
    #endregion

    public Sprite GetSprite(SlotImageType imageType)
    {
        return Sprite;
    }

#if UNITY_EDITOR
    public void ExcelInit(string _id, string _name, Sprite _sprite, string _descript,float _cooldown, SkillTypeEnum _type, bool _isPassive, EventType _triggerEvent, List<SkillEffect> _effects)
    {
        id = _id;
        name = _name;
        sprite = _sprite;
        description = _descript;
        dataType = GameDataType.Skill;

        coolDown = _cooldown;
        type = _type;
        isPassive = _isPassive;
        triggerEvent = _triggerEvent;
        effects = _effects;
    }
#endif
}