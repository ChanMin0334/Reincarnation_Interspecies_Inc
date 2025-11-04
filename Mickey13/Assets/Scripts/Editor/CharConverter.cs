using UnityEngine;
using UnityEditor;

public class CharConverter
{
    [MenuItem("Tools/Convert/GameInfoExcel -> CharacterSO", false, 50)]
    public static void Convert()
    {
        var excelSO = AssetDatabase.LoadAssetAtPath<GameInfoExcel>("Assets/Datas/GameInfoExcel.asset");
        if (excelSO == null)
        {
            Debug.LogError("Excel파일을 찾을수 없습니다.");
            return;
        }

        string folderPath = "Assets/Resources/SOData/CharacterSO"; //변환한 데이터의 위치 경로
        if (!AssetDatabase.IsValidFolder(folderPath)) //이 경로가 없다면
        {
            Debug.LogError("파일 경로가 없습니다.");
            return;
        }

        string spriteFolderPath = "Assets/Datas/Sprites/Character";

        string fullSpriteFolderPath = "Assets/Datas/Sprites/Character/FullSprite";

        for (int i = 0; i < excelSO.Character_IDs.Count; i++) //excelSO의 List를 순회
        {
            var data = excelSO.Character_IDs[i];

            CharacterSO ch = ScriptableObject.CreateInstance<CharacterSO>();

            Sprite charIcon = null;
            Sprite fullSprite = null;

            string[] guids = AssetDatabase.FindAssets(data.ID, new[] { spriteFolderPath });
            string[] spriteGuids = AssetDatabase.FindAssets(data.ID, new[] { fullSpriteFolderPath });
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                charIcon = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }
            else
            {
                Debug.LogWarning($"CharacterConverter : 스프라이트를 찾을 수 없습니다: {data.ID}");
            }

            if(spriteGuids.Length > 0)
            {

                string fullPath = AssetDatabase.GUIDToAssetPath(spriteGuids[0]);
                fullSprite = AssetDatabase.LoadAssetAtPath<Sprite>(fullPath);
            }
            else
            {
                Debug.LogWarning($"CharacterConverter : 풀 스프라이트를 찾을 수 없습니다: {data.ID}");
            }

            string baseStatPath = $"Assets/Scriptables/StatSO/Character/{data.ID}.asset";
            var baseStat = AssetDatabase.LoadAssetAtPath<BaseStatSO>(baseStatPath);

            //이 부분은 더 좋은방법을 찾으면 수정 //###임시
            ch.ExcelInit(
                data.ID,
                data.Name,
                charIcon, //자동 매핑할 예정이면 Resources.Load 할수도? 지금은 수동
                data.Description,
                baseStat,
                data.Class,
                data.Rarity,
                data.LevelUpHP,
                data.LevelUpATK,
                data.AttackType,
                fullSprite
                );

            string skillFolder = "SOData/SkillSO/";

            SkillSO[] allSkills = Resources.LoadAll<SkillSO>(skillFolder);

            ch.ActiveSkill_1 = FindSkill(allSkills, data.ActiveSkill1);
            ch.ActiveSkill_2 = FindSkill(allSkills, data.ActiveSkill2);
            ch.ActiveSkill_3 = FindSkill(allSkills, data.ActiveSkill3);
            ch.PassiveSkill = FindSkill(allSkills, data.PassiveSkill);

            string assetID = string.IsNullOrEmpty(data.ID) ? $"NoID{i}" : data.ID;
            string assetName = string.IsNullOrEmpty(data.Name) ? $"NoName{i}" : data.Name; //data.name이 null이면 true //파일명 변환용
            string assetPath = $"{folderPath}/{assetID}_{assetName}.asset";
            AssetDatabase.CreateAsset(ch, assetPath); //SO 생성
        }

        AssetDatabase.SaveAssets(); //저장
        AssetDatabase.Refresh(); //에디터 반영

        Debug.Log("변환 완료");
    }

    private static SkillSO FindSkill(SkillSO[] allSkills, string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        foreach (var skill in allSkills)
        {
            if (skill.name.Contains(id))
                return skill;
        }

        Debug.LogWarning($"[CharConverter] Skill '{id}'를 찾을 수 없습니다.");
        return null;
    }
}
