using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyTypeEnum
{
    Normal,
    MiddleBoss,
    Boss
}

public enum EnemySizeEnum
{
    Small,
    Medium,
    Large
}

public class EnemySO : EntitySO
{
    //###임시 
    [SerializeField] private EnemyTypeEnum enemyType;
    [SerializeField] private EnemySizeEnum enemySize;

    [Header("LevelUp 배율")]
    [SerializeField] private float levelUpHP;
    [SerializeField] private float levelUpATK;
    [SerializeField] private DropTable dropTable;

    #region 프로퍼티
    public EnemyTypeEnum EnemyType => enemyType;
    public EnemySizeEnum EnemySizeEnum => enemySize;
    public float LevelUpHP => levelUpHP;
    public float LevelUpATK => levelUpATK;

    public DropTable DropTable => dropTable;
    #endregion

#if UNITY_EDITOR
    public void ExcelInit(
        string _id,
        string _name,
        Sprite _sprite,
        string _descript,
        BaseStatSO _baseStat,
        EnemyTypeEnum _enemyType,
        EnemySizeEnum _enemySize,
        float _uphp,
        float _upatk,
        DropTable _dropTable,
        AttackType _attackType)
    {
        id = _id;
        name = _name;
        sprite = _sprite;
        description = _descript;
        dataType = GameDataType.Enemy;
        enemySize = _enemySize;

        baseStat = _baseStat;
        enemyType = _enemyType;

        levelUpHP = _uphp;
        levelUpATK = _upatk;
        dropTable = _dropTable;

        attackType = _attackType;
    }
#endif

    public static EnemySO TryParse(EntitySO data)
    {
        try
        {
            return data as EnemySO;
        }
        catch
        {
            return null;
        }
    }
}
