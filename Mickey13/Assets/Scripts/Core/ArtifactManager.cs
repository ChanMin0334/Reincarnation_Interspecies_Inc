using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArtifactManager : Singleton<ArtifactManager>
{
    [Header("보스 상자 확률")]
    [SerializeField] private ArtifactDropTable bossDropTable;
    [SerializeField] private ArtifactDropTable middleBossDropTable;


    [SerializeField] ArtifactInventory inventory; // 유물 인벤토리
    [SerializeField] ArtifactDropTable currentDropTable; // 현재유물 드롭테이블

    [SerializeField] private int maxRerollCount = 3; // 최대 리롤 횟수
    [SerializeField] private int rerollCost = 100; // 리롤 1회당 필요 재화량
    private int currentRerollCount;
    public int MaxRerollCount => maxRerollCount;
    public int RerollCost => rerollCost;
    public int CurrentRerollCount => currentRerollCount;

    private PopupArtifactReward artifactPopup; // Popup
    private List<ArtifactSO> currentArtifactsList; // 선택할 3개의 유물 리스트
    private ArtifactSO selectedArtifact; // 선택한 유물

    //private Queue<ArtifactChestData> chestQueues = new();
    private Dictionary<ArtifactChestType, Queue<ArtifactChestData>> chestQueues = new();


    public IReadOnlyList<ArtifactSO> CurrentArtifactsList => currentArtifactsList;

    public event Action<List<ArtifactSO>> OnChoicesUpdated; //유물 선택목록 업데이트 이벤트
    public event Action<ArtifactSO> OnSelectionChanged; // 선택한 유물 변경 이벤트
    public event Action OnPopupClosed; // 팝업 종료 이벤트
    public event Action<ArtifactChestType, int> OnChestCountChanaged;

    // UI가 현재 상자 개수를 가져올 수 있도록 public 메서드 제공
    public int GetChestCount(ArtifactChestType type)
    {
        return chestQueues.ContainsKey(type) ? chestQueues[type].Count : 0;
    }

    protected override void Awake()
    {
        chestQueues.Add(ArtifactChestType.normal, new Queue<ArtifactChestData>());
        chestQueues.Add(ArtifactChestType.special, new Queue<ArtifactChestData>());
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            AddChest(ArtifactChestType.special, bossDropTable);
            Debug.Log("보스상자획득");
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            AddChest(ArtifactChestType.normal, middleBossDropTable);
            Debug.Log("중간보스상자획득");
        }
    }
