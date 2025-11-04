using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public enum EntityState
{
    Idle,
    Move,
    Attack,
    Skill,
    Hit,
    Die
}

//todo : 제네릭패턴으로 변경
public class Entity : MonoBehaviour
{
    [Header("Definition")]
    public EntitySO Definition;  // 인스펙터에 연결

    public EntityData Data = new EntityData();

    public Entity lastAttacker; //적을 처치한 캐릭터

    public Transform pivot; //스킬 맞출 스프라이트 Pivot 위치

    protected bool isSpawnProtected = false;
    
    
    // FSM 상태 관리
    public EntityState CurrentState { get; private set; } = EntityState.Idle;
    private Animator animator;
    
    private static readonly int _idleHash = Animator.StringToHash("Idle");
    private static readonly int _runHash = Animator.StringToHash("Run");
    private static readonly int _deathHash = Animator.StringToHash("Death");
    private static readonly int _attackSlashHash = Animator.StringToHash("AttackSlash");
    private static readonly int _attackMagicHash = Animator.StringToHash("AttackMagic");
    private static readonly int _attackShootHash = Animator.StringToHash("AttackShoot");
    private static readonly int _attackPrickHash = Animator.StringToHash("AttackPrick");

    // 베이스 스탯 (성장 반영 전)
    public StatModel BaseFromSO =>
        (Definition != null && Definition.BaseStat != null && Definition.BaseStat.Value != null)
            ? Definition.BaseStat.Value
            : StatModel.Zero();

    // 최종 스탯: 합산만(성장은 별도 시스템에서 Base에 반영된 상태라고 가정)
    public StatModel FinalStat => Data.FinalStat;

    [SerializeField] protected StatModel debugFinalStat; //디버그용
    public StatModel DebugFinalStat => debugFinalStat;
    public string Id => Definition.ID;
    public string Name => Definition.Name;

    //추가

    public bool IsDead => Data.curHP.value <= 0f;

    //public event Action OnLevelChanged; // 레벨 변경 이벤트
    public event Action<BigNumeric, BigNumeric> OnHpChanged; // Hp 변경 이벤트

    public GameObject lastDetected; //마지막으로 감지된 오브젝트
    public Entity target;
    protected float attackCooldownTimer = 0f; // 공격 쿨타임 타이머

    public EntityUI entityUI; //일단은 체력바만

    protected UnityEngine.Vector3 prevPosition; // 이전 프레임 위치 저장용

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        ChangeState(EntityState.Idle);
        prevPosition = transform.position;

