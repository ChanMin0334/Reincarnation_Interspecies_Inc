using System.Collections.Generic;
using UnityEngine;

public class SkillPanel : MonoBehaviour
{
    [Header("ObjectPool")]
    [SerializeField] SkillSlotUI skillSlotPrefab; // 캐릭터 스탯슬롯 프리팹
    [SerializeField] Transform skillContent; // 스탯 슬롯 생성 위치
    
    private List<SkillSlotUI> _skillSlots = new();
    private CharacterUIData _charData;
    
    bool isInitialized = false;

    private void OnEnable()
    {
        if(isInitialized)
            DisplaySkills();
    }

    private void OnDisable()
    {
        if(_charData == null) return;
        ClearSlots();
    }

    public void InitSkillData(CharacterUIData charData)
    {
        _charData = charData;
        isInitialized = true;
    }
    
    private void DisplaySkills() // 대상캐릭터 최종 스탯 출력
    {
        ClearSlots();

        var skillData = _charData.SO;
        
        if(skillData == null || _charData == null) return;

        // 나중에 리스트로 SO를 바꿔서 받아도 될듯?
        if (skillData.PassiveSkill != null)
            CreatSkillSLot(skillData.PassiveSkill);
        if (skillData.ActiveSkill_1 != null)
            CreatSkillSLot(skillData.ActiveSkill_1);
        if (skillData.ActiveSkill_1 != null)
            CreatSkillSLot(skillData.ActiveSkill_2);
        if (skillData.ActiveSkill_1 != null)
            CreatSkillSLot(skillData.ActiveSkill_3);
    }

    private void CreatSkillSLot(SkillSO skillSO)
    {
        int currentLevel = 1; // 추후 스킬 강화 또는 레벨업 기능이 추가된다면 사용(전용 메서드 추가해야할지도?)
        
        var skillData = new SkillUIData(skillSO, currentLevel);
        
        var obj = PoolingManager.Instance.Get(skillSlotPrefab.gameObject, skillContent);
        var slot = obj.GetComponent<SkillSlotUI>();
        slot.Setup(skillData);
        _skillSlots.Add(slot);
    }
    
    private void ClearSlots()
    {
        foreach (var slot in _skillSlots)
        {
            PoolingManager.Instance.Release(slot.gameObject);
        }
        _skillSlots.Clear();
    }
}
