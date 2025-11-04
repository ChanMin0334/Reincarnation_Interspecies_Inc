using UnityEngine;

public class RuneSO : GameData , ISlotUIData
{
    [SerializeField] private StatType effectType; //오르는 스탯의 Type
    [SerializeField] private float effectValue; //증가 수치

    [SerializeField] private RarityEnum rarity;
    [SerializeField] private ApplyTargetTypeEnum target;
    [SerializeField] private CharacterClassEnum charClass;
    [SerializeField] private string targetID;

    public int MaxStack
    {
        get
        {
            return rarity switch
            {
                RarityEnum.None => 0,
                RarityEnum.Common => 50,
                RarityEnum.Rare => 30,
                RarityEnum.Epic => 15,
                RarityEnum.Legendary => 10, //임시 갯수 증가
                RarityEnum.Unique => 5, //
                _ => 0
            };
        }
    }

    #region 프로퍼티
    // public EffectTypeEnum EffectType => effectType;
    public StatType EffectType => effectType;
    public float EffectValue => effectValue;


    public override RarityEnum Rarity => rarity;
    public ApplyTargetTypeEnum Target => target;
    public CharacterClassEnum CharClass => charClass;
    public string TargetID => targetID;
    #endregion


    public Sprite GetSprite(SlotImageType imageType)
    {
        return sprite;
    }

#if UNITY_EDITOR
    public void ExcelInit(string _id, string _name, Sprite _sprite, string _descript, RarityEnum _rarity, ApplyTargetTypeEnum _targetType ,CharacterClassEnum _charclass, string _targetID, StatType _effectType, float _effectValue)
    {
        id = _id;
        name = _name;
        sprite = _sprite;
        description = _descript;
        dataType = GameDataType.Rune;
        rarity = _rarity;
        target = _targetType;
        charClass = _charclass;
        targetID = _targetID;

        effectType = _effectType;
        effectValue = _effectValue;
    }    
#endif
}
