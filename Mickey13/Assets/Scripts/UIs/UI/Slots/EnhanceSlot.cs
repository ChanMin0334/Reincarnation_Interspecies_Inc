using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnhanceSlot : SlotBase<EntityData>
{
    [Header("캐릭터 정보")]
    // [SerializeField] SlotUI charSlot; // 캐릭터 슬롯(초상화 대용)
    [SerializeField] CharCardUI charCardSlot; // 캐릭터 슬롯(초상화 대용)
    [SerializeField] TextMeshProUGUI levelText; // 캐릭터 레벨
    [SerializeField] TextMeshProUGUI maxHpText; // 캐릭터 최대Hp
    [SerializeField] TextMeshProUGUI atkText; // 캐릭터 공격력
    [SerializeField] Image hpIcon; 
    [SerializeField] Image atkIcon;

    [Header("강화")]
    [SerializeField] TextMeshProUGUI enhanceCost; // 강화 비용
    [SerializeField] Button enhanceBtn; // 강화 버튼 
    private string currentEnhanceCount;

    [Header("스킬")] 
    [SerializeField] private List<SlotUI> skillSlots = new();

    // public event Action<EnhanceSlot, int> OnEnhanceClicked;
    public event Action<EnhanceSlot> OnIconClicked;

    #region 캐릭터 데이터 연결

    private void OnEnable()
    {
        User.Instance.OnGoodsChanged += UpdateUI; // 임시
        enhanceBtn.onClick.AddListener(OnClickEnhance);
        // charSlot.OnSlotClicked += HandleIconClick;
        charCardSlot.OnClick += HandleIconClick;
        EventManager.Instance.StartListening(EventType.CharacterStatChanged, UpdateUI);
        
        EventManager.Instance.StartListening(EventType.ItemUpgraded, UpdateUI);
        if (UpgradeManager.Instance != null)
        {
            HandleUpgradeCountChange(UpgradeManager.Instance.CurrentUpgradeCount);
        }

        EventManager.Instance.StartListening(EventType.OnChangedUpgradeCount, HandleUpgradeCountChange);
    }

    private void OnDisable()
    {
        User.Instance.OnGoodsChanged -= UpdateUI; // 임시
        enhanceBtn.onClick.RemoveListener(OnClickEnhance);
        // charSlot.OnSlotClicked -= HandleIconClick;
        charCardSlot.OnClick -= HandleIconClick;

        if (EventManager.Instance != null)
        {
            EventManager.Instance.StopListening(EventType.CharacterStatChanged, UpdateUI);
            EventManager.Instance.StopListening(EventType.OnChangedUpgradeCount, HandleUpgradeCountChange);
        }
    }

    public void Setup(EntityData data, Character character) // 캐릭터 정보 및 스텟 세팅
    {
        base.Setup(data);
        var charSO = DataManager.Instance.GetData<CharacterSO>(_data.id);
        var uiData = new CharacterUIData(charSO, _data);

        // charSlot.Setup(uiData);
        charCardSlot.Setup(uiData);
        SetupSkillIcon(charSO, character);
        UpdateUI();
    }

    private void SetupSkillIcon(CharacterSO charSO, Character character)
    {
        if(charSO == null) return;

        var skills = new List<SkillSO>()
        {
            charSO.ActiveSkill_1,
            charSO.ActiveSkill_2,
            charSO.ActiveSkill_3,
            charSO.PassiveSkill
        };

        var skillInstances = new List<SkillInstance>()
        {
            character?.ActiveSkill1, 
            character?.ActiveSkill2, 
            character?.ActiveSkill3, 
            character?.PassiveSkill
        };
        
        for (int i = 0; i < skillSlots.Count; i++)
        {
            var iconSlot = skillSlots[i];
            if(iconSlot == null) continue;
            
            var cooldownDisplay = iconSlot.GetComponent<SkillCooldownDisplay>();

            SkillSO skillSO = (i < skills.Count) ? skills[i] : null;
            SkillInstance skillInstance = (i < skillInstances.Count) ? skillInstances[i] : null;

            if (skillSO != null)
            {
                iconSlot.Setup((ISlotUIData)skillSO);
                iconSlot.gameObject.SetActive(true);
                
                if(cooldownDisplay != null)
                    cooldownDisplay.Setup(skillInstance);
            }
            else
            {
                iconSlot.Clear();
                iconSlot.gameObject.SetActive(false);
                if (cooldownDisplay != null)
                    cooldownDisplay.Clear();
            }
        }
    }

    public override void Clear() // 캐릭터 정보 및 스텟 초기화
    {
        _data = null;
        // charSlot.Clear();
        charCardSlot.Clear();
        levelText.text = "Lv.-";
        atkText.text = "-";
        maxHpText.text = "-";
        enhanceCost.text = "-";

        foreach (var slot in skillSlots)
        {
            slot.Clear();
            slot.gameObject.SetActive(false);
        }
    }

    public void OnClickEnhance()
    {
        if (_data == null) return;
        IUpgradeable item = _data as IUpgradeable;
        if( item == null) return;
        
        int levelsToUpgrade = 0;

        switch (currentEnhanceCount)
        {
            case "x1":
                levelsToUpgrade = 1;
                break;
            case "x10":
                levelsToUpgrade = 10;
                break;
            case "x100":
                levelsToUpgrade = 100;
                break;
            case "Max":
                levelsToUpgrade = item.CalculateMaxAffordableLevel();
                break;
        }
        if(levelsToUpgrade > 0)
            UpgradeManager.Instance.TryUpgrade(item, levelsToUpgrade);
        // OnEnhanceClicked?.Invoke(this,levelsToUpgrade);
    }
    
    public void HandleIconClick(CharCardUI slot)
    {
        if(_data == null) return;
        OnIconClicked?.Invoke(this);
    }

    private void HandleUpgradeCountChange(object data)
    {
        currentEnhanceCount = (string)data;
        SetCost();
    }

    #endregion

    #region 캐릭터 상태 변화

    protected override void UpdateUI()
    {
        if (_data == null) return;

        levelText.text = $"LV.{_data.level}";
        atkText.text = _data.FinalStat.Atk.ToString();
        maxHpText.text = _data.MaxHP.ToString();
        SetCost();
    }
    
    public void SetCost()
    {
        if (_data == null)
        {
            enhanceCost.text = "0";
            enhanceBtn.interactable = false;
            return;
        }
        
        IUpgradeable item = _data;
        if (item == null) return;

        string currentCount = currentEnhanceCount;

        int levelsToUpgrade = 0;
        BigNumeric cost = 0;

        switch (currentCount)
        {
            case "x1":
                levelsToUpgrade = 1;
                break;
            case "x10":
                levelsToUpgrade = 10;
                break;
            case "x100":
                levelsToUpgrade = 100;
                break;
            case "Max":
                levelsToUpgrade = item.CalculateMaxAffordableLevel();
                break;
        }
        
        if (levelsToUpgrade > 0)
        {
            cost = item.CalculateTotalCost(levelsToUpgrade);
        }
        else
        {
            cost = item.CalculateTotalCost(1);
        }
        enhanceCost.text = cost.ToString();
        BigNumeric userMoney = User.Instance.gold;
        enhanceBtn.interactable = (userMoney >= cost) && (levelsToUpgrade >0);
    }

    #endregion
}
