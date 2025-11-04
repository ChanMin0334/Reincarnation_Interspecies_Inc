using System.Threading.Tasks;
using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 타이틀 화면에서 로그인 선택과 진행을 제어한다.
/// </summary>
public class TitleSceneController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button googleLoginButton;
    [SerializeField] private Button guestLoginButton;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Flow Settings")]
    [SerializeField] private bool autoLoadNextScene = false;
    [SerializeField] private bool tapToStart = true;
    [SerializeField] private string nextSceneName;

    private bool isProcessing;
    private bool awaitingTap;
    private bool hasStarted;

    private FirebaseAuthManager authManager;
    private SaveSyncManager saveSyncManager;
    private GoogleSignInManager googleSignInManager;

    private void Awake()
    {
        authManager = FirebaseAuthManager.Instance;
        saveSyncManager = SaveSyncManager.Instance;
        googleSignInManager = GoogleSignInManager.Instance;

        googleLoginButton?.onClick.AddListener(OnGoogleLoginClicked);
        guestLoginButton?.onClick.AddListener(OnGuestLoginClicked);

        if (authManager != null)
        {
            authManager.OnSignInSuccess += HandleFirebaseSignInSuccess;
            authManager.OnSignInFailed += HandleFirebaseSignInFailed;
        }

        if (googleSignInManager != null)
        {
            googleSignInManager.SignInFailed += HandleGoogleSignInFailed;
        }
    }

    private void Start()
    {
        HideStatus();

        if (authManager != null && authManager.IsSignedIn)
        {
            SetBusy("이전 로그인 정보를 확인하는 중...");
            _ = InitializeAfterSignInAsync();
        }
        else
        {
            SetIdle("로그인 방법을 선택하세요.");
        }
    }

    private void OnDestroy()
    {
        googleLoginButton?.onClick.RemoveListener(OnGoogleLoginClicked);
        guestLoginButton?.onClick.RemoveListener(OnGuestLoginClicked);

        if (authManager != null)
        {
            authManager.OnSignInSuccess -= HandleFirebaseSignInSuccess;
            authManager.OnSignInFailed -= HandleFirebaseSignInFailed;
        }

        if (googleSignInManager != null)
        {
            googleSignInManager.SignInFailed -= HandleGoogleSignInFailed;
        }
    }

    private void Update()
    {
        if (!awaitingTap || hasStarted)
            return;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            TryStartGame();
            return;
        }
#endif

        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    TryStartGame();
                    break;
                }
            }
        }
    }

    private void OnGoogleLoginClicked()
    {
        if (isProcessing)
            return;

        SetBusy("Google 로그인 창을 여는 중...");
        googleSignInManager?.SignInWithGoogle();
    }

    private async void OnGuestLoginClicked()
    {
        if (isProcessing)
            return;

        SetBusy("게스트 로그인 중...");

        if (authManager == null)
        {
            ShowErrorThenReturnToIdle("Firebase 초기화가 필요합니다.");
            return;
        }

        bool success = await authManager.SignInAnonymously();
        if (!success)
        {
            ShowErrorThenReturnToIdle("게스트 로그인에 실패했습니다.");
        }
        // 성공 시에는 HandleFirebaseSignInSuccess가 호출되어 이후 흐름을 담당한다.
    }

    private void HandleFirebaseSignInSuccess(FirebaseUser user)
    {
        _ = HandleFirebaseSignInSuccessAsync(user);
    }

    private async Task HandleFirebaseSignInSuccessAsync(FirebaseUser user)
    {
        SetBusy("세이브 데이터를 동기화하는 중...");

        bool syncSuccess = await saveSyncManager.InitializeCloudSave();
        if (syncSuccess)
        {
            string displayName = string.IsNullOrEmpty(user?.DisplayName) ? "Guest" : user.DisplayName;
            HandleReadyToStart($"환영합니다, {displayName}!");
        }
        else
        {
            ShowErrorThenReturnToIdle("세이브 데이터 동기화에 실패했습니다.");
        }
    }

    private void HandleFirebaseSignInFailed(string error)
    {
        string message = string.IsNullOrEmpty(error) ? "로그인에 실패했습니다." : $"로그인 실패: {error}";
        ShowErrorThenReturnToIdle(message);
    }

    private void HandleGoogleSignInFailed(string error)
    {
        string message = string.IsNullOrEmpty(error) ? "Google 로그인을 취소했습니다." : $"Google 로그인 실패: {error}";
        ShowErrorThenReturnToIdle(message);
    }

    private async Task InitializeAfterSignInAsync()
    {
        bool success = await saveSyncManager.InitializeCloudSave();
        if (success)
        {
            HandleReadyToStart("이전에 로그인한 계정으로 계속 진행합니다.");
        }
        else
        {
            ShowErrorThenReturnToIdle("세이브 데이터를 확인할 수 없습니다.");
        }
    }

    private void SetBusy(string message)
    {
        isProcessing = true;
        awaitingTap = false;
        SetButtonsVisible(false);
        ShowStatus(message);
    }

    private void SetIdle(string message)
    {
        isProcessing = false;
        awaitingTap = false;
        SetButtonsVisible(true);
        HideStatus();
        LogStatus(message);
    }

    private void ShowStatus(string message)
    {
        if (statusText != null)
        {
            statusText.gameObject.SetActive(true);
            statusText.text = message;
        }

        LogStatus(message);
    }

    private void HideStatus()
    {
        if (statusText != null)
            statusText.gameObject.SetActive(false);
    }

    private void SetButtonsVisible(bool visible)
    {
        if (googleLoginButton != null)
        {
            googleLoginButton.gameObject.SetActive(visible);
            googleLoginButton.interactable = visible;
        }

        if (guestLoginButton != null)
        {
            guestLoginButton.gameObject.SetActive(visible);
            guestLoginButton.interactable = visible;
        }
    }

    private void HandleReadyToStart(string baseMessage)
    {
        if (tapToStart)
        {
            isProcessing = false;
            awaitingTap = true;
            SetButtonsVisible(false);
            ShowStatus("화면을 터치하면 시작합니다.");
        }
        else
        {
            SetIdle(baseMessage);
            if (autoLoadNextScene)
            {
                LoadNextScene();
            }
        }
    }

    private async void ShowErrorThenReturnToIdle(string message)
    {
        isProcessing = false;
        awaitingTap = false;
        SetButtonsVisible(false);
        ShowStatus(message);
        await Task.Delay(1500);

        if (this == null)
            return;

        SetIdle("로그인 방법을 선택하세요.");
    }

    private void LogStatus(string message)
    {
        Debug.Log($"[Title] {message}");
    }

    private void LoadNextScene()
    {
        if (hasStarted)
            return;

        if (string.IsNullOrWhiteSpace(nextSceneName))
            return;

        hasStarted = true;
        SceneManager.LoadScene(nextSceneName);
    }

    private void TryStartGame()
    {
        if (isProcessing || hasStarted)
            return;

        awaitingTap = false;
        HideStatus();
        LoadNextScene();
    }
}
