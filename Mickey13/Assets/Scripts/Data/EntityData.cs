using System;
using UnityEngine;
using System.Numerics;

[Serializable]
public class EntityData : ISlotUIData, IInventoryData, IUpgradeable
{
    public EntitySO Definition;

    private StatModel finalStat; // 캐싱용
    private bool isStatDirty = true; // 더티플래그
    
    public EntityData() { }
    public EntityData(CharacterSO so)
    {
        id = so.ID;
        name = so.name;
        Definition = so;
        Init(so.BaseStat.Value);
    }

    public EntityData(EntitySO so)
    {
        id = so.ID;
        name = so.name;
        Definition = so;
        Init(so.BaseStat.Value);
    }

    public string id;
    public string name;

    [Header("Runtime Layers")]
    private StatModel metaPermanent = StatModel.Zero(); // 영구 성장(세이브 유지)
    [SerializeField] private StatModel runProgress = StatModel.Zero(); // 이번 회차 누적
    private StatModel artifacts = StatModel.One(); // 유물
    private StatModel buffs = StatModel.One(); // 일시 버프 => 나중에
    [SerializeField] private StatModel rune = StatModel.One(); // 룬

    public StatModel MetaPermanent { get => metaPermanent; set => metaPermanent = value; }
    public StatModel Rune { get => rune; set => rune = value; }
    public StatModel RunProgress { get => runProgress; set => runProgress = value; }
    public StatModel Artifacts { get => artifacts; set => artifacts = value; }
    public StatModel Buffs { get => buffs; set => buffs = value; }

    [Header("레벨,")]
    [SerializeField] public int level = 1;
    [SerializeField] public BigNumericWrapper curHP = 0; //기본적으로 풀피, 세이브 불러올때 변경

    //환생재화
    [Header("환생재화")]
    [SerializeField] public int pieceOfMemory = 0; //기억의조각

    public BigNumericWrapper MaxHP => FinalStat.HP;

    // 최종 스탯: 합산만(성장은 별도 시스템에서 Base에 반영된 상태라고 가정)
    //public StatModel FinalStat => metaPermanent + runProgress + artifacts + buffs + rune;
    //public StatModel FinalStat => ((metaPermanent + runProgress) * artifacts * rune * buffs * 0.000001f).ClampMinZero();

    public StatModel FinalStat
    {
        get
        {
            if (!isStatDirty)
            {
                return finalStat;
            }
            
            float atkCool = (metaPermanent + runProgress).AtkCooldown;
            atkCool /= (artifacts.AtkCooldown / 100f);
            atkCool /= (rune.AtkCooldown / 100f);
            atkCool /= (buffs.AtkCooldown / 100f);

            StatModel result = ((metaPermanent + runProgress) * artifacts * rune * buffs * 0.000001f).ClampMinZero();

            result.AtkCooldown = atkCool;
            
            finalStat = result;
            isStatDirty = false;

            return result;
        }
    }
    
    public void SetStatDirty()
    {
        isStatDirty = true;
    }

    public string ID => id;

    public string Name => name;
    public RarityEnum Rarity => Definition.Rarity;

    //임시 캐릭터 등급
    public int Grade { get; private set; }

    string IInventoryData.ID => id;

    //public event Action OnLevelChanged; // 레벨 변경 이벤트
    public event Action<BigNumeric, BigNumeric> OnHpChanged; // Hp 변경 이벤트

    //추후 중복으로 뽑은횟수 + 몇성인지 까지 저장해야할 생각
    //###임시
    public void UpdateFromCharacter(EntityData ch)
    {
        metaPermanent = ch.MetaPermanent.Clone();
        rune = ch.Rune.Clone();
        runProgress = ch.RunProgress.Clone();
        artifacts = ch.Artifacts.Clone();
        buffs = ch.Buffs.Clone();

        curHP = ch.curHP;
        level = ch.level;
    }

