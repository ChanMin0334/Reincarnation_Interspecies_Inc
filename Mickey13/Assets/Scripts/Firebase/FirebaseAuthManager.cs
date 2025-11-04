using System;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Crashlytics;

/// <summary>
/// Firebase 인증 관리자
/// Google 로그인 및 익명 로그인 처리
/// </summary>
public class FirebaseAuthManager : Singleton<FirebaseAuthManager>
{
    private FirebaseAuth auth;
    private FirebaseUser currentUser;
    private bool crashlyticsReady;
    private bool isForwardingLog;
    private bool crashlyticsAvailable = true;

    public bool IsInitialized { get; private set; }
    public bool IsSignedIn => currentUser != null;
    public string UserId => currentUser?.UserId;

    public event Action<FirebaseUser> OnSignInSuccess;
    public event Action<string> OnSignInFailed;

    protected override void Awake()
    {
        base.Awake();
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                auth.StateChanged += AuthStateChanged;
                AuthStateChanged(this, null);
                IsInitialized = true;
                SetupCrashlytics();
                Debug.Log("[Firebase Auth] 초기화 성공");
            }
            else
            {
                Debug.LogError($"[Firebase Auth] 초기화 실패: {task.Result}");
                IsInitialized = false;
            }
        });
    }

    private void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        if (auth.CurrentUser != currentUser)
        {
            bool signedIn = currentUser != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && currentUser != null)
            {
                Debug.Log("[Firebase Auth] 로그아웃됨");
                Crashlytics.SetUserId(string.Empty);
            }

            currentUser = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log($"[Firebase Auth] 로그인 성공: {currentUser.UserId}");
                if (!string.IsNullOrEmpty(currentUser.UserId))
                    Crashlytics.SetUserId(currentUser.UserId);
                OnSignInSuccess?.Invoke(currentUser);
            }
        }
    }

    /// <summary>
    /// 익명 로그인
    /// </summary>
    public async Task<bool> SignInAnonymously()
    {
        if (!IsInitialized)
        {
            Debug.LogError("[Firebase Auth] 초기화되지 않음");
            return false;
        }

        try
        {
            await auth.SignInAnonymouslyAsync();
            Debug.Log("[Firebase Auth] 익명 로그인 성공");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Firebase Auth] 익명 로그인 실패: {ex.Message}");
            OnSignInFailed?.Invoke(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Google 로그인 (플랫폼별 Credential 사용)
    /// </summary>
    public async Task<bool> SignInWithGoogle(string idToken, string accessToken)
    {
        if (!IsInitialized)
        {
            Debug.LogError("[Firebase Auth] 초기화되지 않음");
            return false;
        }

        try
        {
            Credential credential = GoogleAuthProvider.GetCredential(idToken, accessToken);
            await auth.SignInWithCredentialAsync(credential);
            Debug.Log("[Firebase Auth] Google 로그인 성공");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Firebase Auth] Google 로그인 실패: {ex.Message}");
            OnSignInFailed?.Invoke(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 로그아웃
    /// </summary>
    public void SignOut()
    {
        if (auth != null)
        {
            auth.SignOut();
            currentUser = null;
            Crashlytics.SetUserId(string.Empty);
            Debug.Log("[Firebase Auth] 로그아웃");
        }
    }

    private void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
        }

        if (crashlyticsReady)
        {
            Application.logMessageReceived -= ForwardLogToCrashlytics;
            crashlyticsReady = false;
        }
    }

    private void SetupCrashlytics()
    {
        if (crashlyticsReady)
            return;

        try
        {
            Crashlytics.IsCrashlyticsCollectionEnabled = true;
        }
        catch (Exception ex)
        {
            crashlyticsAvailable = false;
            Debug.LogWarning($"[Crashlytics] Enable collection failed: {ex.Message}");
            return;
        }

        Application.logMessageReceived -= ForwardLogToCrashlytics;
        Application.logMessageReceived += ForwardLogToCrashlytics;
        crashlyticsReady = true;
    }

    private void ForwardLogToCrashlytics(string condition, string stackTrace, LogType type)
    {
        if (!crashlyticsReady || !crashlyticsAvailable || isForwardingLog)
            return;

        if (type != LogType.Assert && type != LogType.Exception && type != LogType.Error)
            return;

        try
        {
            isForwardingLog = true;
            Crashlytics.Log(condition);
            if (!string.IsNullOrEmpty(stackTrace))
                Crashlytics.Log(stackTrace);

            Crashlytics.LogException(new Exception(string.IsNullOrEmpty(stackTrace)
                ? condition
                : $"{condition}\n{stackTrace}"));
        }
        catch (Exception ex)
        {
            crashlyticsAvailable = false;
            Debug.LogWarning($"[Crashlytics] Forward log failed: {ex.Message}");
        }
        finally
        {
            isForwardingLog = false;
        }
    }
}
