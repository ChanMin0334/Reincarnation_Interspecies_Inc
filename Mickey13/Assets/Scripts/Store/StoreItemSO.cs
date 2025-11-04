using System.Collections.Generic;
using UnityEngine;
public enum StoreCategoryEnum
{
    None, 
    Diamond, //다이아
    SoulStone, //환생석
    Package, //패키지
    Subscription, //구독제
}

public enum StoreItemPopupEnum
{
    Large,
    Middle,
    Small
}

public enum StorePriceTypeEnum
{
    Diamond,
    KRW,
    SoulStone
}

[System.Serializable]
public class RewardData
{
    public string rewardId; //ID
    public int amount; // 수량
}

//테스트용
[CreateAssetMenu(fileName = "StoreItemSO", menuName = "Store/StoreItemSO")]

public class StoreItemSO : GameData
{
    [Header("기본 Info")]
    public int price;
    public StoreCategoryEnum category = StoreCategoryEnum.None;
    public StoreItemPopupEnum popup = StoreItemPopupEnum.Small;
    public StorePriceTypeEnum priceType = StorePriceTypeEnum.Diamond;
    public int maxPurchaseCount;

    [Header("보상목록")]
    public List<RewardData> rewards = new();
}
