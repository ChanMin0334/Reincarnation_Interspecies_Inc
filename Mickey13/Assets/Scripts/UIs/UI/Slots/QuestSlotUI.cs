using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestSlotUI : SlotBase<QuestData>
{
    [Header("QuestInfo")]
    [SerializeField] SlotUI questIcon; // 퀘스트 아이콘
    [SerializeField] TextMeshProUGUI questName; // 퀘스트 이름
    [SerializeField] TextMeshProUGUI questLevel; 
    [SerializeField] TextMeshProUGUI questUnlcok; 

    [Header("Gold")]
    [SerializeField] TextMeshProUGUI questReward; // 퀘스트로 얻는 골드량
    [SerializeField] TextMeshProUGUI unlockGoldCost; // 퀘스트 해금에 필요한 골드량
    [SerializeField] TextMeshProUGUI UpgradeGoldCost; // 퀘스트 강화에 필요한 골드량
    
    [Header("Time")]
    [SerializeField] TimerBar timerBar; //리니어 타이머 바
    [SerializeField] TextMeshProUGUI timer; // 남은 시간 

    [Header("Button")]
    [SerializeField] Button upgradeBtn; // 퀘스트 강화 버튼
    [SerializeField] Button unlockBtn; // 퀘스트 활성화 버튼
    
    private string currentEnhanceCount;
    public event Action<QuestData> OnUnlockClicked;
    // public event Action<QuestData> OnUpgradeClicked;

    private void OnEnable()
    {        
        upgradeBtn.onClick.AddListener(OnClickUpgrade);
        unlockBtn.onClick.AddListener(OnClickUnlock);
        EventManager.Instance.StartListening(EventType.ItemUpgraded, OnClickUpgrade);
        EventManager.Instance.StartListening(EventType.OnChangedUpgradeCount, HandleUpgradeCostChanged);
    }

    private void OnDisable()
    {
        upgradeBtn.onClick.RemoveListener(OnClickUpgrade);
        unlockBtn.onClick.RemoveListener(OnClickUnlock);
        if (EventManager.Instance != null)
        {
            EventManager.Instance.StopListening(EventType.ItemUpgraded, OnClickUpgrade);
            EventManager.Instance.StopListening(EventType.OnChangedUpgradeCount, HandleUpgradeCostChanged);
        }
    }

    public void Update()
    {
        {
            if (_data != null && _data.CurrentStatus == QuestStatus.InProgress)
                UpdateTimer();
        }
    }
    public override void Setup(QuestData data)
    {
        base.Setup(data);
        if (UpgradeManager.Instance != null)
        {
            currentEnhanceCount = UpgradeManager.Instance.CurrentUpgradeCount;
        }
        UpdateUI();
    }

    private void UpdateTimer()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(_data.RemainingTime); // 초를 시:분:초 로 변경
        timer.text = timeSpan.ToString(@"hh\:mm\:ss");

        timerBar.UpdateBar(_data.RemainingTime, _data.BaseData.QuestDuration);
    }

    protected override void UpdateUI() // 퀘스트 데이터 클래스 매개변수 (ex QuestData data)
    {
        questIcon.Setup(_data);
        questName.text = _data.BaseData.Name;
        questLevel.text = $"LV. {_data.CurrentLevel}";
        questReward.text = (_data.GoldReward.value * User.Instance.LaborMult).ToString();
        questUnlcok.text = $"해금 조건\n<color=#B52727><size=125%>{_data.BaseData.QuestRequirement}Km</size></color> 도달";
        unlockGoldCost.text = _data.CalculateUnlockPrice.ToString();
        // UpgradeGoldCost.text = _data.UpgradeGold.ToString();
        UpdateTimer();
        UpdateQuestStatus();
    }

    public void UpdateQuestStatus() // 퀘스트 타입에 따른 해금 버튼이랑 강화 버튼 표시 유무
    {
        BigNumeric userMoney = User.Instance.gold;
        
        switch(_data.CurrentStatus)
        {
            case QuestStatus.Locked:
                questLevel.text = "Lv.0";
                questUnlcok.gameObject.SetActive(true);
                unlockBtn.gameObject.SetActive(true);
                unlockBtn.interactable = (userMoney >= _data.CalculateUnlockPrice && 
                     User.Instance.ReincarnateData.MaxDistance >= _data.BaseData.QuestRequirement );
                
                upgradeBtn.gameObject.SetActive(false);
                upgradeBtn.interactable = false;
                upgradeBtn.GetComponentInChildren<TextMeshProUGUI>().text = "잠금";
                break;

            default:
                unlockBtn.gameObject.SetActive(false);
                questUnlcok.gameObject.SetActive(false);
                upgradeBtn.gameObject.SetActive(true);
                
                // 추가. 강화 배율
                IUpgradeable item = _data;
                if (item == null) return;
                
                string currentCount = currentEnhanceCount;
                int levelsToUpgrade = 0;
                BigNumeric cost = 0;

                switch (currentCount)
                {
                    case "x1":
                        levelsToUpgrade = 1;
                        break;
                    case "x10":
                        levelsToUpgrade = 10;
                        break;
                    case "x100":
                        levelsToUpgrade = 100;
                        break;
                    case "Max":
                        levelsToUpgrade = item.CalculateMaxAffordableLevel();
                        break;
                }
                
                if (levelsToUpgrade > 0)
                {
                    cost = item.CalculateTotalCost(levelsToUpgrade);
                }
                else
                {
                    cost = item.CalculateTotalCost(1);
                }
                UpgradeGoldCost.text = cost.ToString();
                upgradeBtn.interactable = (userMoney >= cost) && (levelsToUpgrade >0);
                upgradeBtn.GetComponentInChildren<TextMeshProUGUI>().text = "강화";
                break;
        }
    }

    public void OnClickUnlock()
    {
        OnUnlockClicked?.Invoke(_data);
        Debug.Log("잠금 해제 버튼 클릭");
    }

    public void OnClickUpgrade()
    {
        if (_data == null) return;
        IUpgradeable item = _data;
        if( item == null) return;
        
        int levelsToUpgrade = 0;

        switch (currentEnhanceCount)
        {
            case "x1":
                levelsToUpgrade = 1;
                break;
            case "x10":
                levelsToUpgrade = 10;
                break;
            case "x100":
                levelsToUpgrade = 100;
                break;
            case "Max":
                levelsToUpgrade = item.CalculateMaxAffordableLevel();
                break;
        }

        if (levelsToUpgrade > 0)
        {
            UpgradeManager.Instance.TryUpgrade(item, levelsToUpgrade);
        }
        UpdateUI();
    }

    private void HandleUpgradeCostChanged(object data)
    {
        currentEnhanceCount = (string)data;
        if (data != null)
        {
            UpdateQuestStatus();
        }
        UpdateUI();
    }
}
