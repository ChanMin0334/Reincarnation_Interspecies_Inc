using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
//using static UnityEditor.Progress;

public class SaveManager : Singleton<SaveManager>
{
    protected override void Awake()
    {
        base.Awake();
        // 자동 저장 코루틴 시작
        StartCoroutine(AutoSaveRoutine());

        if (EventManager.Instance == null)
        {
            Debug.LogError("EventManager 인스턴스를 찾을 수 없습니다. SaveManager가 제대로 작동하지 않을 수 있습니다.");
            StartCoroutine(RetrySettingEvent());
            return;
        }

        SetEvent();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            SaveUser();
            Debug.Log("저장완료");
        }
        if (Input.GetKeyDown
        (KeyCode.F9))
        {
            LoadUser();
            Debug.Log($"로드 완료");
        }
    }
#endif

    public void SaveUser()
    {
        var saveData = User.Instance.ToSaveData();
        JsonSaveSystem.Save(saveData);
        string rawJson = JsonSaveSystem.SerializeToJson(saveData, false);
        CloudSaveBridge.Notify(rawJson);
        Debug.Log("유저 데이터 저장 완료");
    }

    public void LoadUser()
    {
        Debug.Log("LoadUser 함수 실행");
        var data = JsonSaveSystem.Load();
        User.Instance.LoadFromSaveData(data);
        User.Instance.GetIdleReward(data);
        Debug.Log("유저 데이터 로드 완료");
        //나중에 추가해주세요잉
    }

    public void SetEvent()
    {
        // User의 변경시 저장하는 이벤트 리스너 등록
        EventManager.Instance.StartListening(EventType.EndReincarnate, SaveUser);
        EventManager.Instance.StartListening(EventType.AddRuneToInventory, SaveUser);
        EventManager.Instance.StartListening(EventType.AddArtifactToInventory, SaveUser);
        EventManager.Instance.StartListening(EventType.AddCharacterToInventory, SaveUser);
        EventManager.Instance.StartListening(EventType.UpdateCharacterToInventory, SaveUser);
        //1키로 당 저장
        //EventManager.Instance.StartListening(EventType.UpdateRecord, SaveUser);
    }

    private IEnumerator RetrySettingEvent()
    {
        while (EventManager.Instance == null)
        {
            yield return null; // 다음 프레임까지 대기
        }
        SetEvent();
    }

    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(15f);
            Debug.Log("자동 저장 시작");
            SaveUser();
        }
    }

    public void RemoveInvalidData<T, TSO>(List<T> saveList) where T : IInventoryData where TSO : GameData
    {
        if (saveList == null) return;

        if (DataManager.Instance == null) return;

        saveList.RemoveAll(item =>
        {
            if (item == null) return true;

            bool invalid = string.IsNullOrEmpty(item.ID) || DataManager.Instance.GetData<TSO>(item.ID) == null;

            if (invalid)
            {
                Debug.Log($"SaveManager : 기존에 인벤토리에 있던 ID {item?.ID}는 현재 버전에서 존재하지 않습니다. 삭제합니다.");
            }

            return invalid;
        });
    }

    private static class CloudSaveBridge
    {
    private static Type cachedType;
    private static MethodInfo reportMethod;
        private static readonly object[] invokeArgs = new object[1];

        public static void Notify(string json)
        {
            if (string.IsNullOrEmpty(json))
                return;

            if (reportMethod == null)
            {
                cachedType = typeof(SaveManager).Assembly.GetType("SaveSyncManager")
                             ?? Type.GetType("SaveSyncManager, Assembly-CSharp");
                if (cachedType == null)
                    return;

                reportMethod = cachedType.GetMethod("ReportLocalSave", BindingFlags.Public | BindingFlags.Static);
                if (reportMethod == null)
                    return;
            }

            try
            {
                invokeArgs[0] = json;
                reportMethod.Invoke(null, invokeArgs);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveManager] 클라우드 저장 통지 실패: {ex.Message}");
            }
            finally
            {
                invokeArgs[0] = null;
            }
        }
    }
}
