using UnityEngine;

public class ArtifactSO : GameData, ISlotUIData
{
    [SerializeField, Tooltip("등급")] private RarityEnum rarity; //등급
    [SerializeField, Tooltip("타겟")] private ApplyTargetTypeEnum target; //타켓
    [SerializeField, Tooltip("타겟의 클래스")] private CharacterClassEnum charClass; //타켓의 클래스

    //추가
    [SerializeField, Tooltip("유물효과의 Class 이름")] private string effectClassName; 
    [SerializeField, Tooltip("유물 지속시간, 유물 Value 등")] private float value;

    [SerializeField] private int maxCount = 1; //최대 보유량

    //일단 임시 추가
    [SerializeField] private StatType statType;


    #region 프로퍼티
    public override RarityEnum Rarity => rarity;
    public ApplyTargetTypeEnum Target => target;
    public CharacterClassEnum CharClass => charClass;

    public string EffectClassName => effectClassName;
    public float Value => value;

    public int MaxCount => maxCount;

    public StatType StatType => statType;

    #endregion
    public Sprite GetSprite(SlotImageType imageType)
    {
        return sprite;
    }
    
    public float GetCalculatedValue(int count)
    {
        return Value * count;
    }
    
#if UNITY_EDITOR
    public void ExcelInit(string _id, string _name, Sprite _sprite, string _descript, BaseStatSO _basestat, RarityEnum _rarity, ApplyTargetTypeEnum _targetType, CharacterClassEnum _charclass, string _effectClassName, float _value, StatType _statType)
    {
        id = _id;
        name = _name;
        sprite = _sprite;
        description = _descript;
        dataType = GameDataType.Artifact;
        rarity = _rarity;
        target = _targetType;
        charClass = _charclass;
        effectClassName = _effectClassName;
        value = _value;

        //statType = StatType.None;

        statType = _statType;
    }
#endif
}
