using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserSaveData
{
    //public float gold;
    //public float soulStone;

    public BigNumericWrapper goldSave;
    public BigNumericWrapper soulstoneSave;

    public BigNumeric gold;
    public BigNumeric soulStone;
    public long diamond;

    public List<EntityData> charSaveDatas;
    public List<ArtifactData> artifactSaveDatas;
    public List<RuneData> runeSaveDatas;

    public List<string> battleCharacterIds; // 전투 참여 캐릭터 ID 목록
    public List<string> formationCharacterIds;

    public List<QuestData> questSaveDatas;
    public List<QuestData> activeQuestSaveDatas;

    public string lastPlayTime;
    public GameResultData ReincarnateData; //환생시 결과 데이터

    public int curAchievementKm; // 현재 이동 거리
    public int maxAchievementKmEver; // 최대 이동 거리(영구)

    public int StageLevel; // 현재 스테이지 레벨
    
    public int nextMidBossKm; // 다음 중간보스 위치
    public int nextBossKm; // 다음 보스 위치
    
    public ChestQueueSaveData chestQueues; // 상자 큐 데이터
    
    public StorePurchaseData storePurchaseData; // 상점 구매 가능 횟수 데이터

    public UserSaveData()
    {
        goldSave = 0;
        soulstoneSave = 5000;
        gold = 0;
        soulStone = 0;
        diamond = 0;
        //laborMult = 1f;
        //huntMult = 5f;
        ReincarnateData = new GameResultData();
        curAchievementKm = 0;
        maxAchievementKmEver = 0;
        StageLevel = 1;
        nextMidBossKm = 10;
        nextBossKm = 30;
        chestQueues = new ChestQueueSaveData();
        storePurchaseData = new StorePurchaseData();
    }
}

[Serializable]
public class User : Singleton<User>
{
    [Header("재화")]
    public BigNumericWrapper goldString = 0; //골드
    public BigNumericWrapper soulstoneString = 0; //환생석
    public BigNumeric gold => goldString.value;
    public BigNumeric soulStone => soulstoneString.value;

    public long diamond = 0; //다이아

    public GameResultData ReincarnateData = new GameResultData(); //환생시 결과 데이터

    private float laborMult = 1f;
    private float huntMult = 5f;

    [Header("인벤토리")] //로그라이크 데이터
    public ArtifactInventory artifactInven; //유물

    [Header("수집목록")] //로그라이트 데이터
    public CharInventory charInven;
    public RuneInventory runeInven; //룬 인벤토리

    //[Header("유저 데이터")]
    public Dictionary<string, Character> battleCharacterDict = new(); //전투에 참여중인 캐릭터들

    [Header("디버그용")]
    public bool debugmode;

    private int cachedAchievementKm;
    private int cachedNextBossKm = 30;
    private int cachedNextMidBossKm = 10;

    public event Action OnGoodsChanged; // 재화 변경 이벤트 발행

    public float LaborMult => laborMult;
    public float HuntMult => huntMult;

    public int CurAchievementKm
    {
        get
        {
            if (BattleManager.Instance != null && BattleManager.Instance.teamPos != null)
            {
                cachedAchievementKm = Mathf.RoundToInt(BattleManager.Instance.teamPos.position.x / 3f);
            }
            return cachedAchievementKm;
        }
    }

    public int CachedNextBossKm => cachedNextBossKm;
    public int CachedNextMidBossKm => cachedNextMidBossKm;

    public void CacheNextBossKm(int km)
    {
        cachedNextBossKm = Mathf.Max(0, km);
    }

    public void CacheNextMidBossKm(int km)
    {
        cachedNextMidBossKm = Mathf.Max(0, km);
    }

