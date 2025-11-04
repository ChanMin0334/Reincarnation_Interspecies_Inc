using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

[ExcelAsset]
public class GameInfoExcel : ScriptableObject
{
    public List<EntityStatusExcel> Character_Status;
    public List<EntityStatusExcel> Enemy_Status;
    public List<CharacterInfoExcel> Character_IDs;

    public List<EnemyInfoExcel> Enemy_IDs;
    public List<QuestInfoExcel> QuestInfo; //수정필요?
    public List<RuneInfoExcel> Rune_IDs;
    public List<ArtifactInfoExcel> Artifact_IDs;

    //추가
    public List<SkillInfoExcel> Skill_IDs;
}

#region Status부분
[Serializable]
public class EntityStatusExcel
{
    public string ID;
    public float BaseHP;
    public float BaseATK;
    public float BaseCritChance;
    public float BaseCritMult;
    public float BaseATKCoolDown;
    public float BaseSkillCoolDown;
    public float BaseATKRange; //공격범위
    public float BaseATKArea; //사정거리
    public float BaseMoveSpeed;
}
#endregion
[Serializable]

public class GameDataInfoExcel //게임데이터 상속받는애들 기본적으로 다 들고있는 값
{
    public string ID;
    public string Name;
    public string Description;
}
[Serializable]
public class CharacterInfoExcel : GameDataInfoExcel
{
    public CharacterClassEnum Class; 
    public RarityEnum Rarity;  
    public float LevelUpHP;
    public float LevelUpATK;
    public string ActiveSkill1;
    public string ActiveSkill2;
    public string ActiveSkill3;
    public string PassiveSkill;
    public AttackType AttackType;
}
[Serializable]
public class EnemyInfoExcel : GameDataInfoExcel
{
    //public string EnemyTypeID;
    public EnemyTypeEnum EnemyRankID;
    public EnemySizeEnum EnemySizeID;
    public AttackType AttackType;
}

#region 퀘스트, 유물 관련

[Serializable]
public class QuestInfoExcel : GameDataInfoExcel
{
    public float requirement;
    public float duration;
    public BigNumericWrapper upgradeCost;
    public BigNumericWrapper reward;
    public float upgradeMult;
    public float rewardMult;
    //public float questIcon; // 승엽 추가 (UI에 퀘스트 아이콘 표시 해야하는 곳이 있어서 엑셀에 이미지 경로 추가 요청) 확인완
}

[Serializable]
public class RuneInfoExcel : GameDataInfoExcel
{
    public RarityEnum Rarity;
    public ApplyTargetTypeEnum TargetType;
    public CharacterClassEnum TargetClass;
    public string TargetID;

    // public EffectTypeEnum EffectType;
    public StatType EffectType;
    public float EffectValue;
}

[Serializable]
public class ArtifactInfoExcel : GameDataInfoExcel
{
    public RarityEnum Rarity;
    public ApplyTargetTypeEnum TargetType;
    public CharacterClassEnum TargetClass;

    public string EffectName;
    public float Value;

    public StatType StatType;
}

[Serializable]
public class SkillInfoExcel : GameDataInfoExcel
{
    public SkillTypeEnum Type;
    public bool IsPassive;
    public EventType TriggerEvent;
    public float CoolDown;
    public string EffectData1;
    public string EffectData2;
    public string EffectData3;
    public string EffectData4;
    public string EffectData5;
}

#endregion