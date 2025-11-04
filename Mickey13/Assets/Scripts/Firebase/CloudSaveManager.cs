using System;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;

/// <summary>
/// Firestore 클라우드 세이브 관리자
/// 세이브 데이터 업로드/다운로드 처리
/// </summary>
public class CloudSaveManager : Singleton<CloudSaveManager>
{
    private FirebaseFirestore db;
    private const string COLLECTION_NAME = "configs";

    public bool IsInitialized { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        InitializeFirestore();
    }

    private void InitializeFirestore()
    {
        try
        {
            db = FirebaseFirestore.DefaultInstance;
            IsInitialized = true;
            Debug.Log("[Cloud Save] Firestore 초기화 성공");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Cloud Save] Firestore 초기화 실패: {ex.Message}");
            IsInitialized = false;
        }
    }

    /// <summary>
    /// 클라우드에 세이브 데이터 저장
    /// </summary>
    public async Task<bool> SaveToCloud(string userId, string saveJson, string checksum)
    {
        if (!IsInitialized)
        {
            Debug.LogError("[Cloud Save] Firestore 초기화되지 않음");
            return false;
        }

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("[Cloud Save] userId가 null입니다");
            return false;
        }

        try
        {
            var saveData = new
            {
                blob = saveJson,
                checksum = checksum,
                timestamp = FieldValue.ServerTimestamp,
                version = Application.version
            };

            await db.Collection(COLLECTION_NAME).Document(userId).SetAsync(saveData);
            Debug.Log($"[Cloud Save] 클라우드 저장 성공 (userId: {userId})");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Cloud Save] 클라우드 저장 실패: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 클라우드에서 세이브 데이터 로드
    /// </summary>
    public async Task<(bool success, string saveJson, string checksum)> LoadFromCloud(string userId)
    {
        if (!IsInitialized)
        {
            Debug.LogError("[Cloud Save] Firestore 초기화되지 않음");
            return (false, null, null);
        }

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("[Cloud Save] userId가 null입니다");
            return (false, null, null);
        }

        try
        {
            var docRef = db.Collection(COLLECTION_NAME).Document(userId);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Debug.Log($"[Cloud Save] 클라우드 데이터 없음 (userId: {userId})");
                return (false, null, null);
            }

            string blob = snapshot.GetValue<string>("blob");
            string checksum = snapshot.GetValue<string>("checksum");

            Debug.Log($"[Cloud Save] 클라우드 로드 성공 (userId: {userId})");
            return (true, blob, checksum);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Cloud Save] 클라우드 로드 실패: {ex.Message}");
            return (false, null, null);
        }
    }

    /// <summary>
    /// 클라우드 세이브 데이터 삭제
    /// </summary>
    public async Task<bool> DeleteCloudSave(string userId)
    {
        if (!IsInitialized)
        {
            Debug.LogError("[Cloud Save] Firestore 초기화되지 않음");
            return false;
        }

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("[Cloud Save] userId가 null입니다");
            return false;
        }

        try
        {
            await db.Collection(COLLECTION_NAME).Document(userId).DeleteAsync();
            Debug.Log($"[Cloud Save] 클라우드 삭제 성공 (userId: {userId})");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Cloud Save] 클라우드 삭제 실패: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 클라우드 세이브 존재 여부 확인
    /// </summary>
    public async Task<bool> CloudSaveExists(string userId)
    {
        if (!IsInitialized || string.IsNullOrEmpty(userId))
            return false;

        try
        {
            var docRef = db.Collection(COLLECTION_NAME).Document(userId);
            var snapshot = await docRef.GetSnapshotAsync();
            return snapshot.Exists;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Cloud Save] 존재 여부 확인 실패: {ex.Message}");
            return false;
        }
    }
}
