using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [Header("Singleton Settings")]
    [SerializeField]
    [Tooltip("체크하면 DontDestroyOnLoad가 호출됩니다. 부모 매니저에만 체크하세요.")]
    private bool isPersistent = true;
    
    // 좀비 객체 경고 제거용
    private static bool isApplicationQuitting = false;
    
    private static T instance;
    public static T Instance
    {
        get
        {
            // 게임 종료 확인
            if (isApplicationQuitting)
            {
                return null;
            }
            
            if(instance == null)
            {
                instance = FindObjectOfType<T>();

                if(instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    public static bool HasInstance => instance != null;

    public static T InstanceIfInitialized => instance;
    
    protected virtual void Awake()
    {
        if(instance == null)
        {
            instance = this as T;
            if (isPersistent)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    protected virtual void OnApplicationQuit()
    {
        isApplicationQuitting = true;
    }
}
