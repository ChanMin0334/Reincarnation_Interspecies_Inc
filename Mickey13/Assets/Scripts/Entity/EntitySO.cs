using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    None,
    Melee, // 전사 - 휘두름
    Ranged, // 궁수 - 활 쏨
    Magic, // 마법사 - 마법 캐스팅
    Prod // 도적 - 찌름
}
public class EntitySO : GameData
{
    [SerializeField] protected BaseStatSO baseStat;

    // todo. 어택 타입 집어넣기
    //추가 공격 애니메이션 타입
    public AttackType attackType;

    public BaseStatSO BaseStat => baseStat;

    // 임시 등급
    public int Grade {  get; set; }

    //public Sprite GetSprite(SlotImageType imageType)
    //{
    //    switch (imageType)
    //    {
    //        case SlotImageType.Icon:
    //            return sprite;
    //        //case SlotImageType.Banner:
    //        //    return bannerSprite;
    //        //case SlotImageType.FullBody:
    //        //    return fullBodySprite;
    //        default:
    //            return null;
    //    }    
    //}


    //public virtual void Init(string _id, string _name, Sprite _sprite, NewBaseStatSO _stat, string _baseSkillID)
    //{
    //    id = _id;
    //    name = _name;
    //    entitySprite = _sprite;
    //    baseStat = _stat;
    //    baseSkillID = _baseSkillID;
    //}
}
