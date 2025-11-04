using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[System.Serializable]
public struct PoolInfo
{
    public GameObject prefab;
    public int poolSize;
}

public class PoolingManager : Singleton<PoolingManager>
{
    [SerializeField] List<PoolInfo> poolList;

    private Dictionary<GameObject, IObjectPool<GameObject>> poolDict;

    private Transform poolContainer;

    protected override void Awake()
    {
        base.Awake();

        GameObject container = new GameObject("---- Pool Container ----");
        poolContainer = container.transform;
        if(transform.parent == null ) DontDestroyOnLoad( container );

        poolDict = new();
        foreach (var poolInfo in poolList)
        {
            CreatePool(poolInfo.prefab, poolInfo.poolSize);
        }
    }

    private void CreatePool(GameObject prefab, int poolSize)
    {
        // 해당 프리팹이 이미 풀에 있는지 체크
        if (poolDict.ContainsKey(prefab))
        {
            Debug.LogWarning($"Pool에 {prefab}이 이미 존재합니다.");
            return;
        }

        IObjectPool<GameObject> pool = null;

        // 오브젝트 풀 생성자(생성, 대여, 반납, 파괴, 중복체크, 풀 갯수, 풀 최대갯수)
        pool = new ObjectPool<GameObject>(
              createFunc: () =>
              {
                  var instance = Instantiate(prefab);
                  var poolable = instance.GetComponent<Poolable>();
                  if (poolable == null)
                  {
                      poolable = instance.AddComponent<Poolable>();
                  } 
                  poolable.MyPool = pool;
                  return instance;
              },
              actionOnGet: obj => obj.SetActive(true), //대여
              actionOnRelease: obj =>
              {
                  obj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                  obj.transform.localScale = prefab.transform.localScale;
                  if (obj.TryGetComponent<Rigidbody2D>(out var rb2d))
                  {
                      rb2d.velocity = Vector2.zero;
                      rb2d.angularVelocity = 0f;
                  }
                  obj.transform.SetParent(poolContainer);
                  obj.SetActive(false);
              },
              actionOnDestroy: obj => Destroy(obj), //파괴
              collectionCheck: true, //중복체크
              defaultCapacity: poolSize, //풀 크기 
              maxSize: poolSize * 2 //최대 풀 크기
              );

        // 풀 딕셔너리에 추가
        poolDict[prefab] = pool;

        // 게임 시작 시 풀 미리 생성
        var perloadList = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            perloadList.Add(pool.Get());
        }
        foreach(var obj in perloadList)
        {
            pool.Release(obj);
        }
    }

    /// <summary>
    /// 풀에서 오브젝트 대여
    /// UI는 parent 지정 필요
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public GameObject Get(GameObject prefab, Transform parent = null)
    {
        if (!poolDict.TryGetValue(prefab, out var pool))
        {
            Debug.LogWarning($"풀에 {prefab.name} 이 존재하지 않습니다. 풀을 생성합니다");
            CreatePool(prefab, 10);
            pool = poolDict[prefab];
        }

        var instance = pool.Get();

        if(parent != null)
        {
            instance.transform.SetParent(parent,false);
            instance.transform.localScale = Vector3.one;
        }

        return instance;
    }
    
    public GameObject Get(GameObject prefab,Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!poolDict.TryGetValue(prefab, out var pool))
        {
            Debug.LogWarning($"풀에 {prefab.name} 이 존재하지 않습니다. 풀을 생성합니다");
            CreatePool(prefab, 10);
            pool = poolDict[prefab];
        }

        var instance = pool.Get();
        instance.transform.SetPositionAndRotation(position, rotation);
        if(parent != null)
        {
            instance.transform.SetParent(parent,true);
        }

        return instance;
    }

    public void Release(GameObject prefab)
    {
        if(prefab.TryGetComponent<Poolable>(out var poolable))
        {
            poolable.MyPool.Release(prefab);
        }
        else
        {
            Debug.LogWarning($"{prefab.name}은 풀링 대상이 아닙니다. 삭제합니다");
            Destroy(prefab);
        }
    }
}
