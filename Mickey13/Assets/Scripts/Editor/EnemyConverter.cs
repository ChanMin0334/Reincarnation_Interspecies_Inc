using UnityEngine;
using UnityEditor;

public class EnemyConverter
{
    [MenuItem("Tools/Convert/GameInfoExcel -> EnemySO", false, 50)]
    public static void Convert()
    {
        var excelSO = AssetDatabase.LoadAssetAtPath<GameInfoExcel>("Assets/Datas/GameInfoExcel.asset");
        if (excelSO == null)
        {
            Debug.LogError("Excel파일을 찾을수 없습니다.");
            return;
        }

        string folderPath = "Assets/Resources/SOData/EnemySO"; //변환한 데이터의 위치 경로
        if (!AssetDatabase.IsValidFolder(folderPath)) //이 경로가 없다면
        {
            Debug.LogError("파일 경로가 없습니다.");
            return;
        }

        for (int i = 0; i < excelSO.Enemy_IDs.Count; i++) //excelSO의 List를 순회
        {
            var data = excelSO.Enemy_IDs[i];

            EnemySO enemy = ScriptableObject.CreateInstance<EnemySO>();

            string baseStatPath = $"Assets/Scriptables/StatSO/Enemy/{data.ID}.asset";
            var baseStat = AssetDatabase.LoadAssetAtPath<BaseStatSO>(baseStatPath);

            DropTable dropTable = null;

            //todo: 적 유형에 따른 드롭테이블 분기 처리
            if (data.EnemyRankID == EnemyTypeEnum.Normal)
            {
                dropTable = AssetDatabase.LoadAssetAtPath<DropTable>($"Assets/Resources/SOData/DropTableSO/NormalEnemy.asset");
            }
            else if(data.EnemyRankID == EnemyTypeEnum.MiddleBoss)
            {
                dropTable = AssetDatabase.LoadAssetAtPath<DropTable>($"Assets/Resources/SOData/DropTableSO/MiddleBossEnemy.asset");
            }
            else if (data.EnemyRankID == EnemyTypeEnum.Boss)
            {
                dropTable = AssetDatabase.LoadAssetAtPath<DropTable>($"Assets/Resources/SOData/DropTableSO/BossEnemy.asset");
            }

            //이 부분은 더 좋은방법을 찾으면 수정
            enemy.ExcelInit(
                data.ID,
                data.Name,
                null, //자동 매핑할 예정이면 Resources.Load 할수도? 지금은 수동
                data.Description,
                baseStat,
                data.EnemyRankID,
                data.EnemySizeID,
                1.266f, //임시
                1.266f, //임시
                dropTable,
                data.AttackType
                );

            string assetID = string.IsNullOrEmpty(data.ID) ? $"NoID{i}" : data.ID;
            string assetName = string.IsNullOrEmpty(data.Name) ? $"NoName{i}" : data.Name; //data.name이 null이면 true //파일명 변환용
            string assetPath = $"{folderPath}/{assetID}_{assetName}.asset";

            switch (enemy.EnemyType)
            {
                case EnemyTypeEnum.Normal:
                    assetPath = $"{folderPath}/Normal/{assetID}_{assetName}.asset";
                    break;
                case EnemyTypeEnum.MiddleBoss:
                    assetPath = $"{folderPath}/MiddleBoss/{assetID}_{assetName}.asset";
                    break;
                case EnemyTypeEnum.Boss:
                    assetPath = $"{folderPath}/Boss/{assetID}_{assetName}.asset";
                    break;
            }

            AssetDatabase.CreateAsset(enemy, assetPath); //SO 생성
        }

        AssetDatabase.SaveAssets(); //저장
        AssetDatabase.Refresh(); //에디터 반영

        Debug.Log("변환 완료");
    }
}
