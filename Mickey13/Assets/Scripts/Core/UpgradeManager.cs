using UnityEngine;

public interface IUpgradeable
{
    int currentLevel { get; }
    
    BigNumeric CalculateTotalCost(int levelsToUpgrade);
    int CalculateMaxAffordableLevel();
    void ApplyUpgrade(int levelsToUpgrade);
}

public class UpgradeManager : Singleton<UpgradeManager>
{
    [Header("강화 비용 설정")] [SerializeField] private float baseCost = 100f; // 기본 비용 (기존 100)
    [SerializeField] private float costMultiplier = 1.2f; // 증가 배율 (기존 1.2f)

    public float BaseCost => baseCost;
    public float CostMultiplier => costMultiplier;

    public string CurrentUpgradeCount { get; private set; } = "x1"; // 강화 횟수 캐싱

    public void SetUpgradeCount(string newCount)
    {
        if (CurrentUpgradeCount == newCount) return;

        CurrentUpgradeCount = newCount;
        EventManager.Instance.TriggerEvent(EventType.OnChangedUpgradeCount, CurrentUpgradeCount);
    }

    public void TryUpgrade(IUpgradeable item, int levelsToUpgrade)
    {
        if (item == null || levelsToUpgrade <= 0) return;

        BigNumeric totalCost = item.CalculateTotalCost(levelsToUpgrade);

        if (User.Instance.gold < totalCost) return;

        User.Instance.UseGold(totalCost);
        item.ApplyUpgrade(levelsToUpgrade);

        EventManager.Instance.TriggerEvent(EventType.ItemUpgraded, item);
    }
}

#region Old
// public void GoldCharaUpgrade(Character chara, int level) 

//public void GoldCharaUpgrade(EntityData chara, int level) // 결국 돌고돌아 처음 구조에서 빠져나가지를 못한다.
//{
//    // 돈 체크
//    if (User.Instance.gold < CalculateTotalCost(chara, level))
//    { return; }
//    // -> Money > Cost?
//    // 돈 깎고
//    User.Instance.UseGold(CalculateTotalCost(chara, level)); // 돈은 BigInt으로 관리된다고 가정: 자릿수 계산 오버플로우 등 방지를 위함
//    // tempStat 갱신
//    chara.SetLevel(chara.level +level);
//    Debug.Log(chara.name + "의 레벨이 " + chara.level + "이 되었습니다.");
//    Debug.Log($"현재 체력: {chara.level}, 공격력: {chara.FinalStat.Atk}");
//}

//     public void GoldCharaUpgrade(EntityData chara, int level)
//     { 
//         var cost = CalculateTotalCost(chara, level);
//         // 돈 체크
//         if (User.Instance.gold < cost)
//         { return; }
//         // -> Money > Cost?
//         // 돈 깎고
//         User.Instance.UseGold(cost); // 돈은 BigInt으로 관리된다고 가정: 자릿수 계산 오버플로우 등 방지를 위함
//             //## UI 이벤트 호출
//                                   //User.Instance.OnGoodsChanged.Invoke();
//                               // tempStat 갱신
//         chara.SetLevel(chara.level + level);
//
//         EventManager.Instance.TriggerEvent(EventType.CharacterStatChanged);
//         EventManager.Instance.TriggerEvent(EventType.UpdateCharacterToInventory);
//
//         Debug.Log(chara.name + "의 레벨이 " + chara.level + "이 되었습니다.");
//         Debug.Log($"현재 체력: {chara.level}, 공격력: {chara.FinalStat.Atk}");
//     }
//
//     public void Assention(Character chara) // 돌파 메서드: 현재 캐릭터 스크립트에 돌파 관련 데이터가 없어서 주석처리
//     {
//
//     }
//
//     public BigNumeric CalculateTotalCost(EntityData chara, int level)
//     {
//         BigNumeric totalCost = 0;
//         int currentLevel = chara.level;
//         for (int i = 0; i < level; i++)
//         {
//             totalCost += CalculateSingleCost(currentLevel + i); // 임의로 1.2배씩 증가하는 공식 설정 - 100, 120, 144, 172...
//         }
//         return totalCost;
//     }
//
//     public BigNumeric CalculateSingleCost(int level)
//     {
//          BigNumeric singleCost = Mathf.Pow(costMultiplier, level)*baseCost;
//          return singleCost;
//     }
//
//     public int CalculateMaxAffordableLevel(EntityData chara) // 현재 골드로 최대 몇 레벨까지 올릴 수 있는지 계산
//     {
//         BigNumeric currentGold = User.Instance.gold;
//         BigNumeric cumulativeGold = 0; // 레벨업 누적비용
//         
//         int currentLevel = chara.level;
//         int maxLevel = 0;
//         while (true)
//         {
//             BigNumeric nextCost = CalculateSingleCost(currentLevel);
//             if (currentGold >= cumulativeGold + nextCost)
//             {
//                 cumulativeGold += nextCost;
//                 maxLevel++;
//                 currentLevel++;
//             }
//             else
//             {
//                 break;
//             }
//         }
//         return maxLevel;
//     }
// }

#endregion
