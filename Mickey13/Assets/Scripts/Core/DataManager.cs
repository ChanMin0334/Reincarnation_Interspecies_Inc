using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //인스펙터에 확인용
public class GameDataEntry
{
    public string id;
    public GameData data;
}
public class DataManager : Singleton<DataManager>
{
    [SerializeField] private List<GameDataEntry> testDebug = new(); //추후 삭제되어야할 것 디버깅용
    private Dictionary<string, GameData> dataDict = new();

    protected override void Awake()
    {
        base.Awake();
        LoadGameData();
    }
    private void LoadGameData()
    {
        GameData[] allData = Resources.LoadAll<GameData>("SOData");

        for (int i = 0; i < allData.Length; i++)
        {
            if (!dataDict.ContainsKey(allData[i].ID))
            {
                dataDict.Add(allData[i].ID, allData[i]);

                testDebug.Add(new GameDataEntry //추후 삭제되어야할 것 디버깅용
                {
                    id = allData[i].ID,
                    data = allData[i]
                });
            }

            else
            {
                Debug.LogError($"중복된 ID 발견 : {allData[i].ID}");
            }
        }

        Debug.Log($"{dataDict.Count}개 SO 데이터 로드");
    }

    public T GetData<T>(string id) where T : GameData
    {
        if(dataDict.TryGetValue(id, out GameData data))
        {
            return data as T;
        }
        Debug.LogError($"ID {id}에 해당하는 데이터 없음");
        return null;
    }

    /// <summary>
    /// T 타입 데이터 전부 가져오는 함수
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public List<T> GetTypeAllData<T>() where T : GameData
    {
        List<T> result = new List<T>();

        foreach(var k in dataDict)
        {
            if(k.Value is T tData)
            {
                result.Add(tData);
            }
        }

        return result;
    }
}