    public void ApplyToCharacter(Character ch)
    {
        ch.Data = this;
        ch.Definition = this.Definition;

        ch.Data.SetHP(curHP);
        ch.Data.SetLevel(level);
    }

    //최초 초기화
    public void Init(StatModel BaseStat)
    {
        metaPermanent = StatModel.Zero();
        runProgress = BaseStat.Clone();
        artifacts = StatModel.One();
        buffs = StatModel.One();
        rune = StatModel.One();

        curHP = MaxHP; // 수정필요한 부분
        level = 1;
    }

    // 회차 리셋, 환생
    public void ResetForNewRun(StatModel BaseStat)
    {
        runProgress = BaseStat.Clone();
        artifacts = StatModel.One();
        buffs = StatModel.One();
        //룬, 영구성장은 유지
        curHP = MaxHP;
        level = 1;
    }

    public static EntityData Convert(Character ch)
    {
        var data = new EntityData
        {
            id = ch.Definition.ID,
            name = ch.Definition.name,
            metaPermanent = ch.Data.MetaPermanent.Clone(),
            rune = ch.Data.Rune.Clone(),
            runProgress = ch.Data.RunProgress.Clone(),
            artifacts = ch.Data.Artifacts.Clone(),
            buffs = ch.Data.Buffs.Clone(),
            curHP = ch.Data.curHP,
            level = ch.Data.level,

            Definition = ch.Data.Definition,
        };
        return data;
    }

    public void SetLevel(int newLevel)
    {
        var characterSO = CharacterSO.TryParse(Definition);
        var enemySO = EnemySO.TryParse(Definition);

        Debug.Log("SetLevel 호출");
        Debug.Log($"Level Change: {level}  -> {newLevel}");
        
        if (newLevel < 1)
        {
            Debug.Log("Level 값이 1 미만으로 설정되었습니다.");
            newLevel = 1;
        }

        int levelDiff = newLevel - level;

        // 레벨 차이가 없거나 0 이하면 연산 중지
        if (levelDiff <= 0)
        {
            level = Mathf.Max(1, newLevel);
            return;
        }
        level = Mathf.Max(1, newLevel); // 레벨 적용

        if (Definition != null)
        {
            float hpMultiplier = 1.0f;
            float atkMultiplier = 1.0f;
            
            if (characterSO != null)
            {
                hpMultiplier = characterSO.LevelUpHP;
                atkMultiplier = characterSO.LevelUpATK;

            }
            else if (enemySO != null)
            {
                hpMultiplier = enemySO.LevelUpHP;
                atkMultiplier = enemySO.LevelUpATK;

            }
            else
            {
                Debug.LogError("EntitySO가 CharacterSO나 EnemySO로 캐스팅되지 않았습니다.");
                return;
            }

            // HP 총 배율 계산
            (BigNumeric hpNum, BigNumeric hpDen) = BigNumeric.ParseToFraction(hpMultiplier);
            BigNumeric totalHpNum = BigNumeric.Pow(hpNum, levelDiff);
            BigNumeric totalHpDen = BigNumeric.Pow(hpDen, levelDiff);
            
            // Atk 총 배율 계산
            (BigNumeric atkNum, BigNumeric atkDen) = BigNumeric.ParseToFraction(atkMultiplier);
            BigNumeric totalAtkNum = BigNumeric.Pow(atkNum, levelDiff);
            BigNumeric totalAtkDen = BigNumeric.Pow(atkDen, levelDiff);
            
            // 최종 스탯 적용
            RunProgress.HP.value = (RunProgress.HP.value * totalHpNum) / totalHpDen;
            RunProgress.Atk.value = (RunProgress.Atk.value * totalAtkNum) / totalAtkDen;
        }

        if (level > User.Instance.ReincarnateData.MaxLevel)
        {
            User.Instance.ReincarnateData.MaxLevel = level;
        }
        
        SetStatDirty();

        BigNumeric nexMaxHp = FinalStat.HP;
        BigNumeric healAmount = (nexMaxHp * 30) / 100;
        
        curHP += healAmount;
        if (curHP > nexMaxHp)
        {
            curHP = nexMaxHp;
        }
    }

