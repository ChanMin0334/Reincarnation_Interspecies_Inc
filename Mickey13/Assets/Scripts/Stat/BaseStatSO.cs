using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStatSO : ScriptableObject
{
    public string id; //기본 스탯의 대상이 되는 캐릭터, 몬스터 id

    //리팩토링 파트
    [SerializeField] protected StatModel value = new StatModel();

    public StatModel Value => value.Clone();
    
    public BigNumeric _NewBaseHP => value.HP;
    public BigNumeric _NewBaseAtk => value.Atk;
    public float _NewBaseAtkCoolDown => value.AtkCooldown;
    public float _NewBaseSkillCoolDown => value.SkillCooldown;
    public float _NewBaseAtkRange => value.AtkRange;
    public float _NewBaseCritChance => value.CritChance;
    public float _NewBaseCritMult => value.CritMult;
    public float _NewBaseAtkArea => value.AtkArea;
    public float _NewBaseMoveSpeed => value.MoveSpeed;


    //추가된 부분

    public virtual void Init(StatModel stat)
    {
        value = stat;
    }

#if UNITY_EDITOR
    public void ExcelInit(string baseID, float hp, float atk, float critChance, float critMult, float atkCd, float skillCd, float atkRange, float atkArea, float moveSpeed)
    {
        id = baseID;
        value.HP = hp;
        value.Atk = atk;
        value.CritChance = critChance;
        value.CritMult = critMult;
        value.AtkCooldown = atkCd;
        value.SkillCooldown = skillCd;
        value.AtkRange = atkRange;
        value.AtkArea = atkArea;
        value.MoveSpeed = moveSpeed;

        value.DamageTakenMult = 100f;
        value.DamageDealMult = 100f;
    }
#endif
}