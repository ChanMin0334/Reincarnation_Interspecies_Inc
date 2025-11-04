using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class StatConverter
{
    [MenuItem("Tools/Convert/GameInfoExcel => AllTypeBaseStatSO", false, 30)]
    public static void ConvertAllTypeStat()
    {
        ConvertCharacter();
        ConvertEnemy();
        //ConvertArtifact();
    }
    public static void ConvertCharacter()
    {
        ConvertStatList("Assets/Scriptables/StatSO/Character", (excel) => excel.Character_Status);
    }
    public static void ConvertEnemy()
    {
        ConvertStatList("Assets/Scriptables/StatSO/Enemy", (excel) => excel.Enemy_Status);
    }

    //public static void ConvertArtifact()
    //{
    //    ConvertStatList("Assets/Scriptables/StatSO/Artifact", (excel) => excel.Artifact_Status);
    //}

    private static void ConvertStatList(string folderpath, Func<GameInfoExcel, List<EntityStatusExcel>> select)
    {
        var excelSO = AssetDatabase.LoadAssetAtPath<GameInfoExcel>("Assets/Datas/GameInfoExcel.asset");
        if(excelSO == null)
        {
            Debug.LogError("Excel 파일을 찾을수 없습니다.");
            return;
        }

        if(!AssetDatabase.IsValidFolder(folderpath))
        {
            Debug.LogError($"경로 폴더가 없습니다.{folderpath}");
            return;
        }

        var list = select(excelSO);

        for(int i = 0; i < list.Count; i++)
        {
            var data = list[i];

            BaseStatSO stat = ScriptableObject.CreateInstance<BaseStatSO>();
            stat.ExcelInit(
                data.ID,
                data.BaseHP,
                data.BaseATK,
                data.BaseCritChance,
                data.BaseCritMult,
                data.BaseATKCoolDown,
                data.BaseSkillCoolDown,
                data.BaseATKRange,
                data.BaseATKArea,
                data.BaseMoveSpeed
                );

            string assetID = string.IsNullOrEmpty(data.ID) ? $"EmptyID{i}" : data.ID;
            string assetPath = $"{folderpath}/{assetID}.asset";

            AssetDatabase.CreateAsset(stat, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("변환완료");
    }
}
