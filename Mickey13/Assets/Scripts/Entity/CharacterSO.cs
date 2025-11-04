using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterClassEnum
{
    None,
    Warrior,
    Archer,
    Mage,
}
public class CharacterSO : EntitySO
{
    [SerializeField] private CharacterClassEnum charClass;
    [SerializeField] private RarityEnum rarity;

    [Header("LevelUp 배율")]
    [SerializeField] private float levelUpHP;
    [SerializeField] private float levelUpATK;

    [SerializeField] private Sprite fullSprite; //전신

    public SkillSO ActiveSkill_1;
    public SkillSO ActiveSkill_2;
    public SkillSO ActiveSkill_3;
    public SkillSO PassiveSkill;



    #region 프로퍼티
    public CharacterClassEnum CharClass => charClass;
    public override RarityEnum Rarity => rarity;
    public float LevelUpHP => levelUpHP;
    public float LevelUpATK => levelUpATK;
    public Sprite FullSprite => fullSprite;
    #endregion

#if UNITY_EDITOR
    public void ExcelInit(string _id, string _name, Sprite _sprite, string _descript , BaseStatSO _baseStat, CharacterClassEnum _charclass, RarityEnum _rarity, float _uphp, float _upatk, AttackType _attackType, Sprite _fullSprite)
    {
        id = _id;
        name = _name;
        sprite = _sprite;
        description = _descript;
        dataType = GameDataType.Character;

        baseStat = _baseStat;  
        charClass = _charclass;
        rarity = _rarity;

        levelUpHP = _uphp;
        levelUpATK = _upatk;

        attackType = _attackType;

        fullSprite = _fullSprite;
    }
#endif
    public static CharacterSO TryParse(EntitySO data)
    {
        try 
        { 
            return data as CharacterSO;
        }
        catch
        {
            return null;
        }
    }
}
