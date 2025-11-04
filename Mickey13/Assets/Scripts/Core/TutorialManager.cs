using System.Collections.Generic;
using UnityEngine;
using System;

public enum TutorialType
{
    None,
    Intro,                 // 처음 시작했을 때
    StarterPackPurchase,   // 스타트 캐릭터 패키지 구매 완료
    Enhance,            // 강화 탭
    FormationEnter,        // 편성창 진입
    FormationExit,         // 편성 완료 후 퇴장
    Quest,              // 퀘스트 탭
    Inventory,             // 보관함
    TeamAllDead,             // 파티 전멸
    ArtifactBox,           // 유물함 획득
    Store,                 // 상점
    Gacha,                  // 가챠
    CharStats              // 캐릭터 스탯
}

public class TutorialManager : MonoBehaviour
{
    private PopupTutorial popupTutorial;

    public void Initialized()
    {
        EventManager.Instance.StartListening(EventType.AllCharacterDead, TeamAllDeadTutorial);
    }
    
    private void ShowTutorial(TutorialType tutorialType, List<string> messages)
    {
        string tutorialKey = tutorialType.ToString();
        if (PlayerPrefs.GetInt(tutorialKey, 0) == 1) return; // 이미 완료한 튜토리얼이라면 스킵

        Time.timeScale = 0f;
        
        Action onComplete = () =>
        {
            PlayerPrefs.SetInt(tutorialKey, 1);
            PlayerPrefs.Save();
            popupTutorial = null;
            
            Time.timeScale = GameManager.Instance.SpeedLevel;
        };
        
        UIManager.Instance.Open<PopupTutorial>().ShowMessage(messages, onComplete);
    }

    public bool HasPlayedTutorial(TutorialType tutorialType)
    {
        string tutorialKey = tutorialType.ToString();
        return PlayerPrefs.GetInt(tutorialKey, 0) == 1;
    }
    
    public void IntroTutorial()
    {
        List<string> messages = new List<string>
        {
            "만나서 반가워!\n내 이름은 아린!\n지금부터 게임에 대해 간단하게 설명해줄게",
            "먼저 상점 앞에서 기다리고 있는\n동료들을 데리러 가자.\n아래의 상점 탭을 터치해봐!"
        };
        ShowTutorial(TutorialType.Intro, messages);
    }
    
    public void EnhanceTutorial()
    {
        List<string> messages = new List<string>
        {
            "동료들을 파티에 등록해볼까?\n화면 중단 왼쪽에 있는\n편성하기 버튼을 클릭해봐!",
            "혹시 동료들을 아직 데려오지 않았다면\n먼저 상점으로 가봐!\n그곳에서 동료들이 기다리고 있어."
        };
        ShowTutorial(TutorialType.Enhance, messages);
    }

    public void FormationEnterTutorial()
    {
        List<string> messages = new List<string>
        {
            "여기에서 동료들을 파티에 편성할 수 있어!",
            "우측에 배치된 동료는 공격당할 확률이 높아",
            "그러니까 나처럼 튼튼한 동료는\n전열에 배치하는게 좋다구~",
            "아래에 있는 사진 중 하나를 터치하면\n그 동료가 파티에 들어올거야",
            "만약 파티에서 제외하고 싶다면\n다시 한번 사진을 터치해봐.",
            "배치를 완료했다면\n편성하기 버튼을 눌러서\n동료들과 함께 밖으로 나가보자!"
        };
        ShowTutorial(TutorialType.FormationEnter, messages);
    }      
    
    public void FormationExitTutorial()
    {
        List<string> messages = new List<string>
        {
            "지금 강화창에 동료들이 편성된게 보이지?",
            "여기에서 파티에 참가한\n동료들의 능력을 상승시킬 수 있어!",
            "버튼을 꾹 누르고 있으면\n연속해서 강화할 수도 있는 것 같아.",
            "얼른 강해져서\n이종족 녀석들을 잔뜩 잡으러 가자!",
        };
        ShowTutorial(TutorialType.FormationExit, messages);
    }    
    
