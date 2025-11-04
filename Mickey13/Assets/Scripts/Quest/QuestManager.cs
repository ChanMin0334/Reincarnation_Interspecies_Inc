using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    [SerializeField] private List<QuestData> questDatas; //전체 퀘스트 목록
    [SerializeField] private List<QuestData> activeQuests; //진행중인 퀘스트

    public List<QuestData> QuestDatas => questDatas;

    public event Action<QuestData> OnQuestUpdated; // 퀘스트 상태 업데이트 이벤트
    public event Action OnQuestsReset; // 모든 퀘스트 소프트 리셋(환생용 이벤트)

    protected override void Awake()
    {
        base.Awake();

        questDatas = new List<QuestData>();
        activeQuests = new List<QuestData>();

        var questSos = DataManager.Instance.GetTypeAllData<QuestSO>();

        foreach (var so in questSos)
        {
            questDatas.Add(new QuestData(so));
        }
    }

    private void OnEnable()
    {
        EventManager.Instance.StartListening(EventType.EndReincarnate, OnReincarnate);
    }

    private void OnDisable()
    {
        if(EventManager.Instance != null)
            EventManager.Instance.StopListening(EventType.EndReincarnate, OnReincarnate);
    }


    private void Update()
    {
        if (activeQuests.Count > 0)
        {
            QuestInProgress(); // 리니어 타이머 작동용
        }
    }

    public void UnLockQuest(QuestData quest) //퀘스트 해금
    {
        // QuestData quest = questDatas.Find(q => q.BaseData.ID == id);
        if (quest == null || quest.CurrentStatus != QuestStatus.Locked) return;

        if (quest.BaseData.QuestRequirement > User.Instance.CurAchievementKm)
        {
            Debug.Log("퀘스트 해금 거리에 도달 못했습니다.");
            return;

        }
        BigNumeric unlockCost = quest.CalculateUnlockPrice; // 퀘스트 해금에 필요한 골드
        
        if (unlockCost <= User.Instance.gold)
        {
            User.Instance.UseGold(unlockCost);
            quest.CurrentStatus = QuestStatus.Active;

            //퀘스트 해금시 최초 보상, 업그레이드비용 설정
            quest.GoldReward = quest.BaseData.QuestReward.Clone();
            quest.UpgradeGold = quest.BaseData.QuestUpgradeCost.Clone();

            Debug.Log($"{quest.BaseData.Name}퀘스트 해금");
            AcceptQuest(quest);
            OnQuestUpdated?.Invoke(quest);
        }
        else
        {
            Debug.Log("골드 부족");
            return;
        }
    }

    public void AcceptQuest(QuestData quest) //퀘스트 수락
    {
        // QuestData quest = questDatas.Find(q => q.BaseData.ID == id);
        if (quest != null && !activeQuests.Contains(quest) && quest.CurrentStatus == QuestStatus.Active)
        {
            activeQuests.Add(quest);
            Debug.Log($"{quest.BaseData.Name}퀘스트 수락");

            quest.CurrentStatus = QuestStatus.InProgress;
            OnQuestUpdated?.Invoke(quest);
        }
        else
            Debug.Log($"{quest.BaseData.ID}와 같은 퀘스트가없습니다?");
    }

    // public void GetReward(string id) //퀘스트 보상
    // {
    //     QuestData quest = activeQuests.Find(q => q.BaseData.ID == id);
    //     if (quest != null && quest.CurrentStatus == QuestStatus.Cleared)
    //     {
    //         User.Instance.AddGold(quest.GoldReward.value * User.Instance.LaborMult);
    //
    //         // Debug.Log($"보상 받음{quest.GoldReward}G"); //최적화 주석처리
    //
    //         quest.CurrentStatus = QuestStatus.InProgress;
    //
    //         quest.RemainingTime = quest.BaseData.QuestDuration;
    //
    //         OnQuestUpdated?.Invoke(quest);
    //     }
    // }    
    public void GetReward(QuestData quest) //퀘스트 보상
    {
        if (quest != null && quest.CurrentStatus == QuestStatus.Cleared)
        {
            User.Instance.AddGold(quest.GoldReward.value * User.Instance.LaborMult);

            // Debug.Log($"보상 받음{quest.GoldReward}G"); //최적화 주석처리

            quest.CurrentStatus = QuestStatus.InProgress;

            quest.RemainingTime = quest.BaseData.QuestDuration;

            OnQuestUpdated?.Invoke(quest);
        }
    }

    public void QuestInProgress() //퀘스트 진행
    {
        for (int i = activeQuests.Count - 1; i >= 0; i--)
        {
            var quest = activeQuests[i];
            if (quest.CurrentStatus == QuestStatus.InProgress)
            {
                quest.RemainingTime -= Time.deltaTime;

                if (quest.RemainingTime <= 0f)
                {
                    quest.CurrentStatus = QuestStatus.Cleared;
                    // Debug.Log($"퀘스트 달성{quest.BaseData.Name}"); // GC 발생 원인. 주석처리
                    OnQuestUpdated?.Invoke(quest);
                    
                    GetReward(quest); // 보상 1번만 지급
                    
                    quest.RemainingTime = quest.BaseData.QuestDuration;
                    quest.CurrentStatus = QuestStatus.InProgress;
                }
            }
            // if (quest.CurrentStatus == QuestStatus.Cleared)
            // {
            //     GetReward(quest.BaseData.ID);
            // }
        }
    }

    // public void LevelUpQuest(string id) //퀘스트 레벨업
    // {
    //     int index = questDatas.FindIndex(q => q.BaseData.ID == id);
    //     if (index < 0) return;
    //     QuestData quest = questDatas[index];
    //     BigNumeric upgradeCost = quest.UpgradeGold.Clone(); // 퀘스트 레벨업에 필요한 골드
    //
    //     if (upgradeCost <= User.Instance.gold)
    //     {
    //         User.Instance.UseGold(upgradeCost);
    //
    //         quest.CurrentLevel++;
    //
    //         quest.UpgradeGold.value *= quest.BaseData.UpgradeMult;
    //         quest.GoldReward.value *= quest.BaseData.RewardMult;
    //         OnQuestUpdated?.Invoke(quest);
    //     }
    // }

    //public void ApplyOfflineProgress(float second)
    //{
    //    foreach (var q in activeQuests)
    //    {
    //        if (q.CurrentStatus != QuestStatus.InProgress) continue;

    //        float remaining = second;

    //        //디버그용
    //        int clearCount = 0;
    //        BigNumeric totalGold = 0f;

    //        while (remaining > 0)
    //        {
    //            if (q.RemainingTime > remaining)
    //            {
    //                q.RemainingTime -= remaining;
    //                remaining = 0;
    //            }
    //            else
    //            {
    //                remaining -= q.RemainingTime;
    //                q.RemainingTime = 0;
    //                q.CurrentStatus = QuestStatus.Cleared;

    //                User.Instance.AddGold(q.GoldReward.value * User.Instance.LaborMult);

    //                //디버그용
    //                clearCount++;
    //                totalGold += q.GoldReward;

    //                Debug.Log("오프라인중 퀘스트 완료");

    //                q.CurrentStatus = QuestStatus.InProgress;
    //                q.RemainingTime = q.BaseData.QuestDuration;
    //            }
    //        }

    //        if (clearCount > 0)
    //        {
    //            Debug.Log($"[오프라인 진행] 퀘스트 {q.ID} {clearCount}회 클리어 → 총 {totalGold}G 획득");
    //        }
    //    }
    //}

    public void ApplyOfflineProgress(float second)
    {
        foreach (var q in questDatas)
        {
            if (q.CurrentStatus != QuestStatus.InProgress) continue;

            float questDuration = q.BaseData.QuestDuration;
            if (questDuration <= 0f) continue;

            int clearCount = Mathf.FloorToInt(second / questDuration);

            float remainder = second % questDuration;
            q.RemainingTime = questDuration - remainder;

            BigNumeric totalGold = clearCount * q.GoldReward.value * User.Instance.LaborMult;
            if (clearCount > 0)
            {
                User.Instance.AddGold(totalGold);
                Debug.Log($"[오프라인 진행] 퀘스트 {q.ID} {clearCount}회 클리어 → 총 {totalGold}G 획득");
            }
        }
    }

    public void OnReincarnate()
    {
        activeQuests.Clear();
        foreach (var quest in questDatas)
        {
            quest.CurrentLevel = 1;
            quest.CurrentStatus = QuestStatus.Locked;
            quest.RemainingTime = quest.BaseData.QuestDuration;
            quest.IsUnlocked = false;
            quest.GoldReward = 0;
            quest.UpgradeGold = 0;
        }
        OnQuestsReset?.Invoke();
    }

    public (List<QuestData> allQuest, List<QuestData> activeQuest) ToSaveData() //퀘스트 저장
    {
        return (new List<QuestData>(questDatas), new List<QuestData>(activeQuests));
    }

    public void LoadFromSaveData(List<QuestData> allQuest, List<QuestData> activeQuest)
    {
        // 기존 리스트 재활용
        questDatas.Clear();
        activeQuests.Clear();

        if(allQuest != null)
        {
            foreach (var save in allQuest)
            {
                var so = DataManager.Instance.GetData<QuestSO>(save.ID);
                if (so == null) continue;

                save.SetBaseData(so);

                int savedLevel = save.CurrentLevel;
                
                save.RecalculateValuesFromSO(savedLevel);
                
                // //임시 여기 추가
                // save.GoldReward = so.QuestReward.Clone();
                // save.UpgradeGold = so.QuestUpgradeCost.Clone();
                // save.RemainingTime = so.QuestDuration;
                // //까지

                questDatas.Add(save);

            }
        }

        // 저장된 InProgress 타입 퀘스트의 ID를 이용해, 원본 퀘스트(questDatas)의 참조를 activeQuests에 추가
        if(activeQuest != null)
        {
            foreach (var save in activeQuest)
            {
                QuestData originalQuest = questDatas.Find(q => q.ID == save.ID);
                if (originalQuest != null)
                {
                    activeQuests.Add(originalQuest);
                }
            }
        }

        //세이브에 없는 QuestData 불러오기
        var allQuestSos = DataManager.Instance.GetTypeAllData<QuestSO>();
        foreach (var so in allQuestSos)
        {
            bool alreadyload = questDatas.Exists(q => q.ID == so.ID);
            if (!alreadyload)
            {
                questDatas.Add(new QuestData(so));
            }
        }
    }
}
