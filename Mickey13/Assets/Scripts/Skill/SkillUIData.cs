using System;

[Serializable]
public class SkillUIData
{
    public SkillSO SkillSO { get; private set; }
    public int Level { get; private set; } // 추후 스킬 강화 시스템이 생긴다면 사용

    public SkillUIData(SkillSO skillSO, int level = 0)
    {
        SkillSO = skillSO;
        Level = level;
    }
}