    public void SetHP(BigNumeric value)
    {
        BigNumeric oldHp = curHP;
        curHP = BigNumeric.Clamp(value, 0, MaxHP);

        if (oldHp != curHP)
            OnHpChanged?.Invoke(curHP, MaxHP);
    }

    public Sprite GetSprite(SlotImageType imageType)
    {
        return Definition.Sprite;
    }

    public void OnRespawn()
    {
        curHP = FinalStat.HP;
    }

    #region 업그레이드

    public int currentLevel => level;
    
    private BigNumeric CalculateSingleCost(int targetLevel)
    {
        var manager = UpgradeManager.Instance;
        if (manager == null) return new BigNumeric(0);
        
        // 공비(Multiplier)를 분수로 가져오기
        (BigNumeric numerator, BigNumeric denominator) = BigNumeric.ParseToFraction(manager.CostMultiplier);
        
        // 기본 비용
        BigNumeric baseCost = manager.BaseCost;
        
        // 지수(exponent) 계산
        // 1 -> 2 비용 targetLevel = 1은 지수가 0 이어야함
        // 2 -> 3 비용 targetLevel = 2는 지수가 1 이어야함
        int exponent = targetLevel - 1;
        
        if(exponent < 0) exponent = 0; // 음수 방지
        
        // (Multipler^exponent) 계산
        BigNumeric powNum = BigNumeric.Pow(numerator,exponent); 
        BigNumeric powDen = BigNumeric.Pow(denominator,exponent);
        
        // 최종 비용 = BaseCost * (Multiplier^exponent)
        // (baseCost * (numerator^exponent)) / (denominator^exponent)
        return (baseCost * powNum) /  powDen;
    }
    
    public BigNumeric CalculateTotalCost(int levelsToUpgrade)
    {
        if (levelsToUpgrade <= 0) return new BigNumeric(0);

        var manager = UpgradeManager.Instance;
        if (manager == null) return new BigNumeric(0);

        // 공비(r)를 분수 (num / den)로 가져옴
        (BigNumeric num, BigNumeric den) = BigNumeric.ParseToFraction(manager.CostMultiplier);

        // 초항(C) 계산(앞으로 낼 첫 번째 강화 비용)
        BigNumeric C = CalculateSingleCost(this.level); 

        // 공비(r)가 1인 경우 (분모가 0이 됨) (공비 : 공통된 비율(곱해지는 값) == 강화배율)
        if ((num - den) == new BigNumeric(0))
        {
            // (1회 비용 * 횟수)를 반환
            return C * levelsToUpgrade;
        }

        // 4. 등비수열의 합 공식: S_n = C * ( (r^n) - 1 ) / ( r - 1 )
        // r = num / den
        // n = levelsToUpgrade

        // (r^n) 계산
        BigNumeric r_pow_n_num = BigNumeric.Pow(num, levelsToUpgrade); // num^n
        BigNumeric r_pow_n_den = BigNumeric.Pow(den, levelsToUpgrade); // den^n

        // (r^n - 1) 계산 (통분)
        // (num^n / den^n) - 1 = (num^n - den^n) / den^n
        BigNumeric numerator_part1 = r_pow_n_num - r_pow_n_den;
        BigNumeric denominator_part1 = r_pow_n_den;

        // (r - 1) 계산 (통분)
        // (num / den) - 1 = (num - den) / den
        BigNumeric numerator_part2 = num - den;
        BigNumeric denominator_part2 = den;

        // ( (r^n) - 1 ) / ( r - 1 ) 계산 (분수 나눗셈)
        // [ part1 ] / [ part2 ] = part1 * (1 / part2)
        // = (num^n - den^n) * den / ( den^n * (num - den) )
        BigNumeric final_numerator = numerator_part1 * denominator_part2;
        BigNumeric final_denominator = denominator_part1 * numerator_part2;

        if ((num - den) == new BigNumeric(0))
        {
            Debug.LogError("CalculateTotalCost: Division by zero.");
            return C * levelsToUpgrade; // r=1일때의 로직으로 대체
        }
        
        // 최종 합: C * [ ... ]
        // (정밀도를 위해 곱셈을 먼저)
        return (C * final_numerator) / final_denominator;
    }
    
