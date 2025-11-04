using UnityEngine;

public enum BuffTypeEnum
{
    None,
    AtkUp, //공증
    AtkDown, //공다운
    //DmgUp, //뎀증
    //DmgDown, //뎀다운
    AtkSpeedUp, //공속업
    AtkSpeedDown, //공속다운
    CritChanceUp, //치명타확률증가
    CritChanceDown, //치명타확률 다운
    CritDmgUp, //치명타 피해 증가
    CritDmgDown, //치명타 피해 감소
    Stun, //스턴
    RegenHp, //체력재생
    SkillCoolDown, //스킬쿨감
    DamageDealMultUp, //가하는 피해 증가
    DamageDealMultDown, // 감소
    DamageTakenMultUp, //받는 피해 증가
    DamageTakenMultDown, //감소
}

[System.Serializable]
public class BuffData
{
    public BuffTypeEnum Type;
    public float Value;
    public float Duration;
    public float ElapsedTime;

    public BuffData(BuffTypeEnum type, float value, float duration)
    {
        Type = type;
        Value = value;
        Duration = duration;
        ElapsedTime = 0f;
    }
}