    public void QuestTutorial()
    {
        List<string> messages = new List<string>
        {
            "여기에서 작업장을 관리할 수 있어!",
            "30km마다 등장하는\n우두머리 녀석들을 사로잡으면\n작업을 명령할 수 있는 것 같아.",
            "게임에 접속해있지 않더라도\n녀석들은 자나깨나 꾸준히 돈을 벌어올거야.", // 인간이 미안해
            "이종족 녀석들을 잔뜩 잡아오면\n놀면서도 부자가 될 수 있다는 말씀!"
        };
        ShowTutorial(TutorialType.Quest, messages);
    }
    
    public void InventoryTutorial()
    {
        List<string> messages = new List<string>
        {
            "여기는 인벤토리야!",
            "동료들의 정보와\n현재 소지하고 있는 룬,\n그리고 유물을 확인할 수 있어",
            "동료의 정보를 확인하고 싶으면\n사진을 꾸욱! 길게 눌러봐!",
            "원하는 것을 쉽게 찾을 수 있도록\n필터와 정렬 기능도 있으니까 확인해봐~!"
        };
        ShowTutorial(TutorialType.Inventory, messages);
    }
    
    public void StarterPackagePurchaseTutorial()
    {
        List<string> messages = new List<string>
        {
            "이제 전투에 나가기 위한 준비를 해볼까?",
            "처음 만났던 강화창으로 돌아가서\n파티에 동료들을 편성해보자!"
        };
        ShowTutorial(TutorialType.StarterPackPurchase, messages);
    }

    public void CharStatsTutorial()
    {
        List<string> messages = new List<string>
        {
            "여기서 동료의 정보를 확인할 수 있어!",
            "함께할 동료를 잘 파악해두는건 중요하다구!"
        };
        ShowTutorial(TutorialType.CharStats, messages);
    }

    public void StoreTutorial()
    {
        List<string> messages = new List<string>
        {
            "여기가 상점이야!",
            "여기서 전투에 도움이 되는\n상품들을 구매할 수 있어!",
            "아직 상품이 많이 들어오지는 않은 것 같네?",
            "나중에는 상품이 더 많아질테니\n시간이 난다면 확인해봐!",
            "그럼 일단 동료들을 만나러 가볼까?\n (캐릭터 패키지를 구매해주세요)"
        };
        ShowTutorial(TutorialType.Store, messages);
    }

    public void ArtifactTutorial()
    {
        List<string> messages = new List<string>
        {
            "해냈어! 보스를 처치했다구!\n보스를 처치하면 보물상자를 얻을 수 있어!",
            "획득한 보물상자는 화면 좌상단에 쌓이니까\n 한번씩 잊지말고 확인해봐.",
            "보물상자에는 전투에 도움이 되는\n 유물들이 들어있어!",
            "유물은 환생을 하면 사라지고,\n3개 중 하나만 가져갈 수 있으니\n신중하게 선택해봐.",
            "유물을 선택하고\n회수하기 버튼을 누르면\n획득할 수 있어!",
            "유물이 마음에 들지 않으면\n3번까지 목록을 갱신할 수도 있다구~!",
            "무슨 유물을 얻었는지 알고 싶다면\n소지품 탭에서 확인해봐!"
        };
        ShowTutorial(TutorialType.ArtifactBox, messages);
    }

    public void GachaTutorial()
    {
        List<string> messages = new List<string>
        {
            "여기에서는\n새로운 동료들을 모집하거나,\n룬을 얻을 수 있어!",
            "룬은 환생과 관계없이 영구적으로 작용하고,\n 강해지려면 많은 룬이 필요해",
            "그러니까 영혼석이 모이면\n 참지말고 바로바로 써버리자구~!"
        };
        ShowTutorial(TutorialType.Gacha, messages);
    }    
    
    public void TeamAllDeadTutorial()
    {
        List<string> messages = new List<string>
        {
            "이런! 이종족 녀석들이 너무 강해졌는데?",
            "강화를 해도 더이상 전진하기 어렵다면\n환생을 시도해봐",
            "환생하기 버튼을 누르면\n지금까지 도달한 거리에 비례해서\n영혼석을 얻을 수 있어.",
            "영혼석으로 새로운 동료를 모집하거나,\n좋은 룬을 얻어서 다시 도전해보자!"
        };
        ShowTutorial(TutorialType.TeamAllDead, messages);
        EventManager.Instance.StopListening(EventType.AllCharacterDead,TeamAllDeadTutorial);
    }
}
