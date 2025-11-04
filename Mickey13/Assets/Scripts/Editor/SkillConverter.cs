using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class SkillConverter //Prefab연결은 자동으로 되는데 아직 세부적인 부분들은 직접 조정해야함
{
    [MenuItem("Tools/Convert/GameInfoExcel -> SkillSO", false, 100)]
    public static void Convert()
    {
        var excelSO = AssetDatabase.LoadAssetAtPath<GameInfoExcel>("Assets/Datas/GameInfoExcel.asset");
        if (excelSO == null)
        {
            Debug.LogError("Excel파일을 찾을수 없습니다.");
            return;
        }

        string folderPath = "Assets/Resources/SOData/SkillSO";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError("파일 경로가 없습니다.");
            return;
        }

        for (int i = 0; i < excelSO.Skill_IDs.Count; i++)
        {
            var data = excelSO.Skill_IDs[i];

            SkillSO skill = ScriptableObject.CreateInstance<SkillSO>();

            List<SkillEffect> parsedEffects = new();

            string[] effectStrings = new string[]
            {
                data.EffectData1,
                data.EffectData2,
                data.EffectData3,
                data.EffectData4,
                data.EffectData5
            };

            foreach (var effectStr in effectStrings)
            {
                if (string.IsNullOrWhiteSpace(effectStr))
                    continue;

                parsedEffects.AddRange(ParseEffects(effectStr));
            }

            string spriteFolderPath = "Assets/Datas/Sprites/Skill";

            Sprite skillIcon = null;
            string[] guids = AssetDatabase.FindAssets(data.ID, new[] { spriteFolderPath });
            Debug.Log($"[SkillConverter] {data.ID} 검색 결과: {guids.Length}개 (폴더: {spriteFolderPath})");

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                skillIcon = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }
            else
            {
                Debug.LogWarning($"SkillConverter : 스프라이트를 찾을 수 없습니다: {data.ID}");
            }

            string prefabFolderPath = "Assets/Datas/Prefabs/Effects";
            GameObject effectPrefab = null;
            string[] prefabGuids = AssetDatabase.FindAssets(data.ID, new[] { prefabFolderPath });

            if (prefabGuids.Length > 0)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuids[0]);
                effectPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            }
            else
            {
                Debug.LogWarning($"SkillConverter : 이펙트 프리팹을 찾을 수 없습니다: {data.ID}");
            }

            skill.ExcelInit(
                data.ID,
                data.Name,
                skillIcon,
                data.Description,
                data.CoolDown,
                data.Type,
                data.IsPassive,
                data.TriggerEvent,
                parsedEffects
            );

            foreach(var effect in skill.Effects)
            {
                if(effect.effectData == null)
                {
                    effect.effectData = new EffectData();
                }

                effect.effectData.effectPrefab = effectPrefab;
            }

            string assetID = string.IsNullOrEmpty(data.ID) ? $"NoID{i}" : data.ID;
            string assetName = string.IsNullOrEmpty(data.Name) ? $"NoName{i}" : data.Name; //data.name이 null이면 true //파일명 변환용
            string assetPath = $"{folderPath}/{assetID}_{assetName}.asset";
            AssetDatabase.CreateAsset(skill, assetPath); //SO 생성
        }

        AssetDatabase.SaveAssets(); //저장
        AssetDatabase.Refresh(); //에디터 반영

        Debug.Log("변환 완료");
    }

    private static List<SkillEffect> ParseEffects(string data)
    {
        var effects = new List<SkillEffect>();
        if (string.IsNullOrWhiteSpace(data))
            return effects;

        string entry = data.Trim();

        string[] parts = entry.Split(':', 5, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 5)
        {
            Debug.LogWarning($"SkillEffect 형식 오류: {entry}");
            return effects;
        }

        try
        {
            for (int i = 0; i < parts.Length; i++)
                parts[i] = parts[i].Trim();

            if (!Enum.TryParse(parts[0], true, out SkillEffectTypeEnum effectType))
            {
                Debug.LogWarning($"[SkillConverter] effectType 파싱 실패: {parts[0]}");
                return effects;
            }

            if (!Enum.TryParse(parts[1], true, out SkillTargetTypeEnum targetType))
            {
                Debug.LogWarning($"[SkillConverter] targetType 파싱 실패: {parts[1]}");
                return effects;
            }

            float power = float.TryParse(parts[2], out var p) ? p : 0f;
            float duration = float.TryParse(parts[3], out var d) ? d : 0f;

            BuffTypeEnum buffType = BuffTypeEnum.None;
            Enum.TryParse(parts[4], true, out buffType);

            SkillEffect effect = new SkillEffect
            {
                effectType = effectType,
                targetType = targetType,
                power = power,
                duration = duration,
                buffType = buffType,
                effectDescription = $"[{effectType}] {targetType} {power}% {duration}s {buffType}"
            };

            effects.Add(effect);
        }
        catch (Exception ex)
        {
            Debug.LogError($"SkillEffect 파싱 실패: {entry}\n{ex}");
        }

        return effects;
    }
}
