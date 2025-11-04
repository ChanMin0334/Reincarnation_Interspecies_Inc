using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 로컬/클라우드 세이브 동기화 관리자
/// 로컬 5회 저장 시 1회 클라우드 동기화
/// </summary>
public class SaveSyncManager : Singleton<SaveSyncManager>
{
    [SerializeField] private bool firebaseEnabled = true;
    [SerializeField] private int localSaveCountBeforeSync = 5;

    private int localSaveCount = 0;
    private bool isSyncing = false;
    private string latestLocalSave;

    public bool FirebaseEnabled => firebaseEnabled;
    public string LatestCloudSave { get; private set; }

    public static void ReportLocalSave(string saveJson)
    {
        if (string.IsNullOrEmpty(saveJson))
            return;

        Instance.SaveLocal(saveJson);
    }

    /// <summary>
    /// 로컬 저장(유저 데이터) 후 자동 클라우드 동기화 카운트
    /// </summary>
    /// <param name="saveJson">로컬에 방금 저장된 전체 세이브 JSON</param>
    public void SaveLocal(string saveJson)
    {
        if (string.IsNullOrEmpty(saveJson))
        {
            Debug.LogWarning("[SaveSync] 저장할 데이터가 비어 있습니다");
            return;
        }

        latestLocalSave = saveJson;
        localSaveCount++;

        Debug.Log($"[SaveSync] 로컬 저장 카운트 ({localSaveCount}/{localSaveCountBeforeSync})");

        if (firebaseEnabled && localSaveCount >= localSaveCountBeforeSync)
        {
            _ = SyncToCloud();
        }
    }

