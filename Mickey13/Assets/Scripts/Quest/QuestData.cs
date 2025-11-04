using System;
using UnityEngine;
using System.Numerics;

public enum QuestStatus
{
    Locked,
    Active,
    InProgress,
    Cleared
}

[Serializable]
public class QuestData : ISlotUIData, IUpgradeable, IInventoryData
{
    private QuestSO baseData;
    [SerializeField] private string id;
    [SerializeField] private int currentLevel;
    [SerializeField] private QuestStatus currentStatus;
    [SerializeField] private float remainingTime;
    [SerializeField] private bool isUnlocked;

    public QuestSO BaseData => baseData;


    public string ID => id;

    public int CurrentLevel { get => currentLevel; set => currentLevel = value; }
    public QuestStatus CurrentStatus { get => currentStatus; set => currentStatus = value; }
    public float RemainingTime { get => remainingTime; set => remainingTime = value; }
    public bool IsUnlocked { get => isUnlocked; set => isUnlocked = value; }

    public QuestData(QuestSO baseSO)
    {
        baseData = baseSO;
        id = baseSO.ID;
        currentLevel = 1;
        currentStatus = QuestStatus.Locked;
        remainingTime = BaseData.QuestDuration;
        isUnlocked = false;
        GoldReward = BaseData.QuestReward;
        UpgradeGold = BaseData.QuestUpgradeCost;
    }

    public void SetBaseData(QuestSO so)
    {
        baseData = so;
        id = so.ID;
    }

    public Sprite GetSprite(SlotImageType imageType)
    {
        return baseData.Sprite;
    }

    public BigNumeric CalculateUnlockPrice => BaseData.QuestUpgradeCost; //퀘스트 해금비용

    public BigNumericWrapper GoldReward = 0;
    public BigNumericWrapper UpgradeGold = 0;
    public string Name => baseData.name;

    public RarityEnum Rarity => baseData.Rarity;

    public bool IsNew { get; private set; }

    public QuestData Clone()
    {
        return new QuestData(this.BaseData)
        {
            currentLevel = this.currentLevel,
            currentStatus = this.currentStatus,
            remainingTime = this.remainingTime,
            isUnlocked = this.isUnlocked,
            GoldReward = this.GoldReward.Clone(),
            UpgradeGold = this.UpgradeGold.Clone()
        };
    }

    #region 업그레이드
    
    int IUpgradeable.currentLevel => this.CurrentLevel;
    
    public BigNumeric CalculateTotalCost(int levelsToUpgrade)
    {
        if (levelsToUpgrade <= 0) return new BigNumeric(0);

        // 공비(r) 와 초항(C)
        float r = BaseData.UpgradeMult;
        BigNumeric C = this.UpgradeGold.Clone();

        // 공비를 분수로 변환
        (BigNumeric num, BigNumeric den) = BigNumeric.ParseToFraction(r);

        if ((num == den) || (num - den) == new BigNumeric(0))
        {
            return C * levelsToUpgrade; // (1회 비용 * 횟수)
        }
        
        // 등비수열의 합 공식: S_n = C * ( (r^n) - 1 ) / ( r - 1 )
        BigNumeric r_pow_n_num = BigNumeric.Pow(num, levelsToUpgrade); // num^n
        BigNumeric r_pow_n_den = BigNumeric.Pow(den, levelsToUpgrade); // den^n
        
        // (r^n - 1)
        BigNumeric numerator_part1 = r_pow_n_num - r_pow_n_den;
        BigNumeric denominator_part1 = r_pow_n_den;

        // (r - 1)
        BigNumeric numerator_part2 = num - den;
        BigNumeric denominator_part2 = den;

        // ( (r^n) - 1 ) / ( r - 1 )
        BigNumeric final_numerator = numerator_part1 * denominator_part2;
        BigNumeric final_denominator = denominator_part1 * numerator_part2;

        if (final_denominator == 0)
        {
            return C * levelsToUpgrade;
        }

        return (C * final_numerator) / final_denominator;

        // BigNumeric totalCost = 0;
        //
        // BigNumeric upgradeCost = this.UpgradeGold.Clone();
        // float multiplier = BaseData.UpgradeMult;
        //
        // for (int i = 0; i < levelsToUpgrade; i++)
        // {
        //     totalCost += upgradeCost;
        //     upgradeCost *= multiplier;
        // }
        // return totalCost;
    }

