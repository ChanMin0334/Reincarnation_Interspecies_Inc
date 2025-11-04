using UnityEngine;
using UnityEditor;

public class RuneConverter
{
    [MenuItem("Tools/Convert/GameInfoExcel -> RuneSO", false, 80)]
    public static void Convert()
    {
        var excelSO = AssetDatabase.LoadAssetAtPath<GameInfoExcel>("Assets/Datas/GameInfoExcel.asset");
        if (excelSO == null)
        {
            Debug.LogError("Excel파일을 찾을수 없습니다.");
            return;
        }

        string folderPath = "Assets/Resources/SOData/RuneSO"; //변환한 데이터의 위치 경로
        if (!AssetDatabase.IsValidFolder(folderPath)) //이 경로가 없다면
        {
            Debug.LogError("파일 경로가 없습니다.");
            return;
        }

        string gradeSpritePath = "Assets/Datas/Sprites/RuneRarity";
        string statSpritePath = "Assets/Datas/Sprites/RuneType";


        for (int i = 0; i < excelSO.Rune_IDs.Count; i++) //excelSO의 List를 순회
        {
            var data = excelSO.Rune_IDs[i];

            RuneSO rune = ScriptableObject.CreateInstance<RuneSO>();

            string gradePath = $"{gradeSpritePath}/{data.Rarity}.png";
            string statPath = $"{statSpritePath}/{data.EffectType}.png";

            Texture2D gradeTex = AssetDatabase.LoadAssetAtPath<Texture2D>(gradePath);
            Texture2D statTex = AssetDatabase.LoadAssetAtPath<Texture2D>(statPath);

            Sprite finalSprite = null;

            if (gradeTex != null && statTex != null)
            {
                Texture2D mergedTex = CombineTextures(gradeTex, statTex);

                string savePath = $"Assets/Datas/Sprites/RuneImageMerge/{data.ID}_merged.png";
                byte[] bytes = mergedTex.EncodeToPNG();
                System.IO.File.WriteAllBytes(savePath, bytes);
                AssetDatabase.ImportAsset(savePath);

                finalSprite = Sprite.Create(
                    mergedTex,
                    new Rect(0, 0, mergedTex.width, mergedTex.height),
                    new Vector2(0.5f, 0.5f)
                );

                finalSprite = AssetDatabase.LoadAssetAtPath<Sprite>(savePath);
            }
            else
            {
                Debug.LogWarning($"[RuneConverter] {data.ID}에 필요한 스프라이트를 찾지 못했습니다.");
            }

            rune.ExcelInit(
                data.ID,
                data.Name,
                finalSprite, //자동 매핑할 예정이면 Resources.Load 할수도? 지금은 수동
                data.Description,
                data.Rarity,
                data.TargetType,
                data.TargetClass,
                data.TargetID,

                data.EffectType,
                data.EffectValue
                );

            string assetID = string.IsNullOrEmpty(data.ID) ? $"NoID{i}" : data.ID;
            string assetName = string.IsNullOrEmpty(data.Name) ? $"NoName{i}" : data.Name; //data.name이 null이면 true //파일명 변환용
            string assetPath = $"{folderPath}/{assetID}_{assetName}.asset";
            AssetDatabase.CreateAsset(rune, assetPath); //SO 생성
        }

        AssetDatabase.SaveAssets(); //저장
        AssetDatabase.Refresh(); //에디터 반영

        Debug.Log("변환 완료");
    }

    private static Texture2D CombineTextures(Texture2D baseTex, Texture2D overlayTex)
    {
        int width = Mathf.Max(baseTex.width, overlayTex.width);
        int height = Mathf.Max(baseTex.height, overlayTex.height);
        Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false);

        result.SetPixels(baseTex.GetPixels());

        int offsetX = (width - overlayTex.width) / 2;
        int offsetY = (height - overlayTex.height) / 2;

        for (int x = 0; x < overlayTex.width; x++)
        {
            for (int y = 0; y < overlayTex.height; y++)
            {
                Color overlayColor = overlayTex.GetPixel(x, y);
                if (overlayColor.a > 0f)
                {
                    result.SetPixel(x + offsetX, y + offsetY, overlayColor);
                }
            }
        }

        result.Apply();
        return result;
    }
}
