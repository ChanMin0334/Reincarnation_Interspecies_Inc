using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class SkillSlotUI : SlotBase<SkillUIData>
{
    [SerializeField] SlotUI skillIcon;
    [SerializeField] TextMeshProUGUI skillName;
    [SerializeField] TextMeshProUGUI skillType;
    [SerializeField] TextMeshProUGUI skillDescription;
    [SerializeField] TextMeshProUGUI skillCooldown;
    private List<SkillSlotUI> _skillSlots = new();
    protected override void UpdateUI()
    {
        var skillData = _data.SkillSO;
        
        skillIcon.Setup((ISlotUIData)skillData);
        skillName.text = skillData.Name;
        skillType.text = skillData.Type.ToString();
        skillCooldown.text = $"{skillData.CoolDown} 초";
        skillDescription.text = skillData.Description;
        
    }

    public override void Clear()
    {
        base.Clear();
        skillIcon.Clear();
        skillName.text = string.Empty;
        skillType.text = string.Empty;
        skillDescription.text = string.Empty;
        skillCooldown.text = string.Empty;
    }
}
