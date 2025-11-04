using UnityEngine;
using UnityEditor;
using System.IO;

public class QuestConverter
{
    [MenuItem("Tools/Convert/GameInfoExcel -> QuestSO", false, 70)]
    public static void Convert()
    {
        var excelSO = AssetDatabase.LoadAssetAtPath<GameInfoExcel>("Assets/Datas/GameInfoExcel.asset"); //BaseStatExcelSO 를 가져온다.
        if (excelSO == null)
        {
            Debug.LogError("Excel파일을 찾을수 없습니다.");
            return;
        }

        string folderPath = "Assets/Resources/SOData/QuestSO"; //변환한 데이터의 위치 경로
        if (!AssetDatabase.IsValidFolder(folderPath)) //이 경로가 없다면
        {
            Debug.LogError("파일 경로가 없습니다.");
            return;
        }

        string spriteFolderPath = "Assets/Datas/Sprites/Quest";

        for (int i = 0; i < excelSO.QuestInfo.Count; i++) //excelSO의 List를 순회
        {
            var data = excelSO.QuestInfo[i];

            QuestSO quest = ScriptableObject.CreateInstance<QuestSO>(); //BaseStatSO로 생성

            Sprite questIcon = null;
            string[] guids = AssetDatabase.FindAssets(data.ID, new[] { spriteFolderPath });
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                questIcon = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }
            else
            {
                Debug.LogWarning($"[QuestConverter] 스프라이트를 찾을 수 없습니다: {data.ID}");
            }

            //이 부분은 더 좋은방법을 찾으면 수정
            quest.Init(
                data.ID,
                data.Name,
                data.requirement,
                data.duration,
                data.upgradeCost,
                data.reward,
                data.upgradeMult,
                data.rewardMult,
                questIcon
                );

            string assetID = string.IsNullOrEmpty(data.ID) ? $"NoID{i}" : data.ID;
            string assetName = string.IsNullOrEmpty(data.Name) ? $"NoName{i}" : data.Name; //data.name이 null이면 true //파일명 변환용
            string assetPath = $"{folderPath}/{assetID}_{assetName}.asset";
            AssetDatabase.CreateAsset(quest, assetPath); //SO 생성
        }

        AssetDatabase.SaveAssets(); //저장
        AssetDatabase.Refresh(); //에디터 반영

        Debug.Log("변환 완료");
    }
}
