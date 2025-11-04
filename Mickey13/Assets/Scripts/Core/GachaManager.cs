using System.Collections;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using UnityEngine;

public enum CurrencyType
{
    Gold,
    Diamond,
    SoulStone,
}

public class GachaManager : Singleton<GachaManager>
{
    [SerializeField] List<GachaBannerSO> bannerSOs; // 뽑기 머신 목록
    private Dictionary<GachaType, int> pityCounters = new(); // 뽑기 머신 별 천장 카운트 저장용 딕셔너리
    [SerializeField] int gachaCount = 0; // 현재 뽑기 횟수

    private int GachaCount => gachaCount; // 현재 뽑기 횟수 접근용 프로퍼티
    
    public event Action<GachaType> OnPityCountUpdated;

    //public GachaMachine GetMachine(GachaType type) // 뽑기 머신 정보 주입
    //{
    //    foreach (var machine in gachaMachines)
    //    {
    //        if (machine.type == type)
    //            return machine;
    //    }
    //    Debug.LogError($"{type}에 해당하는 GachaMachine을 찾을 수 없습니다!");
    //    return null;
    //}

    /// <summary>
    /// 뽑기 실행 후 결과 반환
    /// 가챠타입, 횟수, 가챠머신
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count"></param>
    /// <param name="machine"></param>
    /// <returns></returns>
    //public List<GameData> ReturnGachaResult(GachaType type, int count, GachaMachine machine)
    //{
    //    var gachaResults = new List<GameData>(); // 뽑은 Data 목록
    //    var diamond = User.Instance.diamond;

    //    for (int i = 0; i < count; i++)
    //    {
    //        if(diamond < gachaCost)
    //        {
    //            Debug.Log("뽑기 비용이 부족합니다.");
    //            UIManager.Instance.Open<PopupAlert>().ShowAlert("보유 재화가 부족합니다.");
    //            return new List<GameData>(); // 비어있는 리스트 반환
    //        }
    //        var so = machine.Pull();
    //        User.Instance.UseDiamond(gachaCost);
    //        gachaCount++;
    //        if(so == null) continue;

    //        gachaResults.Add(so);

    //    }
    //    AddToInventory(gachaResults); // 뽑은캐릭터 중복 체크 후 인벤토리에 보관

    //    return gachaResults;
    //}


    ///// <summary>
    ///// 뽑은 가챠 목록 중복 체크 후 인벤토리에 저장
    ///// </summary>
    ///// <param name="gachaResults"></param>
    //public void AddToInventory(List<GameData> gachaResults)
    //{
    //    //TODO 중복 캐릭터 인벤토리 저장 안되게 조건 추가
    //    foreach (var result in gachaResults)
    //    {
    //        //TODO 가챠횟수만큼 재화 감소

    //        switch (result.DataType)
    //        {
    //            case GameDataType.Character:
    //                User.Instance.charInven.AddCharacter(result as CharacterSO); //현재 CharInventory에 저장
    //                break;
    //            case GameDataType.Rune:
    //                User.Instance.runeInven.AddRune(result.ID);
    //                break;
    //            default:
    //                Debug.LogError("GachaMachine Log : 뽑기 결과 타입오류");
    //                return;
    //        }
    //    }
    //}
    public int GetCurrentPityCount(GachaType type)
    {
        return pityCounters.GetValueOrDefault(type, 0);
    }
    public List<GachaBannerSO> GetActiveGachaBanners()
    {
        if (bannerSOs == null) return new();
        return bannerSOs;
    }

    public List<ISlotUIData> ReturnGachaResult(GachaBannerSO bannerSO, int count)   // 뽑기 결과만 반환하도록 수정
    {
        if (!UseGachaCost(bannerSO, count)) return new(); // 재화 부족 시 빈 리스트 반환

        var gachaResults = new List<GameData>(); // 뽑은 Data 목록
        var gachaMachine = bannerSO.GachaMachine;
        GachaType gachaType = bannerSO.GachaType;

        for (int i = 0; i < count; i++)
        {
            int currentyPity = pityCounters.GetValueOrDefault(gachaType, 0); // 천장 카운트가 없으면 0 반환

            var (result, newPity) = gachaMachine.Pull(currentyPity);
            pityCounters[gachaType] = newPity;
            if (result != null)
                gachaResults.Add(result);
        }
        OnPityCountUpdated?.Invoke(gachaType);
        return AddToInventory(gachaResults);
    }

    private List<ISlotUIData> AddToInventory(List<GameData> gachaResults)
    {
        var uiDataResults = new List<ISlotUIData>();
        foreach(var result in gachaResults)
        {
            ISlotUIData uiData = null;
            switch(result.DataType)
            {
                case GameDataType.Character:
                    var (entityData, isNewChar) = User.Instance.charInven.AddCharacter(result as CharacterSO); //현재 CharInventory에 저장
                    uiData = new CharacterUIData(result as CharacterSO, entityData, isNewChar);
                    break;
                case GameDataType.Rune:
                    var (runeData, isNewRune) = User.Instance.runeInven.AddRune(result.ID);
                    uiData = runeData;
                    runeData.IsNew = isNewRune;
                    break;
                default:
                    Debug.LogError("GachaMachine Log : 뽑기 결과 타입오류");
                    return null;
            }

            if(uiData != null)
            {
                uiDataResults.Add(uiData);
            }
        }
        return uiDataResults;
    }

    private bool UseGachaCost(GachaBannerSO bannerSO, int count)
    {
        int totalCost = count * bannerSO.GachaCost;
        bool hasEnoughtCurrency = false; // 재화 소지 여부

        switch(bannerSO.CurrencyType)
        {
            case CurrencyType.Gold:
                hasEnoughtCurrency = User.Instance.gold.number >= (BigInteger)totalCost;
                break;
            case CurrencyType.Diamond:
                hasEnoughtCurrency = User.Instance.diamond >= totalCost;
                break;
            case CurrencyType.SoulStone:
                hasEnoughtCurrency = User.Instance.soulStone.number >= (BigInteger)totalCost;
                break;
        }

        if (!hasEnoughtCurrency) // 재화가 부족할 경우
        {
            UIManager.Instance.Open<PopupAlert>().ShowAlert("보유 재화가 부족합니다.");
            UIManager.Instance.Close<PopupGachaResult>();
            return false;
        }

        switch(bannerSO.CurrencyType)
        {
            case CurrencyType.Gold:
                User.Instance.UseGold(totalCost);
                break;
            case CurrencyType.Diamond:
                User.Instance.UseDiamond(totalCost);
                break;
            case CurrencyType.SoulStone:
                User.Instance.UseSoulStone(totalCost);
                break;
        }
        return true;
    }
}
