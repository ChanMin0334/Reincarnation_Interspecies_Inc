using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField] private Transform SpawnPoint;
    private BoxCollider2D spawnArea;
    [SerializeField] private float spawnInterval = 2f; // 스폰 간격(초)

    // 스테이지별 적 프리팹 관리
    private Dictionary<int, List<Enemy>> stageEnemyDict = new();
    private Dictionary<string, GameObject> prefabDict;

    [SerializeField] private List<Boss> bossPrefabs = new();
    private Dictionary<string, GameObject> bossPrefabDict;

    [SerializeField] private List<Enemy> midBossPrefabs = new();
    private Dictionary<string, GameObject> midBossPrefabDict;

    // 현재 필드에 소환된 적들
    private List<Enemy> spawnedEnemy = new();
    // 현재 필드에 소환된 보스들
    private List<Boss> bosses = new();
    // 현재 필드에 소환된 중간보스들
    private List<Enemy> midBosses = new();

    private float spawnTimer = 2f;

    // 프로퍼티 수정
    public Dictionary<int, List<Enemy>> StageEnemyDict => stageEnemyDict;
    public Dictionary<string, GameObject> PrefabDict => prefabDict;
    public List<Enemy> SpawnedEnemy => spawnedEnemy;

    public bool isCanEnemySpawn = true;

    private HashSet<int> bossSpawnedSections = new HashSet<int>();
    private Coroutine enemySpawnRoutine;
    private Coroutine bossSpawnRoutine;

    private float nextBossX = 90f;
    private int nextBossKm = 30;
    private Boss currentBoss = null;
    private bool bossReady = false;

    private float nextMidBossX = 10f * 3f;
    private int nextMidBossKm = 10;
    private Enemy currentMidBoss = null;
    private bool midBossReady = false;

    // 다음 중간보스/보스 위치 외부 접근용
    public int NextMidBossKm => nextMidBossKm;
    public int NextBossKm => nextBossKm;

    public void SetNextMidBossKm(int km)
    {
        nextMidBossKm = km;
        nextMidBossX = km * 3f;
        if (User.Instance != null)
            User.Instance.CacheNextMidBossKm(nextMidBossKm);
    }
    
    public void SetNextBossKm(int km)
    {
        nextBossKm = km;
        nextBossX = km * 3f;
        if (User.Instance != null)
            User.Instance.CacheNextBossKm(nextBossKm);
    }

    protected override void Awake()
    {
        base.Awake();
        prefabDict = new Dictionary<string, GameObject>();

        // Resources에서 Enemy 프리팹 로드 후 스테이지별로 분류
        var allEnemies = Resources.LoadAll<Enemy>("Prefab/Enemy").ToList();
        stageEnemyDict = new Dictionary<int, List<Enemy>>();

        foreach (var enemy in allEnemies)
        {
            var id = enemy.Id; // 예: EN_01_GOBLIN
            prefabDict[id] = enemy.gameObject;

            var parts = id.Split('_');
            if (parts.Length >= 3 && int.TryParse(parts[1], out int stageNum))
            {
                if (!stageEnemyDict.ContainsKey(stageNum))
                    stageEnemyDict[stageNum] = new List<Enemy>();
                stageEnemyDict[stageNum].Add(enemy);
            }
        }

        bossPrefabDict = new Dictionary<string, GameObject>();

        bossPrefabs = Resources.LoadAll<Boss>("Prefab/EnemyBoss").ToList();
        foreach (var boss in bossPrefabs)
        {
            var id = boss.Id;
            if (!bossPrefabDict.ContainsKey(id))
                bossPrefabDict[id] = boss.gameObject;
        }

        midBossPrefabDict = new Dictionary<string, GameObject>();

        midBossPrefabs = Resources.LoadAll<Enemy>("Prefab/EnemyMiddle").ToList();
        foreach (var midBoss in midBossPrefabs)
        {
            var id = midBoss.Id;
            if (!midBossPrefabDict.ContainsKey(id))
                midBossPrefabDict[id] = midBoss.gameObject;
        }
    }

    private void Start()
    {
        int startSection = Mathf.FloorToInt(SpawnPoint.position.x / 90f);
        bossSpawnedSections.Add(startSection);
        spawnArea = SpawnPoint.GetComponent<BoxCollider2D>();

        // 저장된 진행도 기반으로 다음 스폰 위치 계산
        int currentKm = User.Instance.CurAchievementKm;

        // 다음 보스 위치 (현재 km보다 큰 30의 배수)
        nextBossKm = ((currentKm / 30) + 1) * 30;
        if (User.Instance != null)
            User.Instance.CacheNextBossKm(nextBossKm);

        // 새 게임일 경우에만 nextMidBossKm 계산 (로드 시에는 이미 설정됨)
        if (nextMidBossKm == 10 && currentKm >= 10)
        {
            int tempKm = 0;
            while (tempKm <= currentKm)
            {
                tempKm += 10;
                if (tempKm % 30 == 0)
                {
                    tempKm += 10;
                }
            }
            nextMidBossKm = tempKm;
            if (User.Instance != null)
                User.Instance.CacheNextMidBossKm(nextMidBossKm);
        }

        enemySpawnRoutine = StartCoroutine(EnemySpawnCoroutine());
        bossSpawnRoutine = StartCoroutine(BossSpawnCoroutine());
    }

    private void Update()
    {
        // 필요시 키 입력 등 추가
    }

    private void OnEnable()
    {
        EventManager.Instance.StartListening(EventType.StartReincarnate, StopAllCoroutines);
        EventManager.Instance.StartListening(EventType.Reincarnating, ClearEnemy);
        EventManager.Instance.StartListening(EventType.EndReincarnate, RestartCoroutine);
        EventManager.Instance.StartListening(EventType.AllCharacterDead, ClearEnemy);
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.StopListening(EventType.StartReincarnate, StopAllCoroutines);
            EventManager.Instance.StopListening(EventType.Reincarnating, ClearEnemy);
            EventManager.Instance.StopListening(EventType.EndReincarnate, RestartCoroutine);
            EventManager.Instance.StopListening(EventType.AllCharacterDead, ClearEnemy);
        }
    }

    private void RestartCoroutine()
    {
        GameManager.Instance.StageLevel = 1;
        enemySpawnRoutine = StartCoroutine(EnemySpawnCoroutine());
        bossSpawnRoutine = StartCoroutine(BossSpawnCoroutine());
    }

    private IEnumerator EnemySpawnCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            int currentStage = GameManager.Instance.StageLevel;
            if (isCanEnemySpawn && !GameManager.Instance.isGamePaused && currentBoss?.target == null)
            {
                SpawnEnemyByStage(currentStage);
                yield return new WaitForSeconds(0.2f);
                SpawnEnemyByStage(currentStage);
                yield return new WaitForSeconds(0.2f);
                SpawnEnemyByStage(currentStage);
            }
        }
    }
    private IEnumerator BossSpawnCoroutine()
    {
        while (true)
        {
            yield return null;

            int currentKm = User.Instance.CurAchievementKm;
            int currentStage = GameManager.Instance.StageLevel;
            float playerX = GetPlayerX();

            // === 보스 스폰 (30KM마다) ===
            if (currentBoss == null && !bossReady)
            {
                int bossCount = bossPrefabs.Count;
                if (bossCount > 0)
                {
                    nextBossX = nextBossKm * 3f;
                    int bossIndex = (currentStage - 1) % bossCount;
                    var bossPrefab = bossPrefabs[bossIndex];
                    Vector3 bossPos = new Vector3(nextBossX, SpawnPoint.position.y, 0f);
                    var boss = EntityFactor.BossSpawn(bossPrefab.Data, bossPos);
                    if (boss != null)
                    {
                        bosses.Add(boss);
                        currentBoss = boss;
                        bossReady = true;
                        boss.gameObject.SetActive(false);
                    }
                }
            }

            // 보스 활성화
            if (bossReady && currentBoss != null && !currentBoss.gameObject.activeSelf)
            {
                if (Mathf.Abs(playerX - nextBossX) < 5f)
                {
                    currentBoss.gameObject.SetActive(true);
                    AudioManager.Instance.PlayBGM(BgmType.Boss_3);
                }
            }

            // === 중간보스 스폰 (10KM마다, 30의 배수 제외) ===
            if (currentMidBoss == null && !midBossReady)
            {
                if (nextMidBossKm <= 0)
                {
                    int correctedKm = (currentKm / 10) + 10;
                    EnemyManager.Instance.SetNextMidBossKm(correctedKm);
                    yield return null;
                    continue;
                }

                int midBossCount = midBossPrefabs.Count;
                if (midBossCount > 0)
                {
                    nextMidBossX = nextMidBossKm * 3f;
                    int spawnStage = (nextMidBossKm / 30) + 1;
                    int midBossIndex = (spawnStage - 1) % midBossCount;

                    var midBossPrefab = midBossPrefabs[midBossIndex];
                    Vector3 midBossPos = new Vector3(nextMidBossX, SpawnPoint.position.y, 0f);
                    var midBoss = EntityFactor.MiddleBossSpawn(midBossPrefab.Data, midBossPos);

                    if (midBoss != null)
                    {
                        midBosses.Add(midBoss);
                        currentMidBoss = midBoss;
                        midBossReady = true;
                        midBoss.gameObject.SetActive(false);
                    }
                }
            }

            // 중간보스 활성화
            if (midBossReady && currentMidBoss != null && !currentMidBoss.gameObject.activeSelf)
            {
                if (Mathf.Abs(playerX - nextMidBossX) < 15f)
                {
                    currentMidBoss.canMove = false;
                    currentMidBoss.gameObject.SetActive(true);
                }
            }
        }
    }

    private float GetPlayerX()
    {
        if (BattleManager.Instance.teamPos != null)
            return BattleManager.Instance.teamPos.position.x;
        return 0f;
    }

    // 스테이지별 적 스폰
    private void SpawnEnemyByStage(int stageNum)
    {
        if (SpawnPoint == null)
            return;
        if (spawnedEnemy.Count >= 10)
            return;

        int stageCount = stageEnemyDict.Count;
        if (stageCount == 0)
            return;

        // 순환 스테이지 번호 계산
        int spawnStageNum = ((stageNum - 1) % stageCount) + 1;

        var enemies = GetEnemiesByStage(spawnStageNum);
        if (enemies.Count == 0) return;

        var spawnEnemy = enemies[Random.Range(0, enemies.Count)];
        float yOffset = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y);
        var spawnPosition = new Vector3(SpawnPoint.position.x, yOffset, yOffset / 4);
        var enemy = EntityFactor.EnemySpawn(spawnEnemy.Data, spawnPosition);
        if (enemy != null)
        {
            spawnedEnemy.Add(enemy);
        }
    }

    // 스테이지별 적 리스트 반환
    public List<Enemy> GetEnemiesByStage(int stageNum)
    {
        return stageEnemyDict.TryGetValue(stageNum, out var list) ? list : new List<Enemy>();
    }

    public EnemySO GetEnemySO(string id)
    {
        return prefabDict.TryGetValue(id, out var go) ? go.GetComponent<Enemy>().Definition as EnemySO : null;
    }

    public EnemySO GetBossSO(string id)
    {
        return bossPrefabs.Find(e => e.Id == id).Definition as EnemySO;
    }

    public EnemySO GetMiddleBossSO(string id)
    {
        return midBossPrefabs.Find(e => e.Id == id)?.Definition as EnemySO;
    }

    public GameObject GetPrefab(string id)
    {
        return prefabDict.TryGetValue(id, out var prefab) ? prefab : null;
    }

    public GameObject GetBossPrefab(string id)
    {
        return bossPrefabDict.TryGetValue(id, out var prefab) ? prefab : null;
    }

    public GameObject GetMiddleBossPrefab(string id)
    {
        return midBossPrefabDict.TryGetValue(id, out var prefab) ? prefab : null;
    }

    public List<Enemy> GetAllSpawnedEnemies()
    {
        return spawnedEnemy;
    }

    public void ClearEnemy()
    {
        foreach (var enemy in spawnedEnemy)
        {
            if (enemy != null && enemy.gameObject != null)
                PoolingManager.Instance.Release(enemy.gameObject);
        }
        foreach (var boss in bosses)
        {
            if (boss != null && boss.gameObject != null)
                PoolingManager.Instance.Release(boss.gameObject);
        }
        foreach (var midBoss in midBosses)
        {
            if (midBoss != null && midBoss.gameObject != null)
                PoolingManager.Instance.Release(midBoss.gameObject);
        }
        spawnedEnemy.Clear();
        bosses.Clear();
        midBosses.Clear();

        currentBoss = null;
        bossReady = false;
        // nextBossKm = 30;

        currentMidBoss = null;
        midBossReady = false;
        // nextMidBossKm = 10;
    }

    public void OnDieEnemy(Enemy enemy, EnemyTypeEnum type)
    {
        if (type == EnemyTypeEnum.Boss)
        {
            OnDieBoss((Boss)enemy);
            return;
        }
        if (type == EnemyTypeEnum.MiddleBoss)
        {
            OnDieMidBoss(enemy);
            return;
        }
        if (spawnedEnemy.Contains(enemy))
        {
            spawnedEnemy.Remove(enemy);
        }
    }
    public void OnDieBoss(Boss Boss)
    {
        if (bossReady && currentBoss != null && currentBoss.gameObject.activeSelf && currentBoss.IsDead)
        {
            bosses.Remove(currentBoss);

            currentBoss = null;
            bossReady = false;

            // 다음 보스 위치 계산 (30KM씩 증가)
            nextBossKm += 30;
            if (User.Instance != null)
                User.Instance.CacheNextBossKm(nextBossKm);

            GameManager.Instance.StageLevel++;
            AudioManager.Instance.PlayBGM(BgmType.Stage_2);
        }
    }

    public void OnDieMidBoss(Enemy midBoss)
    {
        if (currentMidBoss != null && currentMidBoss == midBoss)
        {
            if (midBosses.Contains(currentMidBoss))
                midBosses.Remove(currentMidBoss);

            currentMidBoss = null;
            midBossReady = false;

            // 중간보스 사망 시 다음 위치 계산
            int tempNextKm = nextMidBossKm + 10;
            while (tempNextKm % 30 == 0)
            {
                tempNextKm += 10;
            }
            nextMidBossKm = tempNextKm;
            if (User.Instance != null)
                User.Instance.CacheNextMidBossKm(nextMidBossKm);
            EventManager.Instance.TriggerEvent(EventType.MiddleBossKilled);
        }
    }
}
