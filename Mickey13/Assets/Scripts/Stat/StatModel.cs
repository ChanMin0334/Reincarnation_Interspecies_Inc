using System;
using UnityEngine;
using Unity.VisualScripting;

[Serializable]
public class StatModel
{
    //public float HP;
    //public float Atk;
    public BigNumericWrapper HP;
    public BigNumericWrapper Atk;
    public float AtkCooldown;
    public float SkillCooldown;
    public float AtkRange;
    public float CritChance;
    public float AtkArea;
    public float MoveSpeed;
    public float CritMult; //치명타 배율

    public float DamageDealMult; //가하는 피해 증가 배율
    public float DamageTakenMult; //테스트용 받는피해 배율
    


    public StatModel() { HP = 0; Atk = 0; }

    public StatModel(
        float hp, float atk, float atkCooldown, float skillCooldown,
        float atkRange, float critChance, float atkArea, float moveSpeed, float critMult)
    {
        HP = hp; Atk = atk; AtkCooldown = atkCooldown; SkillCooldown = skillCooldown;
        AtkRange = atkRange; CritChance = critChance; AtkArea = atkArea; MoveSpeed = moveSpeed;
        //추가된 부분
        CritMult = critMult;

        DamageDealMult = 100f;
        DamageTakenMult = 100f;
    }

    // 합산, 최종 스탯 계산 등
    public static StatModel operator +(StatModel a, StatModel b) => new StatModel
    {
        HP = a.HP.value + b.HP.value,
        Atk = a.Atk.value + b.Atk.value,
        AtkCooldown = a.AtkCooldown + b.AtkCooldown,
        SkillCooldown = a.SkillCooldown + b.SkillCooldown,
        AtkRange = a.AtkRange + b.AtkRange,
        CritChance = a.CritChance + b.CritChance,
        AtkArea = a.AtkArea + b.AtkArea,
        MoveSpeed = a.MoveSpeed + b.MoveSpeed,
        CritMult = a.CritMult + b.CritMult,

        //추가
        DamageTakenMult = a.DamageTakenMult + b.DamageTakenMult,
        DamageDealMult = a.DamageDealMult + b.DamageDealMult
    };

    // 차감, 사용 할일이 있을까 모르겠긴 함
    public static StatModel operator -(StatModel a, StatModel b) => new StatModel
    {
        HP = a.HP.value - b.HP.value,
        Atk = a.Atk.value - b.Atk.value,
        AtkCooldown = a.AtkCooldown - b.AtkCooldown,
        SkillCooldown = a.SkillCooldown - b.SkillCooldown,
        AtkRange = a.AtkRange - b.AtkRange,
        CritChance = a.CritChance - b.CritChance,
        AtkArea = a.AtkArea - b.AtkArea,
        MoveSpeed = a.MoveSpeed - b.MoveSpeed,
        CritMult = a.CritMult - b.CritMult,

        //추가
        DamageTakenMult = a.DamageTakenMult - b.DamageTakenMult,
        DamageDealMult = a.DamageDealMult - b.DamageDealMult
    };

    // 배율, 레벨업 or 버프, 디버프 등 배율로 적용되는 경우
    public static StatModel operator *(StatModel a, float m) => new StatModel
    {
        HP = a.HP.value* m,
        Atk = a.Atk.value * m,
        AtkCooldown = a.AtkCooldown * m,
        SkillCooldown = a.SkillCooldown * m,
        AtkRange = a.AtkRange * m,
        CritChance = a.CritChance * m,
        AtkArea = a.AtkArea * m,
        MoveSpeed = a.MoveSpeed * m,
        CritMult = a.CritMult * m,

        //추가
        DamageTakenMult = a.DamageTakenMult * m,
        DamageDealMult = a.DamageDealMult * m
    };

    public static StatModel operator *(StatModel a, StatModel b)
    {
        return new StatModel
        {
            HP = a.HP.value * b.HP.value,
            Atk = a.Atk.value * b.Atk.value,
            AtkCooldown = a.AtkCooldown * b.AtkCooldown,
            SkillCooldown = a.SkillCooldown * b.SkillCooldown,
            AtkRange = a.AtkRange * b.AtkRange,
            CritChance = a.CritChance * b.CritChance,
            AtkArea = a.AtkArea * b.AtkArea,
            MoveSpeed = a.MoveSpeed * b.MoveSpeed,
            CritMult = a.CritMult * b.CritMult,

            //추가
            DamageTakenMult = a.DamageTakenMult * b.DamageTakenMult,
            DamageDealMult = a.DamageDealMult * b.DamageDealMult
        };
    }

    public static StatModel operator *(float m, StatModel a) => a * m;
    public static StatModel operator /(StatModel a, float d) => a * (1f / d);

    // 편의 생성자, 모든 값 0
    public static StatModel Zero() => new StatModel();

    /// <summary>
    /// * 되는 스탯들의 기본값을 100으로 하기위한 함수
    /// </summary>
    /// <returns></returns>
    public static StatModel One() => new StatModel
    {
        HP = 100,
        Atk = 100,
        AtkCooldown = 100f,
        SkillCooldown = 100f,
        AtkRange = 100f,
        CritChance = 100f,
        AtkArea = 100f,
        MoveSpeed = 100f,
        CritMult = 100f,

        DamageTakenMult = 100f,
        DamageDealMult = 100f
    };

    /// <summary>
    /// 음수로 값이 떨어지지 않게 하기 위한 함수
    /// </summary>
    /// <returns></returns>
    public StatModel ClampMinZero()
    {

        HP = BigNumeric.Max(0, HP.value);
        Atk = BigNumeric.Max(0, Atk.value);
        AtkCooldown = MathF.Max(0, AtkCooldown);
        SkillCooldown = MathF.Max(0, SkillCooldown);
        AtkRange = MathF.Max(0, AtkRange);
        CritChance = MathF.Max(0, CritChance);
        AtkArea = MathF.Max(0, AtkArea);
        MoveSpeed = MathF.Max(0, MoveSpeed);
        CritMult = MathF.Max(0, CritMult);

        DamageTakenMult = MathF.Max(0, DamageTakenMult);
        DamageDealMult = MathF.Max(0, DamageDealMult);

        return this;
    }

    public StatModel Clone()
    {
        return new StatModel
        {
            HP = this.HP?.Clone(),
            Atk = this.Atk?.Clone(),
            AtkCooldown = this.AtkCooldown,
            SkillCooldown = this.SkillCooldown,
            AtkRange = this.AtkRange,
            CritChance = this.CritChance,
            AtkArea = this.AtkArea,
            MoveSpeed = this.MoveSpeed,
            CritMult = this.CritMult,

            DamageTakenMult = this.DamageTakenMult,
            DamageDealMult = this.DamageDealMult
        };
    }

    public override string ToString()
    {
        return $"StatModel(HP: {HP.value}, Atk: {Atk.value}, AtkCooldown: {AtkCooldown}, SkillCooldown: {SkillCooldown}, " +
               $"AtkRange: {AtkRange}, CritChance: {CritChance}, AtkArea: {AtkArea}, MoveSpeed: {MoveSpeed}, CritMult: {CritMult})";
    }
}