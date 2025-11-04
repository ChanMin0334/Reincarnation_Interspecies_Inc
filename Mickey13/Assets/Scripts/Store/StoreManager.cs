using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StorePurchaseData : ISerializationCallbackReceiver // 딕셔너리를 직렬화할 수 있도록 도와주는 인터페이스
{
    [NonSerialized]
    public Dictionary<string, int> purchaseCounts;
    
    [SerializeField]
    private List<string> Keys = new List<string>();
    [SerializeField]
    private List<int> Values = new List<int>();
    
    public void OnBeforeSerialize()
    {
        Keys.Clear();
        Values.Clear();

        foreach (var kvp in purchaseCounts)
        {
            Keys.Add(kvp.Key);
            Values.Add(kvp.Value);
        }
    }
    
    public void OnAfterDeserialize()
    {
        purchaseCounts = new Dictionary<string, int>();

        for (int i = 0; i < Keys.Count; i++)
        {
            purchaseCounts[Keys[i]] = Values[i];
        }
    }
    
    public StorePurchaseData()
    {
        purchaseCounts = new Dictionary<string, int>();
    }
}

public class StoreManager : Singleton<StoreManager>
{
    [Header("상품")]
    [SerializeField] private List<StoreItemSO> storeItems = new(); // 디버그용 직렬화
    private List<StoreItemSO> runtimeStoreItems = new List<StoreItemSO>();
    private StorePurchaseData storePurchaseData = new StorePurchaseData();

    public event Action storeDataInitialized;
    public event Action<StoreItemSO> clearPurchaseItem; // 구매 가능 목록 삭제 이벤트 발행
    public bool isStoreDataInit {get; private set;} = false;
    private void Start()
    {
        storeItems.Clear();

        if(DataManager.Instance != null)
        {
            var loaded = DataManager.Instance.GetTypeAllData<StoreItemSO>();
            storeItems.AddRange(loaded);
            Debug.Log($"StoreManager : {storeItems.Count}개 아이템 등록 완료");

            foreach (var item in storeItems)
                Debug.Log($"StoreManager : 로드된 상품: {item.ID}");
        }
    }

    public void Purchase(StoreItemSO item)
    {
        if(item == null)
        {
            Debug.LogWarning($"StoreManager : item 은 null");
            return;
        }

        var user = User.Instance;

        switch(item.priceType)
        {
            case StorePriceTypeEnum.Diamond:
                if(user.diamond < item.price)
                {
                    Debug.Log("다이아가 부족합니다.");
                    UIManager.Instance.Open<PopupAlert>().ShowAlert($"저런...\n다이아가 부족하신것같네요?");
                    return;
                }
                user.UseDiamond(item.price);
                break;

            case StorePriceTypeEnum.SoulStone:
                if (user.soulStone < (BigNumeric)item.price)
                {
                    Debug.Log("영혼석이 부족합니다.");
                    UIManager.Instance.Open<PopupAlert>().ShowAlert("저런...\n영혼석이 부족하신것같네요?");
                    return;
                }
                user.UseSoulStone(item.price);
                break;

            case StorePriceTypeEnum.KRW:
                break;
        }
        
        if (item.maxPurchaseCount != -1) // -1 (무한 구매)가 아닌 경우
        {
            int remainingCount = storePurchaseData.purchaseCounts[item.ID];
            remainingCount--; 
            storePurchaseData.purchaseCounts[item.ID] = remainingCount; // 영구 저장

            if (remainingCount == 0)
            {
                if (runtimeStoreItems.Contains(item))
                {
                    runtimeStoreItems.Remove(item);
                    clearPurchaseItem?.Invoke(item); 
                }
            }
        }
        
        Debug.Log($"StoreManager : {item.Name} 구매 가격 : {item.price}");

        RewardManager.Instance.GetRewards(item.rewards);
        SaveManager.Instance.SaveUser();
        
        CheckCharacterStarterPackagePurchase(item); // 스타터 패키지 구매시 튜토리얼

        Debug.Log($"StoreManager : {item.Name} 구매 {item.rewards.Count}개 ");
        UIManager.Instance.Open<PopupAlert>().ShowAlert($"구매를 완료했습니다!\n{item.Name}\n구매 가격 : {item.price}");
    }

    private void CheckCharacterStarterPackagePurchase(StoreItemSO package) // 임시, 나중에 스타터 패키지 아이템 ID가 바뀌면 수정해야함
    {
        if (package.ID == "ABC0")
        {
            GameManager.Instance.Tutorial.StarterPackagePurchaseTutorial();
        }
    }

    // public List<StoreItemSO> GetAllItems() => storeItems;
    public List<StoreItemSO> GetItemsForStore() => runtimeStoreItems;

    public StorePurchaseData ToSaveData() => storePurchaseData;

    public void LoadFromSaveData(StorePurchaseData data)
    {
        storePurchaseData = data ?? new StorePurchaseData();

        InitializeRuntimeStore();
    }

    private void InitializeRuntimeStore() // 런타임 상점 목록 초기화 
    {
        // 상점목록이 비었다면 불러오기
        if (storeItems.Count == 0)
        {
            if (DataManager.Instance != null)
            {
                storeItems = DataManager.Instance.GetTypeAllData<StoreItemSO>();
            }
            else
            {
                isStoreDataInit = true;
                storeDataInitialized?.Invoke();
                return;
            }
        }

        runtimeStoreItems.Clear();

        // 구매 가능 횟수 확인
        foreach (var item in storeItems)
        {
            int maxCount = item.maxPurchaseCount;

            if (maxCount == -1) // -1은 무한대 
            {
                runtimeStoreItems.Add(item);
                continue;
            }
            
            // 남은 횟수가 0보다 큰 상점 목록이 있다면 런타임 목록에 추가
            if (storePurchaseData.purchaseCounts.TryGetValue(item.ID, out int remainingCount))
            {
                if (remainingCount > 0)
                {
                    runtimeStoreItems.Add(item); 
                }
            }
            // 새로 추가된 상점 목록이 있다면 최대 횟수로 런타임 목록에 추가
            else
            {
                storePurchaseData.purchaseCounts[item.ID] = maxCount;
                runtimeStoreItems.Add(item); 
            }
        }
        isStoreDataInit = true;
        storeDataInitialized?.Invoke();
    }
}