    /// <summary>
    /// 클라우드 동기화 (비동기)
    /// </summary>
    public async Task<bool> SyncToCloud()
    {
        if (!firebaseEnabled)
        {
            Debug.Log("[SaveSync] Firebase 비활성화됨");
            return false;
        }

        if (isSyncing)
        {
            Debug.Log("[SaveSync] 이미 동기화 중");
            return false;
        }

        if (!FirebaseAuthManager.Instance.IsSignedIn)
        {
            Debug.LogWarning("[SaveSync] 로그인되지 않음");
            return false;
        }

        if (string.IsNullOrEmpty(latestLocalSave))
        {
            Debug.LogWarning("[SaveSync] 업로드할 로컬 데이터가 없습니다. SaveLocal 호출이 필요합니다");
            return false;
        }

        isSyncing = true;

        try
        {
            string userId = FirebaseAuthManager.Instance.UserId;
            if (!TryGetRawJson(latestLocalSave, out string rawJson))
            {
                Debug.LogError("[SaveSync] 로컬 세이브를 복호화할 수 없습니다");
                return false;
            }

            if (!IntegrityChecker.Instance.ValidateGameLogic(rawJson))
            {
                Debug.LogError("[SaveSync] 무결성 검증 실패로 업로드를 중단합니다");
                return false;
            }

            string checksum = IntegrityChecker.Instance.GenerateChecksum(rawJson);
            string encryptedPayload = JsonSaveSystem.EncryptToString(rawJson);

            bool success = await CloudSaveManager.Instance.SaveToCloud(userId, encryptedPayload, checksum);

            if (success)
            {
                localSaveCount = 0;
                LatestCloudSave = rawJson;
                Debug.Log("[SaveSync] 클라우드 동기화 성공");
            }

            return success;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SaveSync] 클라우드 동기화 실패: {ex.Message}");
            return false;
        }
        finally
        {
            isSyncing = false;
        }
    }

    /// <summary>
    /// 클라우드에서 로드
    /// </summary>
    public async Task<bool> LoadFromCloud()
    {
        if (!firebaseEnabled)
        {
            Debug.Log("[SaveSync] Firebase 비활성화됨");
            return false;
        }

        if (!FirebaseAuthManager.Instance.IsSignedIn)
        {
            Debug.LogWarning("[SaveSync] 로그인되지 않음");
            return false;
        }

        try
        {
            string userId = FirebaseAuthManager.Instance.UserId;
            var (success, encryptedBlob, checksum) = await CloudSaveManager.Instance.LoadFromCloud(userId);

            if (!success)
            {
                Debug.Log("[SaveSync] 클라우드 데이터 없음");
                return false;
            }

            if (!TryGetRawJson(encryptedBlob, out string rawJson))
            {
                Debug.LogError("[SaveSync] 클라우드 데이터를 복호화할 수 없습니다");
                return false;
            }

            // 무결성 검증
            if (!IntegrityChecker.Instance.VerifyChecksum(rawJson, checksum))
            {
                Debug.LogError("[SaveSync] 체크섬 불일치 - 데이터 손상 의심");
                return false;
            }

            // 게임 로직 무결성 검증
            if (!IntegrityChecker.Instance.ValidateGameLogic(rawJson))
            {
                Debug.LogError("[SaveSync] 게임 로직 무결성 검증 실패");
                return false;
            }

            if (!TryApplyCloudSave(rawJson))
            {
                Debug.LogError("[SaveSync] 클라우드 데이터를 로컬에 반영하지 못했습니다.");
                return false;
            }

            LatestCloudSave = rawJson;
            localSaveCount = 0;

            Debug.Log("[SaveSync] 클라우드 로드 성공");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SaveSync] 클라우드 로드 실패: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 클라우드 세이브 초기화 (게임 시작 시 호출)
    /// </summary>
    public async Task<bool> InitializeCloudSave()
    {
        if (!firebaseEnabled)
            return false;

        if (!FirebaseAuthManager.Instance.IsSignedIn)
        {
            bool signedIn = await FirebaseAuthManager.Instance.SignInAnonymously();
            if (!signedIn)
            {
                Debug.LogError("[SaveSync] 익명 로그인 실패");
                return false;
            }
        }

        // 클라우드 세이브 존재 확인
        string userId = FirebaseAuthManager.Instance.UserId;
        bool cloudExists = await CloudSaveManager.Instance.CloudSaveExists(userId);

        if (cloudExists)
        {
            // 클라우드 데이터가 있으면 로드
            return await LoadFromCloud();
        }
        else
        {
            Debug.Log("[SaveSync] 클라우드 데이터 없음. 로컬 데이터를 기반으로 초기화합니다.");
            WarmLocalCacheFromDisk();

            if (firebaseEnabled && !string.IsNullOrEmpty(latestLocalSave))
            {
                bool uploaded = await SyncToCloud();
                if (!uploaded)
                {
                    Debug.LogWarning("[SaveSync] 초기 클라우드 업로드에 실패했습니다. 이후 저장 시 재시도됩니다.");
                }
            }
            return true;
        }
    }

    /// <summary>
    /// 강제 동기화 (즉시 클라우드 저장)
    /// </summary>
    public async Task<bool> ForceSyncToCloud()
    {
        localSaveCount = localSaveCountBeforeSync;
        return await SyncToCloud();
    }

    private bool TryApplyCloudSave(string rawJson)
    {
        if (!JsonSaveSystem.TryDeserialize(rawJson, out var data))
        {
            Debug.LogError("[SaveSync] 클라우드 데이터 파싱 실패");
            return false;
        }

        try
        {
            JsonSaveSystem.Save(data);
            latestLocalSave = JsonSaveSystem.SerializeToJson(data, false);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SaveSync] 클라우드 데이터를 로컬에 기록 실패: {ex.Message}");
            return false;
        }
    }

    private void WarmLocalCacheFromDisk()
    {
        try
        {
            var localData = JsonSaveSystem.Load();
            latestLocalSave = JsonSaveSystem.SerializeToJson(localData, false);
            LatestCloudSave = latestLocalSave;
            localSaveCount = 0;
            Debug.Log("[SaveSync] 로컬 세이브를 캐시에 로드했습니다.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SaveSync] 로컬 세이브 로드 실패: {ex.Message}");
        }
    }

    private bool TryGetRawJson(string source, out string rawJson)
    {
        rawJson = null;

        if (string.IsNullOrEmpty(source))
            return false;

        if (LooksLikeJson(source))
        {
            rawJson = source;
            return true;
        }

        if (JsonSaveSystem.TryDecryptToJson(source, out var decrypted))
        {
            rawJson = decrypted;
            return true;
        }

        return false;
    }

    private bool LooksLikeJson(string text)
    {
        foreach (char c in text)
        {
            if (char.IsWhiteSpace(c))
                continue;

            return c == '{' || c == '[';
        }

        return false;
    }
}
