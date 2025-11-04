using System.Collections.Generic;
using UnityEngine;

public class StatusPanel : MonoBehaviour
{
    [Header("ObjectPool")]
    [SerializeField] GameObject statSlotPrefab; // 캐릭터 스탯슬롯 프리팹
    [SerializeField] Transform statContent; // 스탯 슬롯 생성 위치
    
    [Header("Filterling")]
    [SerializeField] private List<StatCategory> displayCategories = new();
    
    private StatDatabaseSO _statData; // 스탯 정보 데이터
    private CharacterUIData _charData; // 캐릭터 정보(so, EntityData)

    private List<StatSlotUI> _statSlots = new();
    
    bool isInitialized = false;
    
    private void OnEnable()
    {
        if(isInitialized)
            DisplayStats();
    }

    private void OnDisable()
    {
        if(_statData == null || _charData == null) return;
        ClearSlots();
    }

    public void InitStatData(StatDatabaseSO statData, CharacterUIData charData)
    {
        _statData = statData;
        _charData = charData;

        isInitialized = true;
    }
    
    public void DisplayStats() // 대상캐릭터 최종 스탯 출력
    {
        ClearSlots();

        var finalStat = _charData.CharData.FinalStat;
        foreach (var definition in _statData.statDefinitions)
        {
            if(!displayCategories.Contains(definition.category)) continue; // 카테고리 이외의 스텟은 표시 X
            
            var value = GetStatValue(finalStat, definition.type);
            if (value is float FVal)
            {
                var obj = PoolingManager.Instance.Get(statSlotPrefab, statContent);
                var slot = obj.GetComponent<StatSlotUI>();
                slot.Setup(definition, FVal);
                _statSlots.Add(slot);
            }
            else if (value is BigNumericWrapper BVal)
            {
                var obj = PoolingManager.Instance.Get(statSlotPrefab, statContent);
                var slot = obj.GetComponent<StatSlotUI>();
                slot.Setup(definition, BVal);
                _statSlots.Add(slot);
            }
        }
    }
    
    private void ClearSlots()
    {
        foreach (var slot in _statSlots)
        {
            PoolingManager.Instance.Release(slot.gameObject);
        }
        _statSlots.Clear();
    }
    
    private  object GetStatValue(StatModel model, StatType type) //스탯 데이터 추가
    {
        switch(type)
        {
            case StatType.Atk: return model.Atk;
            case StatType.HP: return model.HP;
            case StatType.Atk_Cooldown: return model.AtkCooldown;
            case StatType.Skill_Cooldown: return model.SkillCooldown;
            case StatType.Crit_Chance: return model.CritChance;
            case StatType.Crit_Mult: return model.CritMult;
            case StatType.Atk_Area: return model.AtkArea;
            case StatType.Atk_Range: return model.AtkRange;
            default: return 0f;
        }
    }
}
