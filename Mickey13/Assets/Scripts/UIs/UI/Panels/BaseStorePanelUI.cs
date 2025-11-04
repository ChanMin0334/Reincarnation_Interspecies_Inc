using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class BaseStorePanelUI : MonoBehaviour
{
    [SerializeField] private StoreCategoryEnum storeCategory;

    [SerializeField] private StoreSlotUI smallSlotPrefab;
    [SerializeField] private StoreSlotUI largeSlotPrefab;
    [SerializeField] private Transform slotParent;

    private List<StoreSlotUI> activeSlots = new();

    private void OnEnable()
    {
        if (StoreManager.Instance != null)
        {
            StoreManager.Instance.storeDataInitialized += UpdateUI;
            StoreManager.Instance.clearPurchaseItem += ClearPurchaseItem;
        }

        if (StoreManager.Instance != null && StoreManager.Instance.isStoreDataInit)
        {
            UpdateUI();
        }

    }

    private void OnDisable()
    {
        if (StoreManager.Instance != null)
        {
            StoreManager.Instance.storeDataInitialized -= UpdateUI;
            StoreManager.Instance.clearPurchaseItem -= ClearPurchaseItem;
        }
        ClearSlots();
    }

    private void UpdateUI()
    {
        ClearSlots();

        List<StoreItemSO> items = StoreManager.Instance.GetItemsForStore().Where(item => item.category == storeCategory).ToList();
        
        foreach(var item in items)
        {
            StoreSlotUI prefabToUse;
            switch (item.popup)
            {
                case StoreItemPopupEnum.Large:
                    prefabToUse = largeSlotPrefab;
                    break;
                case StoreItemPopupEnum.Small:
                default:
                    prefabToUse = smallSlotPrefab;
                    break;
            }
            
            var obj = PoolingManager.Instance.Get(prefabToUse.gameObject, slotParent);
            var slot = obj.GetComponent<StoreSlotUI>();
            slot.Setup(item);
            slot.OnPurchaseBtnClicked += HandlePurchaseRequest;
            activeSlots.Add(slot);
        }
    }

    private void ClearSlots()
    {
        if (activeSlots.Count == 0) return;

        foreach (var slot in activeSlots)
        {
            slot.OnPurchaseBtnClicked -= HandlePurchaseRequest;
            slot.Clear();
            PoolingManager.Instance?.Release(slot.gameObject);
        }
        activeSlots.Clear();
    }

    private void ClearPurchaseItem(StoreItemSO item)
    {
        var slot = activeSlots.FirstOrDefault(s => s.Data.ID == item.ID);
        if (slot != null)
        {
            slot.OnPurchaseBtnClicked -= HandlePurchaseRequest;
            slot.Clear();
            PoolingManager.Instance.Release(slot.gameObject);
            activeSlots.Remove(slot);
        }
    }

    private void HandlePurchaseRequest(StoreItemSO item)
    {
        UIManager.Instance.Open<PopupConfirm>()
            .ShowPurchase(item, () => StoreManager.Instance.Purchase(item));
    }
}