    public int CalculateMaxAffordableLevel()
    {
        var manager = UpgradeManager.Instance;
        if (manager == null) return 0;

        BigNumeric currentGold = User.Instance.gold;

        // 초항(C): 현재 레벨에서 다음 레벨(level + 1)로 가는 1회 비용
        BigNumeric C = CalculateSingleCost(this.level);
        
        // 공비(r)
        float r_float = manager.CostMultiplier;

        // --- 1. 공비(r)가 1인 경우 (등차수열) ---
        if (Math.Abs(r_float - 1.0f) < 0.0001f)
        {
            if (C.number == 0) return 500000; // 비용이 0이면 무한 (최대 레벨 반환)
            try 
            {
                // n = Gold / C
                return (int)(currentGold / C).number; 
            }
            catch (OverflowException) 
            { 
                // int 범위를 초과하면 최대 레벨 반환
                return 500000; 
            }
        }

        // --- 2. 공비(r)가 1이 아닌 경우 (등비수열 로그 공식) ---
        // 목표: n = log_r( (Gold * (r - 1) / C) + 1 )

        // K = ( (Gold * (r - 1)) / C ) + 1
        float r_minus_1_float = r_float - 1.0f;
        
        // (r-1)을 BigNumeric의 분수 형태로 변환
        (BigNumeric r_m1_num, BigNumeric r_m1_den) = BigNumeric.ParseToFraction(r_minus_1_float);

        // 0으로 나누기 방지
        if (r_m1_den.number == 0 || C.number == 0) return 0; 

        // K = ( (Gold * r_m1_num) + (C * r_m1_den) ) / (C * r_m1_den)
        BigNumeric K_numerator = (currentGold * r_m1_num) + (C * r_m1_den);
        BigNumeric K_denominator = C * r_m1_den;

        // 0으로 나누기 또는 log(0) 방지
        if (K_denominator.number == 0) return 0;
        
        // K가 0 또는 음수이면 log 계산 불가 (업그레이드 불가)
        if (K_numerator.number <= 0) return 0;

        // n = log(K) / log(r)  (자연로그 ln 사용)
        
        // log(K) = log(K_num / K_den) = log(K_num) - log(K_den)
        double logK = BigInteger.Log(K_numerator.number) - BigInteger.Log(K_denominator.number);
        
        // log(r)
        double logR = Math.Log(r_float); // float이므로 Math.Log (자연로그) 사용

        if (Math.Abs(logR) < 0.000001) // r=1인 경우 (안전장치)
        {
            if (C.number == 0) return 500000;
            try { return (int)(currentGold / C).number; }
            catch (OverflowException) { return 500000; }
        }

        int n = (int)Math.Floor(logK / logR);

        if (n < 0) return 0; // 계산 결과가 음수면 0 반환

        // --- 3. 보정식 (로그 근사값 보정) ---
        // 부동 소수점 오차로 n이 1 작게 계산될 수 있으므로
        // n+1 레벨의 비용을 '단 한 번만' 계산해서 확인
        // (CalculateTotalCost는 이미 있으니 그대로 사용)
        if (CalculateTotalCost(n + 1) <= currentGold)
        {
            return n + 1;
        }
        else
        {
            return n;
        }
    }

    public void ApplyUpgrade(int levelsToUpgrade)
    {
        SetLevel(level + levelsToUpgrade);
        EventManager.Instance.TriggerEvent(EventType.CharacterStatChanged);
        EventManager.Instance.TriggerEvent(EventType.UpdateCharacterToInventory);
    }

    #endregion
    
}