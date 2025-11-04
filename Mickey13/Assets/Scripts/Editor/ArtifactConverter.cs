using UnityEditor;
using UnityEngine;

public class ArtifactConverter
{
    [MenuItem("Tools/Convert/GameInfoExcel -> ArtifactSO", false, 90)]
    public static void Convert()
    {
        var excelSO = AssetDatabase.LoadAssetAtPath<GameInfoExcel>("Assets/Datas/GameInfoExcel.asset");
        if (excelSO == null)
        {
            Debug.LogError("Excel파일을 찾을수 없습니다.");
            return;
        }

        string folderPath = "Assets/Resources/SOData/ArtifactSO"; //변환한 데이터의 위치 경로
        if (!AssetDatabase.IsValidFolder(folderPath)) //이 경로가 없다면
        {
            Debug.LogError("파일 경로가 없습니다.");
            return;
        }

        for (int i = 0; i < excelSO.Artifact_IDs.Count; i++) //excelSO의 List를 순회
        {
            var data = excelSO.Artifact_IDs[i];

            ArtifactSO artifact = ScriptableObject.CreateInstance<ArtifactSO>();

            string baseStatPath = $"Assets/Scriptables/StatSO/Rune/{data.ID}.asset";
            var baseStat = AssetDatabase.LoadAssetAtPath<BaseStatSO>(baseStatPath);

            string spriteFolderPath = "Assets/Datas/Sprites/Artifact";

            Sprite artifactIcon = null;
            string[] guids = AssetDatabase.FindAssets(data.ID, new[] { spriteFolderPath });

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                artifactIcon = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }
            else
            {
                Debug.LogWarning($"[SkillConverter] 스프라이트를 찾을 수 없습니다: {data.ID}");
            }

            artifact.ExcelInit(
                data.ID,
                data.Name,
                artifactIcon, //자동 매핑할 예정이면 Resources.Load 할수도? 지금은 수동
                data.Description,
                baseStat,
                data.Rarity,
                data.TargetType,
                data.TargetClass,
                data.EffectName,
                data.Value,
                data.StatType
                );

            string assetID = string.IsNullOrEmpty(data.ID) ? $"NoID{i}" : data.ID;
            string assetName = string.IsNullOrEmpty(data.Name) ? $"NoName{i}" : data.Name; //data.name이 null이면 true //파일명 변환용
            string assetPath = $"{folderPath}/{assetID}_{assetName}.asset";
            AssetDatabase.CreateAsset(artifact, assetPath); //SO 생성
        }

        AssetDatabase.SaveAssets(); //저장
        AssetDatabase.Refresh(); //에디터 반영

        Debug.Log("변환 완료");
    }
}
