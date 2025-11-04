using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : Singleton<GameManager>
{
    public bool isGamePaused = true;

    public Camera cameraMain;

    [SerializeField] StatDatabaseSO statDatabase;
    public StatDatabaseSO StatDatabase => statDatabase;
    
    [Header("튜토리얼")]
    [SerializeField] TutorialManager tutorialManager;
    public TutorialManager Tutorial => tutorialManager;
    
    public float SpeedLevel { get; set; } = 1;

    [SerializeField] public int StageLevel = 1;

    void Start()
    {
        Tutorial.Initialized();
        SaveManager.Instance.LoadUser();
        AudioManager.Instance.PlayBGM(BgmType.Stage_1);
        Time.timeScale = GameManager.Instance.SpeedLevel;
    }
    
    public void ClearAllTutorial() // 튜토리얼 초기화 디버그용
    {
        foreach (TutorialType tutorialType in System.Enum.GetValues(typeof(TutorialType)))
        {
            if (tutorialType == TutorialType.None) continue;
            
            string tutorialKey = tutorialType.ToString();
            PlayerPrefs.DeleteKey(tutorialKey);
        }
        PlayerPrefs.Save();
    }

    // 모바일/WebGL 환경에서 백그라운드 진입 시 호출됨
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PauseGame();
            SaveManager.Instance.SaveUser();
        }
        else
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
        Debug.Log("게임 일시정지 (모바일/WebGL)");
        SaveManager.Instance.SaveUser();
    }

    private void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = GameManager.Instance.SpeedLevel;
        Debug.Log("게임 재개 (모바일/WebGL)");
        User.Instance.GetIdleReward(JsonSaveSystem.Load());
    }
}
