using UnityEngine;

public enum DamageTypeEnum //속성이 추가된다면
{
    Normal,
    Fire,
    Ice,
    Poison
}

public class DamageData
{
    public Entity Attacker;
    public Entity Target;
    //public float Value;
    public BigNumeric Value;
    public bool IsCritical;
    public bool IsSkill;
    public DamageTypeEnum Type;

    public DamageData() { }
    public DamageData(Entity attacker, Entity target, BigNumeric value, bool isCritical = false , bool isSkill = false, DamageTypeEnum type = DamageTypeEnum.Normal)
    {
        Attacker = attacker;
        Target = target;
        Value = value;
        IsCritical = isCritical;
        IsSkill = isSkill;
        Type = type;
    }
}
