using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreSlotUI : SlotBase<StoreItemSO>
{
    [Header("상점 구성")]
    [SerializeField] List<SlotUI> itemSlots; // 구매 아이템 리스트(이미지, 갯수)
    [SerializeField] Button purchaseBtn; // 구매 버튼
    [SerializeField] TextMeshProUGUI nameText; // 아이템 이름
    [SerializeField] TextMeshProUGUI descriptionText; // 아이템 설명
    [SerializeField] TextMeshProUGUI priceText; // 아이템 가격
    [SerializeField] Image costIcon; // 구매 재화 아이콘

    [SerializeField] Sprite diamondSprite;
    [SerializeField] Sprite soulStoneSprite;
    
    public event Action<StoreItemSO> OnPurchaseBtnClicked;

    private void OnEnable()
    {
        purchaseBtn.onClick.AddListener(HandlePurchaseClicked);
    }

    private void OnDisable()
    {
        purchaseBtn.onClick.RemoveListener(HandlePurchaseClicked);
    }

    public override void Setup(StoreItemSO itemData)
    {
        base.Setup(itemData);
    }

    protected override void UpdateUI()
    {
        nameText.text = _data.Name;
        descriptionText.text = _data.Description;

        if(_data.priceType == StorePriceTypeEnum.KRW)
        {
            priceText.text = $"KRW {_data.price}";
        }
        else
        {
            priceText.text = _data.price.ToString("N0");
        }


        switch (_data.priceType)
        {
            case StorePriceTypeEnum.Diamond:
                costIcon.gameObject.SetActive(true);
                costIcon.sprite = diamondSprite;
                break;
            case StorePriceTypeEnum.SoulStone:
                costIcon.gameObject.SetActive(true);
                costIcon.sprite = soulStoneSprite;
                break;
            case StorePriceTypeEnum.KRW:
                costIcon.gameObject.SetActive(false);
                break;
        }

        for(int i = 0;  i < itemSlots.Count; i++)
        {
            if (i < _data.rewards.Count)
            {
                itemSlots[i].gameObject.SetActive(true);
                RewardData reward = _data.rewards[i];
                var data = DataManager.Instance.GetData<GameData>(reward.rewardId);
                if(data != null)
                {
                    itemSlots[i].Setup(data, true, reward.amount);
                }
                else
                {
                    itemSlots[i].Clear();
                    itemSlots[i].gameObject.SetActive(false);
                }
            }
            else
            {
                itemSlots[i].Clear();
                itemSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public override void Clear()
    {
        nameText.text = string.Empty;
        descriptionText.text = string.Empty;
        priceText.text = string.Empty;
        
        foreach (var slot in itemSlots)
        {
            slot.Clear();
        }
    }

    private void HandlePurchaseClicked()
    {
        OnPurchaseBtnClicked?.Invoke(_data);
    }

}
