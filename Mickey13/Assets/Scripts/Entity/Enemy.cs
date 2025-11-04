using System.Collections.Generic;
using System;
using UnityEngine;

public class Enemy : Entity
{
    public float rayOffsetX = 0.5f; // Ray 시작 위치의 X 오프셋
    private CharacterTeam characterTeam;
    private int layerMask;

    public LootDropper lootDropper;

    [SerializeField] public bool canMove = true;
    
    private bool hasDied = false; // Die() 중복 호출 방지

    protected override void Awake()
    {
        base.Awake();
        layerMask = 1 << LayerMask.NameToLayer("Team");
        lootDropper = GetComponent<LootDropper>();
        hasDied = false;

        // UI에 있는 Curgold RectTransform 가져오기
        var goldUIObj = GameObject.Find("CurGold");
        RectTransform goldRect = null;
        if (goldUIObj != null)
            goldRect = goldUIObj.GetComponent<RectTransform>();

        // EnemySO로 명시적 형변환
        var enemySO = Definition as EnemySO;
        if (enemySO != null)
        {
            lootDropper.Init(
                enemySO.DropTable,
                this.gameObject.transform,
                goldRect,
                GameManager.Instance.cameraMain,
                LootManager.Instance.CoinPrefab,
                LootManager.Instance.Anchor
            );
        }
        else
        {
            Debug.LogError("Definition이 EnemySO가 아닙니다.");
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        characterTeam = null;
        hasDied = false;
    }
    

    protected override void Update()
    {
        base.Update();
        Vector2 rayStart = transform.position + new Vector3(-rayOffsetX, 0.5f, 0f);
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.left, FinalStat.AtkRange * 0.75f, layerMask);

        if (hit.collider != null)
        {
            if (characterTeam == null)
                characterTeam = hit.collider.GetComponent<CharacterTeam>();

            target = getTarget();

            if (target != null)
            {
                base.TryAttack(); // 쿨타임에 따라 공격
            }
        }
        else
        {
            if(canMove)
                MoveLeft();
        }
        // if(Data.curHP.value <= 0f && !hasDied)
        // {
        //     hasDied = true;
        //     Die();
        // }
    }

    // 왼쪽으로 계속 이동
    private void MoveLeft()
    {
        transform.position += Vector3.left * FinalStat.MoveSpeed * Time.deltaTime;
    }

    // Ray 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 rayStart = transform.position + new Vector3(-rayOffsetX, 0.5f, 0f);
        Gizmos.DrawLine(rayStart, rayStart + Vector3.left * FinalStat.AtkRange);
    }

    private Character getTarget()
    {
        if (characterTeam == null)
            return null;

        // 파티에 있는 캐릭터 리스트를 가져옴
        // var characters = new List<Character>(CharacterManager.Instance.BattleCharacterDict.Values);
        var characters = CharacterManager.Instance.BattleCharacterList;
        if (characters.Count == 0)
            return null;

        // 맨 앞 캐릭터(리스트의 첫 번째)를 2, 나머지는 1의 가중치로 랜덤 선택
        int totalWeight = 1 + (characters.Count - 1); // 기본적으로 1씩
        totalWeight += 1; // 맨 앞 캐릭터는 +1 (즉, 2)

        int rand = UnityEngine.Random.Range(0, totalWeight);
        if (rand < 2)
        {
            // 맨 앞 캐릭터 선택 (확률 2/totalWeight)
            return characters[0];
        }
        else
        {
            // 나머지 캐릭터 중 하나 선택 (확률 1/totalWeight씩)
            int idx = 1 + (rand - 2);
            if (idx < characters.Count)
                return characters[idx];
            else
                return characters[0]; // 예외 방지
        }
    }

    //// todo : 오브젝트 풀링으로 하면 Destroy가 아니라 disable 처리
    //private void OnDestroy()
    //{
    //    EventManager.Instance.TriggerEvent(EventType.EnemyDied, this);
    //    EnemyManager.Instance.OnDieEnemy(this, false);
    //    // 드롭 테이블에 따른 아이템 드롭 처리
    //}

    protected override void Die()
    {
        var dmgData = new DamageData(lastAttacker, this, 0f);
        EventManager.Instance.TriggerEvent(EventType.EnemyDied, dmgData);
        base.Die();
        
        // EnemySO에서 적 타입 가져오기
        var enemySO = Definition as EnemySO;
        var type = enemySO != null ? enemySO.EnemyType : EnemyTypeEnum.Normal;
        
        Debug.Log($"[Enemy] Die 호출: {name}, type={type}");
        EnemyManager.Instance.OnDieEnemy(this, type);
        lootDropper.OnDeath(Data.level);
        User.Instance.ReincarnateData.enemiesDefeated++;
        
        PoolingManager.Instance.Release(this.gameObject);
    }
}
