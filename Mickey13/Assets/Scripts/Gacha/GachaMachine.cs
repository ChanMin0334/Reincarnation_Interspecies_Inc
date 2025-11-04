using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PickupPools //픽업 개별 확률 // 아래와 동일 외부로 뺄 가능성 있음
{
    public GameData pickupData;
    public float pickupPercent;
}

[Serializable]
public class RarityPercent //등급별 확률 //유물뽑기가 나오면 외부로 뺄수있음 + 등급 공유시
{
    public RarityEnum rarity; 
    public float percent;             
}

[Serializable]
public class GachaMachine
{
    [Header("등급별 확률 따라가는 캐릭터 풀")]
    [SerializeField] private List<GameData> basicPools; //일반목록(등급별 확률 따라감)


    [SerializeField] private List<RarityPercent> rarityPercent;

    [Header("픽업 캐릭터 풀 (개별 확률 컨트롤 가능)")]
    [SerializeField] private List<PickupPools> pickupPools; //픽업풀

    [SerializeField] private bool debugMode = false; //디버깅모드
                                                     //[SerializeField] private CharInventory inventory; //테스트 => 추후에는 빠져야할것

    //public GachaType type;

    //[SerializeField] private int pityCounter = 0;
    [Header("천장")]
    [SerializeField] private int pityLimit = 10;
    
    public int PityLimit => pityLimit;

    public (GameData result, int newPityCount) Pull(int currentPity) //추후 키값으로 대체 가능 Key => DataManager => GetData
    {
        if (basicPools == null || basicPools.Count == 0)
        {
            Debug.LogError("GachaMachine Log : 기본 풀 비어있음");
            return (null, currentPity);
        }

        if (rarityPercent == null || rarityPercent.Count == 0)
        {
            Debug.LogError("GachaMachine Log : 등급 확률 없음");
            return (null, currentPity);
        }

        int thisPityCount = currentPity + 1; // 이번 뽑기 횟수 (50번째 뽑기때 픽업 출현하도록 만드는 변수)
        if(thisPityCount >= pityLimit && pickupPools.Count > 0)
        {
            //currentPity = 0;
            var forcePickUp = GetRandomPickUp();

            if(debugMode) Debug.Log($"GachaMachine Log : 천장도달 {forcePickUp.Name} 획득");

            return (forcePickUp, 0);
        }

        float random = UnityEngine.Random.Range(0, GetTotalPercent());

        if(pickupPools.Count > 0)
        {
            var pickupResult = TryPickUp(random);
            if (pickupResult != null)
            {
                //currentPity = 0;
                return (pickupResult,0);
            }

            //pityCounter++;
        }
        
        return (GetBasicPickUp(random),thisPityCount);
    }

    private float GetTotalPercent()
    {
        float rarityTotal = 0f;
        foreach (var r in rarityPercent)
        {
            rarityTotal += r.percent;
        }

        if(pickupPools.Count > 0)
        {
            float pickupTotal = 0f;
            foreach (var e in pickupPools)
            {
                pickupTotal += e.pickupPercent;
            }
            return pickupTotal + rarityTotal;
        }


        return rarityTotal;     
    }

    private GameData TryPickUp(float random)
    {
        float sum = 0f;
        for(int i = 0; i < pickupPools.Count; i++)
        {
            sum += pickupPools[i].pickupPercent;
            if(random < sum)
            {
                if (debugMode) Debug.Log($"GachaMachine Log : 픽업 당첨 {pickupPools[i].pickupData.Name}");
                return pickupPools[i].pickupData;
            }
        }
        return null;
    }

    private GameData GetBasicPickUp(float random)
    {
        float total = rarityPercent.Sum(r => r.percent);
        float sum = 0f;

        RarityEnum pickRarity = RarityEnum.None;

        foreach(var r in rarityPercent)
        {
            sum += r.percent;
            if(random <= sum)
            {
                pickRarity = r.rarity;
                Debug.Log($"pickRarity 값 {pickRarity}");
                Debug.Log($"GachaMachine Count {rarityPercent.Count}");
                break;
            }
        }

        if(pickRarity == RarityEnum.None && rarityPercent.Count > 0)
        {
            pickRarity = rarityPercent[rarityPercent.Count - 1].rarity;
        }

        var pool = basicPools.FindAll(ch => ch.Rarity == pickRarity);
        if(pool.Count == 0)
        {
            Debug.LogError($"{pickRarity} 등급 캐릭터 없음");
            return null;
        }

        var result = pool[UnityEngine.Random.Range(0, pool.Count)];

        return result;
    }


    private GameData GetRandomPickUp() //픽업뽑는 함수
    {
        if (pickupPools == null || pickupPools.Count == 0) return null;

        int index = UnityEngine.Random.Range(0, pickupPools.Count);

        return pickupPools[index].pickupData;
    }
}
