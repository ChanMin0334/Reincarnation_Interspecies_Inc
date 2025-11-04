using System.Collections.Generic;
using UnityEngine;

public class QuestPanel : MonoBehaviour
{
    [SerializeField] QuestSlotUI prefab; // 퀘스트 슬롯 프리팹
    [SerializeField] Transform content; // 퀘스트 프리팹 생성할 부모

    private List<QuestSlotUI> activeSlots = new(); // 현재 활성화된 슬롯들 저장(오브젝트풀)

    private void OnEnable()
    {
        InitQuests(QuestManager.Instance.QuestDatas);
        QuestManager.Instance.OnQuestUpdated += RefreshSlot;
        QuestManager.Instance.OnQuestsReset += HandleQuestsReset;

        User.Instance.OnGoodsChanged += UpdateAllQuestSlots;
        User.Instance.OnAchievementKmChanged += UpdateAllQuestSlots;
        GameManager.Instance.Tutorial.QuestTutorial();

    }
    private void OnDisable()
    {
        QuestManager.Instance.OnQuestUpdated -= RefreshSlot;
        QuestManager.Instance.OnQuestsReset -= HandleQuestsReset;
        
        User.Instance.OnGoodsChanged -= UpdateAllQuestSlots;
        User.Instance.OnAchievementKmChanged -= UpdateAllQuestSlots;
        
        ClearAllSlots();
    }

    private void InitQuests(List<QuestData> questDatas)
    {
        ClearAllSlots();

        foreach (var questData in questDatas)
        {
            GameObject obj = PoolingManager.Instance.Get(prefab.gameObject, content);
            QuestSlotUI slot = obj.GetComponent<QuestSlotUI>();

            slot.Setup(questData);
            activeSlots.Add(slot); // 현재 활성화된 슬롯 리스트에 추가

            //이벤트 핸들러는 연결 해제 후 다시 연결하는게 안전
            slot.OnUnlockClicked -= HandleUnlockClicked;
            // slot.OnUpgradeClicked -= HandleUpgradeClicked;
            slot.OnUnlockClicked += HandleUnlockClicked;
            // slot.OnUpgradeClicked += HandleUpgradeClicked;
        }
    }
    
    private void ClearAllSlots()
    {
        if (activeSlots.Count == 0) return;
        foreach (var slot in activeSlots)
        {
            PoolingManager.Instance.Release(slot.gameObject);
        }
        activeSlots.Clear();
    }

    private void RefreshSlot(QuestData questData) // 퀘스트슬롯 새로고침
    {
        QuestSlotUI selectedSlot = activeSlots.Find(slot => slot.Data.BaseData.ID == questData.BaseData.ID);
        if (selectedSlot != null)
        {
            selectedSlot.Setup(questData);
        }
    }

    private void HandleUnlockClicked(QuestData data)
    {
        Debug.Log("퀘스트 잠금 해제 버튼 클릭 이벤트 실행");
        QuestManager.Instance.UnLockQuest(data);
    }

    // private void HandleUpgradeClicked(QuestData data)
    // {
    //     Debug.Log("퀘스트 강화 버튼 클릭 이벤트 실행");
    //     QuestManager.Instance.LevelUpQuest(data.BaseData.ID);
    // }

    private void HandleQuestsReset() // 퀘스트 초기화 이벤트 구독
    {
        InitQuests(QuestManager.Instance.QuestDatas);
    }

    private void UpdateAllQuestSlots()
    {
        foreach (var slot in activeSlots)
        {
            slot.UpdateQuestStatus();
        }
    }    
}
