using System;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.RemoteConfig;
using Firebase.Extensions;

/// <summary>
/// Firebase Remote Config 버전 체크
/// 앱 버전 확인 및 점검 중 여부 체크
/// </summary>
public class VersionChecker : Singleton<VersionChecker>
{
    private FirebaseRemoteConfig remoteConfig;

    public bool IsInitialized { get; private set; }
    public string MinimumVersion { get; private set; }
    public string LatestVersion { get; private set; }
    public bool IsUnderMaintenance { get; private set; }
    public string MaintenanceMessage { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        InitializeRemoteConfig();
    }

    private void InitializeRemoteConfig()
    {
        remoteConfig = FirebaseRemoteConfig.DefaultInstance;

        var defaults = new System.Collections.Generic.Dictionary<string, object>
        {
            { "minimum_version", "1.0.0" },
            { "latest_version", "1.0.0" },
            { "is_maintenance", false },
            { "maintenance_message", "서버 점검 중입니다." }
        };

        remoteConfig.SetDefaultsAsync(defaults).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                IsInitialized = true;
                Debug.Log("[Version Checker] RemoteConfig 초기화 성공");
                _ = FetchRemoteConfig();
            }
            else
            {
                Debug.LogError("[Version Checker] RemoteConfig 초기화 실패");
            }
        });
    }

    /// <summary>
    /// Remote Config 값 가져오기
    /// </summary>
    public async Task<bool> FetchRemoteConfig()
    {
        if (!IsInitialized)
        {
            Debug.LogError("[Version Checker] RemoteConfig 초기화되지 않음");
            return false;
        }

        try
        {
            await remoteConfig.FetchAsync(TimeSpan.Zero);
            await remoteConfig.ActivateAsync();

            MinimumVersion = remoteConfig.GetValue("minimum_version").StringValue;
            LatestVersion = remoteConfig.GetValue("latest_version").StringValue;
            IsUnderMaintenance = remoteConfig.GetValue("is_maintenance").BooleanValue;
            MaintenanceMessage = remoteConfig.GetValue("maintenance_message").StringValue;

            Debug.Log($"[Version Checker] Fetch 성공 - Min: {MinimumVersion}, Latest: {LatestVersion}, Maintenance: {IsUnderMaintenance}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Version Checker] Fetch 실패: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 현재 앱 버전이 최소 요구 버전 이상인지 확인
    /// </summary>
    public bool IsVersionValid()
    {
        string currentVersion = Application.version;
        return CompareVersion(currentVersion, MinimumVersion) >= 0;
    }

    /// <summary>
    /// 업데이트가 필요한지 확인
    /// </summary>
    public bool IsUpdateAvailable()
    {
        string currentVersion = Application.version;
        return CompareVersion(currentVersion, LatestVersion) < 0;
    }

    /// <summary>
    /// 버전 비교 (Major.Minor.Patch)
    /// </summary>
    private int CompareVersion(string v1, string v2)
    {
        var parts1 = v1.Split('.');
        var parts2 = v2.Split('.');

        for (int i = 0; i < Mathf.Max(parts1.Length, parts2.Length); i++)
        {
            int num1 = i < parts1.Length && int.TryParse(parts1[i], out int n1) ? n1 : 0;
            int num2 = i < parts2.Length && int.TryParse(parts2[i], out int n2) ? n2 : 0;

            if (num1 != num2)
                return num1.CompareTo(num2);
        }

        return 0;
    }

    /// <summary>
    /// 게임 진입 가능 여부 체크
    /// </summary>
    public async Task<(bool canEnter, string message)> CheckGameAccess()
    {
        await FetchRemoteConfig();

        if (IsUnderMaintenance)
        {
            return (false, MaintenanceMessage);
        }

        if (!IsVersionValid())
        {
            return (false, $"업데이트가 필요합니다.\n최소 버전: {MinimumVersion}\n현재 버전: {Application.version}");
        }

        if (IsUpdateAvailable())
        {
            Debug.Log($"[Version Checker] 새 버전 사용 가능: {LatestVersion}");
        }

        return (true, "");
    }
}