#endif

    public void AddChest(ArtifactChestType type, ArtifactDropTable dropTable)
    {
        var chestData = new ArtifactChestData(dropTable);
        chestQueues[type].Enqueue(chestData);
        Debug.Log($"상자 추가 {chestQueues[type].Count}개");
        OnChestCountChanaged?.Invoke(type, chestQueues[type].Count);
    }

    public void OpenChest(ArtifactChestType type, Sprite icon)
    {
        if (chestQueues[type].Count == 0) return;

        var chest = chestQueues[type].Dequeue();

        OnChestCountChanaged?.Invoke(type, chestQueues[type].Count);

        // 유물 선택창 열기 및 이벤트 구독 추가
        var popup = UIManager.Instance.Open<PopupArtifactReward>();
        popup.Setup(this, type, icon);
        popup.OnConfirm.AddListener(ConfirmSelection);
        popup.OnRetry.AddListener(Reroll);

        // 팝업창 닫을 시 이벤트 구독 해제
        OnPopupClosed += () =>
        {
            popup.OnConfirm.RemoveListener(ConfirmSelection);
            popup.OnRetry.RemoveListener(Reroll);
        };

        StartArtifactChoice(chest.dropTable);
    }

    private void ClearChest()
    {
        foreach (var chest in chestQueues.Values)
            chest.Clear();
    }

    private void OnEnable()
    {
        EventManager.Instance.StartListening(EventType.EndReincarnate, OnReincarnate);
        EventManager.Instance.StartListening(EventType.BossKilled, OnBossKilled);
        EventManager.Instance.StartListening(EventType.MiddleBossKilled, OnMiddleBossKilled);
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.StopListening(EventType.EndReincarnate, OnReincarnate);
            EventManager.Instance.StopListening(EventType.BossKilled, OnBossKilled);
            EventManager.Instance.StopListening(EventType.MiddleBossKilled, OnMiddleBossKilled);
        }
    }

    //
    private void OnBossKilled() {AddChest(ArtifactChestType.special, bossDropTable);}
    private void OnMiddleBossKilled() {AddChest(ArtifactChestType.normal, middleBossDropTable);}
    
    public void StartArtifactChoice(ArtifactDropTable dropTable) // 팝업 및 유물 호출
    {
        if (dropTable == null) return;
        currentDropTable = dropTable;

        currentRerollCount = maxRerollCount;
        selectedArtifact = null;
        currentArtifactsList = dropTable.GetRandomCandidates(3, false);

        OnChoicesUpdated?.Invoke(currentArtifactsList);
    }

    public void SelectedArtifact(ArtifactSO selected) // 유물 선택
    {
        if (currentArtifactsList == null) return;
        selectedArtifact = selected;
        OnSelectionChanged?.Invoke(selectedArtifact);
    }

    public void ConfirmSelection() // 선택한 유물 획득하기
    {
        if (selectedArtifact == null) return;

        inventory.AddArtifact(selectedArtifact.ID);
        Debug.Log($"{selectedArtifact.name} 인벤토리에 보관");

        Debug.Log("유물 선택 끝");
        OnPopupClosed?.Invoke();
    }

    public void Reroll() // 다시뽑기
    {
        if (currentRerollCount == 0)
        {
            Debug.Log("리롤 횟수가 부족합니다");
            UIManager.Instance.Open<PopupAlert>().ShowAlert($"리롤 횟수가 부족합니다.\n남은횟수 : {currentRerollCount}");
            return;
        }

        if (rerollCost > User.Instance.soulStone)
        {
            Debug.Log("리롤 비용이 부족합니다.");
            UIManager.Instance.Open<PopupAlert>().ShowAlert($"리롤 비용이 부족합니다.");
            return;
        }
        else
        {
            User.Instance.UseSoulStone(rerollCost);

            currentRerollCount--;
            selectedArtifact = null;
            currentArtifactsList = currentDropTable.GetRandomCandidates(3, false);

            OnChoicesUpdated?.Invoke(currentArtifactsList);
        }
    }

    public void OnReincarnate()
    {
        //ArtifactEffectManager.Instance.RemoveAllEffects();
        inventory.Init(new List<ArtifactData>());
        ClearChest();
    }

    // 상자 큐 저장
    public ChestQueueSaveData ToSaveData()
    {
        var saveData = new ChestQueueSaveData
        {
            normalChestCount = chestQueues[ArtifactChestType.normal].Count,
            specialChestCount = chestQueues[ArtifactChestType.special].Count
        };
        
        Debug.Log($"[ArtifactManager] 상자 저장 - 일반: {saveData.normalChestCount}, 특별: {saveData.specialChestCount}");
        
        return saveData;
    }

    // 상자 큐 로드
    public void LoadFromSaveData(ChestQueueSaveData data)
    {
        Debug.Log($"[ArtifactManager] 상자 로드 시작 - 일반: {data.normalChestCount}, 특별: {data.specialChestCount}");
        
        // 기존 큐 초기화
        chestQueues[ArtifactChestType.normal].Clear();
        chestQueues[ArtifactChestType.special].Clear();
        Debug.Log($"[ArtifactManager] 큐 초기화 완료");

        // 저장된 개수만큼 상자 추가 (이벤트 발생 없이 직접 큐에 추가)
        for (int i = 0; i < data.normalChestCount; i++)
        {
            var chestData = new ArtifactChestData(middleBossDropTable);
            chestQueues[ArtifactChestType.normal].Enqueue(chestData);
        }
        Debug.Log($"[ArtifactManager] 일반 상자 {data.normalChestCount}개 추가 완료. 현재 큐 크기: {chestQueues[ArtifactChestType.normal].Count}");

        for (int i = 0; i < data.specialChestCount; i++)
        {
            var chestData = new ArtifactChestData(bossDropTable);
            chestQueues[ArtifactChestType.special].Enqueue(chestData);
        }
        Debug.Log($"[ArtifactManager] 특별 상자 {data.specialChestCount}개 추가 완료. 현재 큐 크기: {chestQueues[ArtifactChestType.special].Count}");

        // UI 갱신 이벤트 발생
        Debug.Log($"[ArtifactManager] UI 갱신 이벤트 발생 - OnChestCountChanaged null? {OnChestCountChanaged == null}");
        OnChestCountChanaged?.Invoke(ArtifactChestType.normal, chestQueues[ArtifactChestType.normal].Count);
        OnChestCountChanaged?.Invoke(ArtifactChestType.special, chestQueues[ArtifactChestType.special].Count);
        
        // UI 컨트롤러가 있다면 직접 갱신 호출
        var chestController = FindObjectOfType<ArtifactChestController>();
        if (chestController != null)
        {
            Debug.Log($"[ArtifactManager] ArtifactChestController 찾음 - 직접 UI 갱신 호출");
            chestController.RefreshChestUI();
        }
        else
        {
            Debug.LogWarning($"[ArtifactManager] ArtifactChestController를 찾을 수 없음");
        }
        
        Debug.Log($"[ArtifactManager] 상자 로드 완료");
    }
}