    // public int CalculateMaxAffordableLevel()
    // {
    //     BigNumeric currentGold = User.Instance.gold;
    //     BigNumeric cumulativeGold = 0;
    //     int ceiling = 500000; // 이론 상의 최대 업그레이드 레벨
    //     int floor = CurrentLevel;
    //     while (floor < ceiling)
    //     {
    //         int mid = (ceiling + floor + 1) / 2;
    //         if (CalculateTotalCost(mid) > currentGold)
    //         {
    //             ceiling = mid-1;
    //         }
    //         else
    //         {
    //             floor = mid;
    //         }
    //     }
    //
    //     return floor;
    // }
    
public int CalculateMaxAffordableLevel()
{
    BigNumeric currentGold = User.Instance.gold;

    // --- 참고 ---
    // ApplyUpgrade 메서드에서 'UpgradeGold.value'를 사용하는 것을 기반으로,
    // 실제 BigNumeric 값이 'UpgradeGold.value'에 있다고 가정합니다.
    // 만약 'UpgradeGold' 자체가 BigNumeric 타입이라면 '.value'를 제거해주세요.
    BigNumeric C = this.UpgradeGold.value; 
    // --- ---

    float r_float = BaseData.UpgradeMult;

    // 1. 공비(r)가 1인 경우 (등차수열)
    if (Math.Abs(r_float - 1.0f) < 0.0001f)
    {
        if (C.number == 0) return 500000; // 0으로 나누기 방지 (최대 레벨 반환)
        
        BigNumeric n = currentGold / C;
        
        // BigNumeric을 int로 변환 (Overflow 가능성 체크)
        try
        {
            // n.number는 BigInteger이므로 int로 캐스팅
            return (int)n.number; 
        }
        catch (OverflowException)
        {
            // int.MaxValue 또는 게임의 논리적 최대값(500000) 반환
            return 500000; 
        }
    }

    // 2. 공비(r)가 1이 아닌 경우 (등비수열)
    // 목표: n = log_r( (Gold * (r - 1) / C) + 1 )
    
    // K = ( (Gold * (r - 1)) / C ) + 1
    float r_minus_1_float = r_float - 1.0f;
    
    // (r-1)을 BigNumeric의 분수 형태로 변환
    (BigNumeric r_m1_num, BigNumeric r_m1_den) = BigNumeric.ParseToFraction(r_minus_1_float);

    // 0으로 나누기 방지
    if (r_m1_den.number == 0 || C.number == 0) return 0; 

    // K = ( (Gold * r_m1_num) / (C * r_m1_den) ) + 1
    // K = ( (Gold * r_m1_num) + (C * r_m1_den) ) / (C * r_m1_den)
    
    BigNumeric K_numerator = (currentGold * r_m1_num) + (C * r_m1_den);
    BigNumeric K_denominator = C * r_m1_den;

    // 0으로 나누기 또는 log(0) 방지
    if (K_denominator.number == 0) return 0;
    
    // K가 0 또는 음수이면 log 계산 불가 (업그레이드 불가)
    if (K_numerator.number <= 0) return 0;

    // n = log(K) / log(r)  (자연로그 ln 사용)
    
    // log(K) = log(K_num / K_den) = log(K_num) - log(K_den)
    // BigInteger.Log(value)는 자연로그(base e) 값을 double로 반환합니다.
    double logK = BigInteger.Log(K_numerator.number) - BigInteger.Log(K_denominator.number);
    
    // log(r)
    double logR = Math.Log(r_float); // float이므로 Math.Log (자연로그) 사용

    // logR이 0에 가까우면 (r이 1에 가까우면) 위에서 이미 처리됨
    if (Math.Abs(logR) < 0.000001)
    {
        // r=1인 경우와 동일하게 처리 (안전장치)
        if (C.number == 0) return 500000;
        try { return (int)(currentGold / C).number; }
        catch (OverflowException) { return 500000; }
    }

    int levelsToUpgrade = (int)Math.Floor(logK / logR);

    // logK가 음수이고 logR이 양수이면 (r > 1, K < 1) n이 음수가 됨 -> 0 반환
    if (levelsToUpgrade < 0) return 0; 

    // 부동 소수점 연산의 미세한 오차로 n이 1 작게 계산될 수 있음
    // n+1 레벨의 비용을 '단 한 번만' 계산해서 확인
    if (CalculateTotalCost(levelsToUpgrade + 1) <= currentGold)
    {
        return levelsToUpgrade + 1;
    }
    else
    {
        return levelsToUpgrade;
    }
}

