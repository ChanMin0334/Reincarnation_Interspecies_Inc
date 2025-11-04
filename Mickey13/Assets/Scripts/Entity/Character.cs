using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Character : Entity
{
    private SkillInstance activeSkill1;
    private SkillInstance activeSkill2;
    private SkillInstance activeSkill3;
    private SkillInstance passiveSkill;
    
    public SkillInstance ActiveSkill1 => activeSkill1;
    public SkillInstance ActiveSkill2 => activeSkill2;
    public SkillInstance ActiveSkill3 => activeSkill3;
    public SkillInstance PassiveSkill => passiveSkill;

    private bool skillInitialized = false;
    
    private SpriteRenderer characterSprite;
    private Color orignCharacterColor;
    private Tween blinkTween;

    public Character(EntitySO entitySO)
    {
        Definition = entitySO;
    }

    protected override void Awake()
    {
        base.Awake();
        if (characterSprite == null)
            characterSprite = GetComponentInChildren<SpriteRenderer>();

        if (characterSprite != null)
            orignCharacterColor = characterSprite.color;
    }

    public void Init(EntityData _data)
    {
        Debug.Log($"[Character.Init] {Name} 호출됨, GetInstanceID={GetInstanceID()} stack:\n{Environment.StackTrace}");

        Definition = _data.Definition;
        Data = _data;
        InitSkill();
    }
    
    public override BigNumeric ApplyDamage(DamageData dmg)
    {
        if (isSpawnProtected)
            return 0;
        
        return base.ApplyDamage(dmg);
    }
    
    public void InitSkill()
    {
        if (skillInitialized)
        {
            Debug.Log("이미 스킬을 Init했음.");
            return;
        }

        Debug.Log($"{Name} InitSkill 실행 | Definition={(Definition == null ? "null" : Definition.name)}");
        var charSO = Definition as CharacterSO;
        if (charSO == null)
        {
            Debug.LogError($"{Name}: CharacterSO 캐스팅 실패");
            return;
        }

        // var rarity = charSO.Rarity;

        if (charSO.ActiveSkill_1 != null)
            activeSkill1 = new SkillInstance(charSO.ActiveSkill_1, this);

        //if (charSO.ActiveSkill_2 != null && rarity >= RarityEnum.Legendary)
        if (charSO.ActiveSkill_2 != null)
            activeSkill2 = new SkillInstance(charSO.ActiveSkill_2, this);

        if (charSO.ActiveSkill_3 != null)
            activeSkill3 = new SkillInstance(charSO.ActiveSkill_3, this);

        //if (charSO.PassiveSkill != null && rarity >= RarityEnum.Unique)
        if (charSO.PassiveSkill != null)
            passiveSkill = new SkillInstance(charSO.PassiveSkill, this);


        skillInitialized = true;
    }

    //internal bool CheckRange(float distance)
    //{
    //    bool canAttack = distance <= FinalStat.AtkRange;
    //    return canAttack;
    //}

    protected override void Update()
    {
        base.Update();

        if (target != null && !IsDead)
        {
            UseSkill();
            TryAttack();  
        }
    }

    //임시 추가
    public override void TryAttack()
    {
        if (!CharacterTeam.AllCanAttack)
        {
            // Debug.Log($"{Name} : 팀이 아직 사거리 밖이라 공격 불가");
            return;
        }

        if (target == null)
        {
            // Debug.Log("Character : Target이 null입니다.");
            return;
        }
        if (!IsAttackReady)
        {
            // Debug.Log("Character : IsAttackReady가 준비 안됐습니다.");
            return;
        }

        Attack();
        attackCount++;
        attackCooldownTimer = FinalStat.AtkCooldown > 0f ? FinalStat.AtkCooldown : 1f;
    }

    public void UseSkill()
    {
        if (!skillInitialized) return;
        if (IsDead) return;

        if (activeSkill1 != null && activeSkill1.CanUse())
        {
            ChangeState(EntityState.Skill);
            //ChangeState(EntityState.Skill, Skill1.AttackType);
            activeSkill1.UseSkill(target);
            // Debug.Log($"{Name}이 {activeSkill1.Data.Name} 사용!");
            return;
        }

        if (activeSkill2 != null && activeSkill2.CanUse())
        {
            ChangeState(EntityState.Skill);
            activeSkill2.UseSkill(target);
            // Debug.Log($"{Name}이{activeSkill2.Data.Name} 사용!");
            return;
        }

        if (activeSkill3 != null && activeSkill3.CanUse())
        {
            ChangeState(EntityState.Skill);
            activeSkill3.UseSkill(target);
            // Debug.Log($"{Name}이{activeSkill3.Data.Name} 사용!");
            return;
        }
    }

    public void ResetSkillCooldown()
    {
        if (!skillInitialized) return;

        if (activeSkill1 != null)
        {
            activeSkill1.ResetCooldown();
            Debug.Log("ActiveSkill1 이 초기화됨");
        }

        if (activeSkill2 != null)
        {
            activeSkill2.ResetCooldown();
            Debug.Log("ActiveSkill2 이 초기화됨");
        }

        if (activeSkill3 != null)
        {
            activeSkill3.ResetCooldown();
            Debug.Log("ActiveSkill3 이 초기화됨");
        }
    }

    public void OnRespawn()
    {
        // this.gameObject.SetActive(true);
        // SetHP(Data.MaxHP);
        ApplyRespawnHeal();
    }

    protected override void Die()
    {
        base.Die();
        User.Instance.ReincarnateData.DeathCount++;
        EventManager.Instance.TriggerEvent(EventType.CharacterDead);
    }

    #region 캐릭터 스폰 무적 이펙트

    protected override void OnSpawnProtectionStart()
    {
        StartBlinkingEffect();
    }

    protected override void OnSpawnProtectionEnd()
    {
        StopBlinkingEffect();
    }

    private void StartBlinkingEffect()
    {
        if (characterSprite == null) return;
        
        blinkTween?.Kill();
        characterSprite.color = orignCharacterColor;
        
        blinkTween = characterSprite.DOColor(Color.cyan, 0.15f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);;
    }

    private void StopBlinkingEffect()
    {
        if (characterSprite == null) return;
        blinkTween?.Kill();
        characterSprite.color = orignCharacterColor;
    }

    private void OnDisable()
    {
        StopBlinkingEffect();
    }

    #endregion

}