    private void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.StartListening(EventType.StartReincarnate, SaveSoulStoneBeforeReincarnate);
            EventManager.Instance.StartListening(EventType.EndReincarnate, OnReincarnate);
        }
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.StopListening(EventType.StartReincarnate, SaveSoulStoneBeforeReincarnate);
            EventManager.Instance.StopListening(EventType.EndReincarnate, OnReincarnate);
        }
    }

    // 이동거리 표시 로직

    public int maxAchievementKmEver = 0; // 최대 이동 거리(영구저장)
    public event Action OnAchievementKmChanged;
    private int lastAchievementKm = -1; // 거리 체크 최적화, 초기화용 변수

    private int t => Math.Max(0, (ReincarnateData.MaxDistance / 10) * 10); // 10단위 내림, 최소 0

    public int soulStone_will_receved
        => t == 0 ? 0 : (int)Math.Round((0.1 * (double)t * (double)t + t + 80.0) * 3.5); // 영혼석 환생 보상 계산식, 0KM 일때는 0 #유저테스트용으로 3.5배 적용


    private void UpdateArchivmentKm(int curDistance) // 이동 거리 기록
    {
        ReincarnateData.MaxDistance = Mathf.Max(ReincarnateData.MaxDistance, curDistance); // 현재 게임에서의 최대 거리, 현재 이동 거리
        maxAchievementKmEver = Mathf.Max(maxAchievementKmEver, curDistance); // 영구 최대 거리
        OnAchievementKmChanged?.Invoke();
    }
    
    private void Update()
    {
        int currentKm = CurAchievementKm;
        if (currentKm != lastAchievementKm) // 거리 변화 발생시 업데이트 실행
        {
            UpdateArchivmentKm(currentKm); // 업데이트
            lastAchievementKm = currentKm; // 마지막 이동 거리 저장
        }
        
        if (debugmode)
        {
            if (Input.GetKey(KeyCode.F1))
            {
                AddGold(100000f);
                AddDiamond(100000);
                AddSoulStone(100000f);
            }
        }
    }

    public void AddGold(BigNumeric amount)
    {
        if (debugmode) Debug.Log($"{amount}G 획득");
        User.Instance.ReincarnateData.goldEarned += amount;
        goldString.value += amount;
        OnGoodsChanged?.Invoke();
    }

    public bool UseGold(BigNumeric amount)
    {
        if (gold < amount)
        {
            if(debugmode) Debug.Log("재화가 부족합니다.");

            return false;
        }

        goldString.value -= amount;
        OnGoodsChanged?.Invoke();
        return true;
    }

    public void AddSoulStone(BigNumeric amount)
    {
        if (debugmode) Debug.Log($"{amount}스톤 획득");
        soulstoneString.value += amount;
        OnGoodsChanged?.Invoke();
    }

    public bool UseSoulStone(BigNumeric amount)
    {
        if(soulStone < amount)
        {
            if (debugmode) Debug.Log("스톤이 부족합니다.");

            return false;
        }

        soulstoneString.value -= amount;
        OnGoodsChanged?.Invoke();
        return true;
    }
    public void AddDiamond(int amount)
    {
        if (debugmode) Debug.Log($"{amount}다이아 획득");
        diamond += amount;
        OnGoodsChanged?.Invoke();
    }

    public bool UseDiamond(int amount)
    {
        if(diamond < amount)
        {
            if (debugmode) Debug.Log("다이아가 부족합니다.");

            return false;
        }

        diamond -= amount;
        OnGoodsChanged?.Invoke();
        return true;
    }

    // 환생 직전에 영혼석 저장
    public void SaveSoulStoneBeforeReincarnate()
    {
        ReincarnateData.soulStoneEarned = soulStone_will_receved;
    }

    public void OnReincarnate()
    {
        Debug.Log("User OnReincarnate 함수 실행.");
        ArtifactEffectManager.Instance.RemoveAllEffects();

        //AddSoulStone(ReincarnateData.soulStoneEarned.ToFloat()); // 영혼석 보상 지급
        AddSoulStone(ReincarnateData.soulStoneEarned.ToFloat()); // 영혼석 보상 지급
        goldString.value = 0; // 골드 초기화

        foreach (var ch in charInven.SaveCharacters)
        {
            ch.ResetForNewRun(CharacterManager.Instance.GetCharacterSO(ch.id).BaseStat.Value);
        }
        
        ReincarnateData.reset();
        
        GameManager.Instance.StageLevel = 1; // 스테이지 레벨 초기화
        
        if (BattleManager.Instance != null && BattleManager.Instance.teamPos != null)
        {
            BattleManager.Instance.teamPos.position = new Vector3(0, BattleManager.Instance.teamPos.position.y, BattleManager.Instance.teamPos.position.z);
        }
        
        lastAchievementKm = -1; // 이동거리 초기화
        cachedAchievementKm = 0;
        CacheNextBossKm(30);
        CacheNextMidBossKm(10);
        UpdateArchivmentKm(CurAchievementKm); // 이동거리 업데이트
        
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.SetNextMidBossKm(10); 
            EnemyManager.Instance.SetNextBossKm(30);
        }
        
        ToSaveData();
    }

    public UserSaveData ToSaveData()
    {
        //ArtifactEffectManager.Instance.RemoveAllEffects();

        charInven.SaveCharactersToData();

        
        var allQuests = new List<QuestData>();
        var activeQuests = new List<QuestData>();
        if (QuestManager.Instance != null)
        {
            (allQuests, activeQuests) = QuestManager.Instance.ToSaveData();
        }
           

        // 전투 참여 캐릭터 ID 목록 저장
        var battleIds = new List<string>(battleCharacterDict.Keys);

        var confirmedFormation = FormationManager.Instance?.GetConfirmFormation();
        var formationIds = new List<String>();
        if (confirmedFormation != null)
        {
            foreach(var data in confirmedFormation)
            {
                formationIds.Add(data?.CharData?.id);
            }
        }

        // 유물, 룬 데이터 저장
        var artifactDatas = new List<ArtifactData>();
        if (artifactInven != null && artifactInven.OwnedArtifact != null)
            artifactDatas.AddRange(artifactInven.OwnedArtifact);

        var runeDatas = new List<RuneData>();
        if (runeInven != null && runeInven.OwnedRunes != null)
            runeDatas.AddRange(runeInven.OwnedRunes);

        int currentKm = CurAchievementKm;
        if (ReincarnateData != null)
        {
            ReincarnateData.MaxDistance = Mathf.Max(ReincarnateData.MaxDistance, currentKm);
            ReincarnateData.UpdateCount();
        }
        maxAchievementKmEver = Mathf.Max(maxAchievementKmEver, currentKm);

        if (EnemyManager.Instance != null)
        {
            CacheNextBossKm(EnemyManager.Instance.NextBossKm);
            CacheNextMidBossKm(EnemyManager.Instance.NextMidBossKm);
        }
        
        return new UserSaveData
        {
            goldSave = gold,
            soulstoneSave = soulStone,
            diamond = diamond,

            //laborMult = laborMult,
            //huntMult = huntMult,


            charSaveDatas = charInven?.SaveCharacters ?? new List<EntityData>(),
            artifactSaveDatas = artifactDatas,
            runeSaveDatas = runeDatas,
            battleCharacterIds = battleIds,
            formationCharacterIds = formationIds,

            questSaveDatas = allQuests,
            activeQuestSaveDatas = activeQuests,

            lastPlayTime = DateTime.Now.ToBinary().ToString(),

            ReincarnateData = this.ReincarnateData ?? new GameResultData(),

            curAchievementKm = currentKm,
            maxAchievementKmEver = maxAchievementKmEver,
            StageLevel = (GameManager.Instance != null) ? GameManager.Instance.StageLevel : 1,
            nextMidBossKm = cachedNextMidBossKm,
            nextBossKm = cachedNextBossKm,
            chestQueues = (ArtifactManager.Instance != null) 
                ? ArtifactManager.Instance.ToSaveData() 
                : new ChestQueueSaveData(),
            
            storePurchaseData = (StoreManager.Instance != null) 
                ? StoreManager.Instance.ToSaveData() 
                : new StorePurchaseData(),
        };
    }

    public void LoadFromSaveData(UserSaveData data)
    {
        SaveManager.Instance.RemoveInvalidData<ArtifactData, ArtifactSO>(data.artifactSaveDatas);
        SaveManager.Instance.RemoveInvalidData<RuneData, RuneSO>(data.runeSaveDatas);
        SaveManager.Instance.RemoveInvalidData<QuestData, QuestSO>(data.questSaveDatas);
        SaveManager.Instance.RemoveInvalidData<QuestData, QuestSO>(data.activeQuestSaveDatas);

        goldString = data.goldSave;
        soulstoneString = data.soulstoneSave;
        diamond = data.diamond;

        //laborMult = data.laborMult;
        //huntMult = data.huntMult;

        ReincarnateData = data.ReincarnateData;
        ReincarnateData.SetStandard();

        charInven.LoadCharacters(data.charSaveDatas);
        
    maxAchievementKmEver = data.maxAchievementKmEver;
    cachedAchievementKm = data.curAchievementKm;
    CacheNextBossKm(data.nextBossKm);
    CacheNextMidBossKm(data.nextMidBossKm);

        GameManager.Instance.StageLevel = data.StageLevel;
        
        // 중간보스 스폰 카운트 복원 - teamPos 설정 후로 이동
        if (BattleManager.Instance != null && BattleManager.Instance.teamPos != null)
        {
            BattleManager.Instance.teamPos.position = new Vector3(data.curAchievementKm * 3, BattleManager.Instance.teamPos.position.y, BattleManager.Instance.teamPos.position.z);
        }
        // 중간보스/보스 위치 복원 - teamPos 설정 후 호출
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.SetNextMidBossKm(data.nextMidBossKm);
            EnemyManager.Instance.SetNextBossKm(data.nextBossKm);
        }

        // 유물, 룬 데이터 복원
        if (artifactInven != null && data.artifactSaveDatas != null)
        {
            artifactInven.Init(data.artifactSaveDatas);
        }

        if (runeInven != null && data.runeSaveDatas != null)
        {
            runeInven.Init(data.runeSaveDatas);
        }

        // 전투 참여 캐릭터 복원
        battleCharacterDict.Clear();
        if (data.battleCharacterIds != null)
        {
            foreach (var id in data.battleCharacterIds)
            {
                if (charInven.CharSaved.TryGetValue(id, out var character))
                    battleCharacterDict[id] = character;
            }
        }

        // 편성표 복원
        if (data.formationCharacterIds != null && charInven != null)
        {
            var loadedFormationData = new List<EntityData>();
            foreach (var id in data.formationCharacterIds)
            {
                if (string.IsNullOrEmpty(id))
                {
                    loadedFormationData.Add(null); // 빈 슬롯
                }
                else
                {
                    var charData = charInven.SaveCharacters.Find(c => c.id == id);
                    loadedFormationData.Add(charData);
                }
            }
            var ui = UIManager.Instance.GetUI<UIMain>();
            if (ui != null && ui.EnhancePanel != null)
            {
                ui.EnhancePanel.SetFormation(loadedFormationData);
            }
            // BattleManager.Instance.teamPos.position = new Vector3(data.curAchievementKm * 3, BattleManager.Instance.teamPos.position.y, BattleManager.Instance.teamPos.position.z);
            FormationManager.Instance.LoadFromSaveData(loadedFormationData);
            // FormationManager.Instance.SpawnField();
        }

        QuestManager.Instance.LoadFromSaveData(data.questSaveDatas, data.activeQuestSaveDatas);
        
        // 상자 큐 복원
        if (ArtifactManager.Instance != null && data.chestQueues != null)
        {
            Debug.Log($"[User] ArtifactManager 상자 로드 호출 - 일반: {data.chestQueues.normalChestCount}, 특별: {data.chestQueues.specialChestCount}");
            ArtifactManager.Instance.LoadFromSaveData(data.chestQueues);
        }
        else
        {
            Debug.LogWarning($"[User] 상자 로드 실패 - ArtifactManager null? {ArtifactManager.Instance == null}, chestQueues null? {data.chestQueues == null}");
        }
        
        //추가
        //실행순서 주기때문에 여기서 실행
        ArtifactEffectManager.Instance.ApplyAllArtifactsToAllCharacters();

        lastAchievementKm = -1;
        UpdateArchivmentKm(CurAchievementKm);

        if (StoreManager.Instance != null)
        {
            StoreManager.Instance.LoadFromSaveData(data.storePurchaseData ?? new StorePurchaseData());
        }
    }

    public void GetIdleReward(UserSaveData data)
    {
        if(!string.IsNullOrEmpty(data.lastPlayTime))
        {
            //정밀하게 저장하기위해 long, double 사용
            long binary = Convert.ToInt64(data.lastPlayTime);
            DateTime lastTime = DateTime.FromBinary(binary);
            TimeSpan elapsed = DateTime.Now - lastTime;

            double seconds = elapsed.TotalSeconds;
            double reward = seconds * laborMult * (data.maxAchievementKmEver * 10);

            AddGold((float)reward);
            QuestManager.Instance.ApplyOfflineProgress((float)seconds);

            Debug.Log($"{seconds}동안 방치");

        }
    }

    public void ResetLaborAndHuntMult()
    {
        laborMult = 1f;
        huntMult = 5f;
    }

    public void AddLaborMult(float value)
    {
        laborMult = 1f;
        laborMult += value;
    }

    public void AddHuntMult(float value)
    {
        huntMult = 5f;
        huntMult += value;
    }

    // private void OnApplicationQuit()
    // {
    //     if(SaveManager.Instance != null)
    //         SaveManager.Instance.SaveUser();
    // }
}