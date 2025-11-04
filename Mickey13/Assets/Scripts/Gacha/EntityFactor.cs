using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFactor
{
    public static Character CharSpawn(EntityData saveData, Transform parent)
    {
        Debug.Log($"[CharSpawn 호출] {saveData.id} 캐릭터 생성 시도됨");

        var so = CharacterManager.Instance.GetCharacterSO(saveData.id);
        var prefab = CharacterManager.Instance.GetPrefab(saveData.id);

        if (so == null || prefab == null)
        {
            Debug.LogError($"Spawn실패 {saveData.id}에 해당하는 SO나 프리팹 없음");

            return null;
        }

        var obj = GameObject.Instantiate(prefab, parent);
        Debug.Log($"[Instantiate 완료] {saveData.id} => GameObject: {obj.name}, InstanceID: {obj.GetInstanceID()}, Parent: {parent.name}");

        var ch = obj.GetComponent<Character>();

        if (ch == null)
            Debug.LogError($"[CharSpawn 오류] {saveData.id} 프리팹에서 Character 컴포넌트를 찾지 못함!");
        else
            Debug.Log($"[CharSpawn 결과] {saveData.id} => Character 컴포넌트 ID: {ch.GetInstanceID()}");


        saveData.ApplyToCharacter(ch);

        CharacterManager.Instance.BattleCharacterDict[saveData.id] = ch;

        if (IsZero(saveData.RunProgress))
        {
            ch.Data.Init(so.BaseStat.Value);
        }

        ch.Definition = so;
        ch.Init(saveData);

        ch.ActiveSpawnProtection(3.0f); // 스폰시 3초 무적

        return ch;
    }

    public static Enemy EnemySpawn(EntityData saveData, Vector3 Position)
    {
        var so = EnemyManager.Instance.GetEnemySO(saveData.id);
        var prefab = EnemyManager.Instance.GetPrefab(saveData.id);
        if (so == null || prefab == null)
        {
            Debug.LogError($"Spawn실패 {saveData.id}에 해당하는 SO나 프리팹 없음");

            return null;
        }

        var obj = PoolingManager.Instance.Get(prefab);
        obj.transform.position = Position;
        obj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        var en = obj.GetComponent<Enemy>();

        en.Data.Init(so.BaseStat.Value);

        en.Data.SetLevel(User.Instance.CurAchievementKm / 10);

        en.Data.SetHP(en.Data.MaxHP);

        return en;
    }

    //###임시
    public static bool IsZero(StatModel stat)
    {
        return stat.HP.value == 0 &&
           stat.Atk.value == 0 &&
           stat.AtkCooldown == 0 &&
           stat.SkillCooldown == 0 &&
           stat.AtkRange == 0 &&
           stat.CritChance == 0 &&
           stat.AtkArea == 0 &&
           stat.MoveSpeed == 0 &&
           stat.CritMult == 0;
    }
    public static Boss BossSpawn(EntityData saveData, Vector3 position)
    {
        var so = EnemyManager.Instance.GetBossSO(saveData.id);
        var prefab = EnemyManager.Instance.GetBossPrefab(saveData.id);
        if (so == null || prefab == null)
        {
            Debug.LogError($"Spawn실패 {saveData.id}에 해당하는 SO나 프리팹 없음");

            return null;
        }

        var obj = PoolingManager.Instance.Get(prefab,position,Quaternion.identity);
        // obj.transform.position = position;
        // obj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        var en = obj.GetComponent<Boss>();

        en.Data.Init(so.BaseStat.Value);

        en.Data.SetLevel(User.Instance.CurAchievementKm / 2);

        en.Data.SetHP(en.Data.MaxHP);

        return en;
    }

    public static Boss BossSpawn(EntityData saveData, Transform parent)
    {
        return BossSpawn(saveData, parent.position);
    }

    public static Enemy MiddleBossSpawn(EntityData saveData, Vector3 position)
    {
        var so = EnemyManager.Instance.GetMiddleBossSO(saveData.id);
        var prefab = EnemyManager.Instance.GetMiddleBossPrefab(saveData.id);
        if (so == null || prefab == null)
        {
            Debug.LogError($"Spawn실패 {saveData.id}에 해당하는 SO나 프리팹 없음");

            return null;
        }

        var obj = PoolingManager.Instance.Get(prefab,position,Quaternion.identity);
        // obj.transform.position = position;
        // obj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        var en = obj.GetComponent<Enemy>();

        en.Data.Init(so.BaseStat.Value);

        en.Data.SetLevel(User.Instance.CurAchievementKm / 2);

        en.Data.SetHP(en.Data.MaxHP);

        return en;
    }

    public static Enemy MiddleBossSpawn(EntityData saveData, Transform parent)
    {
        return MiddleBossSpawn(saveData, parent.position);
    }
}