    public void ApplyUpgrade(int levelsToUpgrade)
    {
        if (levelsToUpgrade <= 0) return;
        
        CurrentLevel += levelsToUpgrade;

        // UpgradeGold 배율 적용
        (BigNumeric ugNum, BigNumeric ugDen) = BigNumeric.ParseToFraction(BaseData.UpgradeMult);
        BigNumeric totalUgNum = BigNumeric.Pow(ugNum, levelsToUpgrade);
        BigNumeric totalUgDen = BigNumeric.Pow(ugDen, levelsToUpgrade);
    
        UpgradeGold.value = (UpgradeGold.value * totalUgNum) / totalUgDen;

        // GoldReward 배율 적용
        (BigNumeric grNum, BigNumeric grDen) = BigNumeric.ParseToFraction(BaseData.RewardMult);
        BigNumeric totalGrNum = BigNumeric.Pow(grNum, levelsToUpgrade);
        BigNumeric totalGrDen = BigNumeric.Pow(grDen, levelsToUpgrade);

        GoldReward.value = (GoldReward.value * totalGrNum) / totalGrDen;
        
        // UpgradeGold.value *= MathF.Pow(BaseData.UpgradeMult, levelsToUpgrade);
        // GoldReward.value *= Mathf.Pow(BaseData.RewardMult, levelsToUpgrade);
    }

    public void RecalculateValuesFromSO(int targetLevel)
    {
        if (baseData == null) return;
        
        // SO 상의 퀘스트 보상과 비용 클론으로 복제해오기
        BigNumeric baseReward = baseData.QuestReward.Clone();
        BigNumeric baseCost = baseData.QuestUpgradeCost.Clone();

        // 레벨을 몇 올려야 하는지 계산
        int levelsToApply = targetLevel - 1;

        // 올려야 할 레벨이 없거나 음수인 경우 기본 SO 정보 반영
        if (levelsToApply <= 0)
        {
            GoldReward.value = baseReward;
            UpgradeGold.value = baseCost;
        }
        else
        {
            // UpgradeGold 배율 적용
            (BigNumeric ugNum, BigNumeric ugDen) = BigNumeric.ParseToFraction(BaseData.UpgradeMult);
            BigNumeric totalUgNum = BigNumeric.Pow(ugNum, levelsToApply);
            BigNumeric totalUgDen = BigNumeric.Pow(ugDen, levelsToApply);
            
            UpgradeGold.value = (UpgradeGold.value * totalUgNum) / totalUgDen;
            
            // GoldReward 배율 적용
            (BigNumeric grNum, BigNumeric grDen) = BigNumeric.ParseToFraction(BaseData.RewardMult);
            BigNumeric totalGrNum = BigNumeric.Pow(grNum, levelsToApply);
            BigNumeric totalGrDen = BigNumeric.Pow(grDen, levelsToApply);

            GoldReward.value = (GoldReward.value * totalGrNum) / totalGrDen;
        }
        CurrentLevel = targetLevel;
    }
    #endregion
}