        //UI생성
        entityUI = GetComponent<EntityUI>();
        entityUI.UICanvas = Instantiate(entityUI.UIPrefab, transform.position, entityUI.UIPrefab.transform.rotation ,transform);
        entityUI.Init();
    }

    protected virtual void Start()
    {
        if (pivot == null)
        {
            foreach (Transform child in GetComponentsInChildren<Transform>(true))
            {
                if (child.name.Contains("Pivot"))
                {
                    pivot = child;
                    break;
                }
            }
        }

        if (pivot == null)
            pivot = transform;
    }
    
    protected virtual void OnEnable()
    {
        ChangeState(EntityState.Idle);
        
        lastAttacker = null;
        lastDetected = null;
        target = null;
        attackCooldownTimer = 0f;
        attackCount = 0;
        
        prevPosition = transform.position;
        entityUI.Init();
        entityUI.UICanvas.gameObject.SetActive(true);
    }

    protected virtual void Update()
    {
        debugFinalStat = FinalStat; //GC Alloc 발생 원인. 임시 주석 처리

        // 공격 쿨타임 타이머 감소
        if (attackCooldownTimer >= 0f)
            attackCooldownTimer -= Time.deltaTime;

        // 죽었으면 Death 상태로
        if (IsDead)
        {
            if (CurrentState != EntityState.Die)
                ChangeState(EntityState.Die);
            return;
        }

        // 이동 체크
        if (transform.position != prevPosition)
        {
            if (CurrentState != EntityState.Move)
                ChangeState(EntityState.Move);
        }
        else
        {
            if (CurrentState != EntityState.Idle)
                ChangeState(EntityState.Idle);
        }

        prevPosition = transform.position;
    }

    public bool IsAttackReady => attackCooldownTimer <= 0f;

    protected int attackCount = 0;

    public virtual void TryAttack()
    {
        if (target == null) return;
        if (!IsAttackReady) return;

        Attack();
        attackCount++;


        attackCooldownTimer = FinalStat.AtkCooldown > 0f ? FinalStat.AtkCooldown : 1f; // 쿨타임 개념

        //float atkSpeed = Mathf.Max(0.01f, FinalStat.AtkCooldown); //공격속도의 개념으로 변경
        //attackCooldownTimer = 1f / atkSpeed;
    }

    public void LevelUp()
    {
        //레벨 + 1 시키기
        //RunProgress에서 레벨업 로직 작동시키기, EntitySO에서 성장계수 가져와서 적용

        //SetLevel(level + 1);
    }

    public void SetLevel(int Level)
    {
        ////레벨 세팅
        ////레벨 최초 초기화 후 적용하기
        ////RunProgress에서 레벨만큼 로직 작동시키기, EntitySO에서 성장계수 가져와서 적용

        ////###추가
        //level = Mathf.Max(1, Level);
        ////렙업 스탯반영
        ////### 임시 배율 설정: 1%
        //RunProgress.HP = Mathf.Pow(1.01f, level - 1);
        //curHP = MaxHP;
        //RunProgress.Atk = Mathf.Pow(1.01f, level - 1);

        //OnLevelChanged?.Invoke();
    }

    public void SetPrefabsBySO(EntitySO so)
    {
        Data = new EntityData(so);
    }

    public void SetHP(BigNumeric value)
    {
        Data.curHP = BigNumeric.Clamp(value, 0, Data.MaxHP);
        OnHpChanged?.Invoke(Data.curHP, Data.MaxHP);

        // 피격/사망 상태 처리
        if (Data.curHP.value <= 0)
        {
            ChangeState(EntityState.Die);
        }
        else
        {
            ChangeState(EntityState.Hit);
        }
    }

    public void ApplyRespawnHeal()
    {
        Data.curHP = Data.MaxHP;
        OnHpChanged?.Invoke(Data.curHP, Data.MaxHP);
        if (CurrentState == EntityState.Die)
        {
            ChangeState(EntityState.Hit); 
        }
    }

    public virtual void Attack()
    {
        if (target == null) return;
        //float damage = FinalStat.Atk;
        //target.TakeDamage(damage);
        ChangeState(EntityState.Attack);
        DealDamage(target, FinalStat.Atk);
    }

    //public void TakeDamage(float damage)
    //{
    //    SetHP(Data.curHP - damage);
    //}

    //추가
    /// <summary>
    /// 데미지를 주는 함수
    /// </summary>
    /// <param name="target"></param>
    /// <param name="baseDmg"></param>
    /// <param name="isSkill"></param>
    //public void DealDamage(Entity target, float baseDmg, bool isSkill = false)
    public void DealDamage(Entity target, BigNumeric baseDmg, bool isSkill = false)
    {
        if (target == null || IsDead) return;

        DamageData dmg = new DamageData(this, target, baseDmg, isSkill: isSkill);

        EventManager.Instance.TriggerEvent(EventType.BeforeAttack, dmg);

        float critChance = FinalStat.CritChance;

        if(UnityEngine.Random.value <= critChance / 100f)
        {
            dmg.IsCritical = true; 
        }

        if (dmg.IsCritical)
        {
            dmg.Value *= FinalStat.CritMult / 100f;
        }

        dmg.Value *= dmg.Attacker.FinalStat.DamageDealMult / 100f;

        BigNumeric dealt = target.ApplyDamage(dmg);

        //ChangeState(EntityState.Attack); // 데미지를 줄때마다 Attack을 호출하여 Skill 데미지마다 Attack 실행되어 주석


        EventManager.Instance.TriggerEvent(EventType.AfterAttack, dmg);

        Debug.Log($"타입: {target.GetType()}");
        if(target.GetType() == typeof(Enemy)) //환생 가한피해량 
        {
            User.Instance.ReincarnateData.TotalDamage += dealt;
        }
        if (target.IsDead)
            EventManager.Instance.TriggerEvent(EventType.TargetDeath, dmg);
    }

    /// <summary>
    /// 피해량 적용
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public virtual BigNumeric ApplyDamage(DamageData dmg)
    {
        if (IsDead) return 0f;
 
        EventManager.Instance.TriggerEvent(EventType.CalculateDamage, dmg);

        float dmgTakenMult = FinalStat.DamageTakenMult / 100f;
        dmg.Value *= dmgTakenMult;

        BigNumeric final = BigNumeric.Max(0, dmg.Value);
        SetHP(Data.curHP - final);

        if (dmg.Attacker != null) lastAttacker = dmg.Attacker;

        EventManager.Instance.TriggerEvent(EventType.OnDamaged, dmg);

        //치명타 
        if (dmg.IsCritical)
            BattleManager.Instance.CriticalDamagePrefab.Spawn(this.transform.position, dmg.Value.ToString());
        else
            BattleManager.Instance.DamagePrefab.Spawn(this.transform.position, dmg.Value.ToString());

        if (Data.curHP.value <= 0)
            Die();

        return final;
    }

    /// <summary>
    /// 체력회복
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(BigNumeric amount)
    {
        if (IsDead || amount <= 0f) return;

        BattleManager.Instance.HealPrefab.Spawn(this.transform.position, amount.ToString());
        BigNumeric newHp = BigNumeric.Min(Data.curHP + amount, Data.MaxHP);
        SetHP(newHp);

        var healData = new HealData(this, amount);
        EventManager.Instance.TriggerEvent(EventType.OnHeal, healData);
        ChangeState(EntityState.Attack);
        ////Debug.Log($"{Name} attacks {target.Name} for {FinalStat.Atk} damage!");
        //float damage = FinalStat.Atk;
        //target.TakeDamage(damage);
    }

    /// <summary>
    /// 죽을때 호출
    /// </summary>
    protected virtual void Die()
    {
        if (pivot != null)
        {
            foreach (Transform child in pivot)
            {
                if (child.TryGetComponent<EffectController>(out EffectController effectController))
                {
                    effectController?.ReleaseEffect();
                }
            }
        }
        
        Debug.Log($"{Name}은 죽었습니다.");
        ChangeState(EntityState.Die);
        //사망 로직

        EventManager.Instance.TriggerEvent(EventType.OwnerDeath, this);
    }

    public void ChangeState(EntityState newState, AttackType SkillType = AttackType.None)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
        UpdateAnimatorState(SkillType);
    }

    private void UpdateAnimatorState(AttackType _skillType = AttackType.None)
    {
        if (animator == null) return;

        switch (CurrentState)
        {
            case EntityState.Idle:
                animator.SetBool(_idleHash, true);
                animator.SetBool(_runHash, false);
                break;
            case EntityState.Move:
                animator.SetBool(_runHash, true);
                animator.SetBool(_idleHash, false);
                break;
            case EntityState.Die:
                animator.SetTrigger(_deathHash);
                break;
            case EntityState.Attack:
                switch (Definition.attackType)
                {
                    case AttackType.Melee:
                        animator.SetTrigger(_attackSlashHash);
                        break;
                    case AttackType.Magic:
                        animator.SetTrigger(_attackMagicHash);
                        break;
                    case AttackType.Ranged:
                        animator.SetTrigger(_attackShootHash);
                        break;
                    case AttackType.Prod:
                        animator.SetTrigger(_attackPrickHash);
                        break;
                    default:
                        Debug.Log(Definition.attackType + " 공격타입 애니메이션 미설정");
                        break;
                }
                break; // Added break here to fix CS0163  
            case EntityState.Skill:
                switch (_skillType)
                {
                    case AttackType.Melee:
                        animator.SetTrigger(_attackSlashHash);
                        break;
                    case AttackType.Magic:
                        animator.SetTrigger(_attackMagicHash);
                        break;
                    case AttackType.Ranged:
                        animator.SetTrigger(_attackShootHash);
                        break;
                    case AttackType.Prod:
                        animator.SetTrigger(_attackPrickHash);
                        break;
                    case AttackType.None:
                        ChangeState(EntityState.Attack);
                        break;
                }
                break;
        }
    }
    
    public void ActiveSpawnProtection(float duration)
    {
        StopCoroutine(SpawnProtectionCoroutine(duration));
        StartCoroutine(SpawnProtectionCoroutine(duration));
    }
    
    private IEnumerator SpawnProtectionCoroutine(float duration)
    {
        isSpawnProtected = true;
        OnSpawnProtectionStart();
        yield return new WaitForSeconds(duration);
        isSpawnProtected = false;
        OnSpawnProtectionEnd();
    }

    /// <summary>
    /// 무적 시간 시작시 작동
    /// </summary>
    protected virtual void OnSpawnProtectionStart() {}
    /// <summary>
    /// 무적 시간 종료시 작동
    /// </summary>
    protected virtual void OnSpawnProtectionEnd() {}
    
}
