using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Tilemaps;
public class EntityPrefabCreator
{
    [MenuItem("Tools/Convert/CreateEntityPrefabs", false, 60)]
    public static void CreateEntityPrefabs()
    {
        ConvertSOFolder<CharacterSO, Character, BaseStatSO>("Assets/Resources/SOData/CharacterSO", "Assets/Resources/Prefab/Character");
        ConvertEnemySOFolder<EnemySO, Enemy, BaseStatSO>("Assets/Resources/SOData/EnemySO/Normal", "Assets/Resources/Prefab/Enemy");
        ConvertEnemySOFolder<EnemySO, Enemy, BaseStatSO>("Assets/Resources/SOData/EnemySO/MiddleBoss", "Assets/Resources/Prefab/EnemyMiddle");
        ConvertEnemySOFolder<EnemySO, Boss, BaseStatSO>("Assets/Resources/SOData/EnemySO/Boss", "Assets/Resources/Prefab/EnemyBoss");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void ConvertSOFolder<TEntitySO, TEntity, TStatSO>(string soFolderPath, string prefabFolderPath)
    where TEntitySO : EntitySO
    where TEntity : Entity
    where TStatSO : BaseStatSO
    {
        if (!AssetDatabase.IsValidFolder(soFolderPath))
        {
            Debug.LogError($"{soFolderPath} 폴더가 없습니다.");
            return;
        }

        if (!AssetDatabase.IsValidFolder(prefabFolderPath))
        {
            Debug.LogError($"{prefabFolderPath} 폴더가 없습니다.");
            return;
        }

        string[] guids = AssetDatabase.FindAssets($"t:{typeof(TEntitySO).Name}", new[] { soFolderPath });
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            var entitySo = AssetDatabase.LoadAssetAtPath<TEntitySO>(path);
            if (entitySo == null) continue;

            var tempPrefabFolderPath = prefabFolderPath;

            // 1) 외형 ID/경로 가져오기
            string appearanceId = null;
            GameObject appearancePrefab = null;

            // CharacterSO에 AppearanceId/AppearancePrefab 필드가 있다고 가정
            var appearanceIdProp = entitySo.GetType().GetProperty("AppearanceId");
            if (appearanceIdProp != null)
                appearanceId = appearanceIdProp.GetValue(entitySo) as string;
            if (string.IsNullOrEmpty(appearanceId))
                appearanceId = entitySo.ID.ToString();

            var appearancePrefabProp = entitySo.GetType().GetProperty("AppearancePrefab");
            if (appearancePrefabProp != null)
                appearancePrefab = appearancePrefabProp.GetValue(entitySo) as GameObject;

            // 2) 외형 프리팹 로드 (없으면 규칙 기반 Resources 경로)
            if (appearancePrefab == null)
            {
                string resPath = $"CharacterResources/{appearanceId}/{appearanceId}";
                appearancePrefab = Resources.Load<GameObject>(resPath);
            }

            // 3) 캐릭터일 때: 외형 프리팹 인스턴스에서 시작
            GameObject root;
            if (appearancePrefab != null)
            {
                root = (GameObject)PrefabUtility.InstantiatePrefab(appearancePrefab);
            }
            else
            {
                GameObject basePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Entity/EntityPrefab.prefab");
                if (basePrefab == null)
                {
                    Debug.LogError("기본 프리팹을 찾을 수 없습니다: " + "Assets/Prefabs/Entity");
                    return;
                }
                root = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);
            }

            // 5) 엔티티 컴포넌트/정의/레이어
            var entity = root.GetComponent<TEntity>() ?? root.AddComponent<TEntity>();
            entity.GetType().GetField("Definition").SetValue(entity, entitySo);

            // 6) SO 기반 추가 세팅
            entity.SetPrefabsBySO(entitySo);

            entity.gameObject.AddComponent<EntityUI>();

            var pivot = new GameObject("Pivot");
            pivot.transform.position = new Vector3(0f, 0.25f, 0f);

            GameObject.Instantiate(pivot, entity.gameObject.transform);

            // 7) 결과 저장 경로
            string prefabPath = Path.Combine(tempPrefabFolderPath, $"{entitySo.ID}_{entitySo.Name}.prefab");

            // 8) 저장
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            GameObject.DestroyImmediate(root);
            GameObject.DestroyImmediate(pivot);

            Debug.Log($"{prefabPath} 생성 완료");
        }
    }

    public static void ConvertEnemySOFolder<TEntitySO, TEntity, TStatSO>(string soFolderPath, string prefabFolderPath)
    where TEntitySO : EnemySO
    where TEntity : Entity
    where TStatSO : BaseStatSO
    {
        if (!AssetDatabase.IsValidFolder(soFolderPath))
        {
            Debug.LogError($"{soFolderPath} 폴더가 없습니다.");
            return;
        }

        if (!AssetDatabase.IsValidFolder(prefabFolderPath))
        {
            Debug.LogError($"{prefabFolderPath} 폴더가 없습니다.");
            return;
        }

        string[] guids = AssetDatabase.FindAssets($"t:{typeof(TEntitySO).Name}", new[] { soFolderPath });
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            var entitySo = AssetDatabase.LoadAssetAtPath<TEntitySO>(path);
            if (entitySo == null) continue;

            var tempPrefabFolderPath = prefabFolderPath;

            // 1) 외형 ID/경로 가져오기
            string appearanceId = null;
            GameObject appearancePrefab = null;

            // CharacterSO에 AppearanceId/AppearancePrefab 필드가 있다고 가정
            var appearanceIdProp = entitySo.GetType().GetProperty("AppearanceId");
            if (appearanceIdProp != null)
                appearanceId = appearanceIdProp.GetValue(entitySo) as string;
            if (string.IsNullOrEmpty(appearanceId))
                appearanceId = entitySo.ID.ToString();

            var appearancePrefabProp = entitySo.GetType().GetProperty("AppearancePrefab");
            if (appearancePrefabProp != null)
                appearancePrefab = appearancePrefabProp.GetValue(entitySo) as GameObject;

            // 2) 외형 프리팹 로드 (없으면 규칙 기반 Resources 경로)
            if (appearancePrefab == null)
            {
                string resPath = $"CharacterResources/{appearanceId}/{appearanceId}";
                appearancePrefab = Resources.Load<GameObject>(resPath);
            }

            // 3) 캐릭터일 때: 외형 프리팹 인스턴스에서 시작
            GameObject root;
            if (appearancePrefab != null)
            {
                root = (GameObject)PrefabUtility.InstantiatePrefab(appearancePrefab);
            }
            else
            {
                GameObject basePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Entity/EntityPrefab.prefab");
                if (basePrefab == null)
                {
                    Debug.LogError("기본 프리팹을 찾을 수 없습니다: " + "Assets/Prefabs/Entity");
                    return;
                }
                root = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);
            }

            // 4) SpriteRenderer 교체는 적만, 캐릭터는 스킵

            var sr = root.GetComponent<SpriteRenderer>();
            if (sr) sr.sprite = entitySo.Sprite;

            // 5) 엔티티 컴포넌트/정의/레이어
            var entity = root.GetComponent<TEntity>() ?? root.AddComponent<TEntity>();
            entity.GetType().GetField("Definition").SetValue(entity, entitySo);

            // 몬스터 프리팹 생성시 레이어 설정

            root.layer = LayerMask.NameToLayer("Enemy");

            // 추가: Collider2D 설정
            root.AddComponent<BoxCollider2D>().offset = new Vector2(0, 0.5f);
            root.GetComponent<BoxCollider2D>().size = new Vector2(0.5f, 2f);

            var pivot = new GameObject("Pivot");

            var scaleFactor = 1f;

            // 적 크기 및 타입에 따른 스케일 및 피벗 조정
            if (entitySo.EnemySizeEnum == EnemySizeEnum.Small)
            {
                scaleFactor = 0.8f;
            }
            else if (entitySo.EnemySizeEnum == EnemySizeEnum.Medium)
            {
                scaleFactor = 1f;
            }
            else if (entitySo.EnemySizeEnum == EnemySizeEnum.Large)
            {
                scaleFactor = 1.3f;
            }

            if(entitySo.EnemyType == EnemyTypeEnum.Boss)
            {
                scaleFactor *= 2f;
                pivot.transform.position = new Vector3(0f, 0.5f, 0f);
            }
            else if (entitySo.EnemyType == EnemyTypeEnum.MiddleBoss)
            {
                scaleFactor *= 1.5f;
                pivot.transform.position = new Vector3(0f, 0.4f, 0f);
            }
            else
            {
                pivot.transform.position = new Vector3(0f, 0.25f, 0f);
            }

            root.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);

            GameObject.Instantiate(pivot, entity.gameObject.transform);


            var lootdropper = root.AddComponent<LootDropper>();

            // 6) SO 기반 추가 세팅
            entity.SetPrefabsBySO(entitySo);

            entity.gameObject.AddComponent<EntityUI>();

            // 7) 결과 저장 경로
            string prefabPath = Path.Combine(tempPrefabFolderPath, $"{entitySo.ID}_{entitySo.Name}.prefab");

            // 8) 저장
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            GameObject.DestroyImmediate(root);
            GameObject.DestroyImmediate(pivot);

            Debug.Log($"{prefabPath} 생성 완료");
        }
    }
}

