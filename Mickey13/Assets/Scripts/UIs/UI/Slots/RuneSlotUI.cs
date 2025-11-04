using System;
using TMPro;
using UnityEngine;

public class RuneSlotUI : SlotBase<RuneData>
{
    [SerializeField] SlotUI runeIcon;
    [SerializeField] TextMeshProUGUI runeName;
    [SerializeField] TextMeshProUGUI runeCount;
    [SerializeField] TextMeshProUGUI runeDescription;
    [SerializeField] TextMeshProUGUI runeAmount;
    
    private StatDatabaseSO statDatabase;

    private int _ownedCount; // 소지한 룬 갯수 캐싱

    public event Action<RuneData> OnSlotClicked;

    private void Awake()
    {
        statDatabase = GameManager.Instance.StatDatabase;
    }

    private void OnEnable()
    {
        EventManager.Instance.StartListening(EventType.AddRuneToInventory, UpdateUI);
    }

    private void OnDisable()
    {
        EventManager.Instance.StopListening(EventType.AddRuneToInventory, UpdateUI);
    }

    public void Setup(RuneData rune, int ownedCount = 0)
    {
        _ownedCount = ownedCount;
        base.Setup(rune);
    }

    protected override void UpdateUI()
    {
        runeIcon.Setup(_data);
        runeName.text = _data.Name;
        var definition = statDatabase.GetDefinition(_data.Definition.EffectType);
        
        runeDescription.text = $"{_data.Definition.Description}\n(<size=110%><color=red>{_data.EffectValue}{definition.valueSuffix}</color></size>)";


        if (_ownedCount > 0)
        {
            runeCount.text = $"{_ownedCount} / {_data.Definition.MaxStack}"; // 임시 
        }
        else
        {
            runeCount.text = $"0 / 0";
        }
    }

    public override void Clear()
    {
        base.Clear();
        runeIcon.Clear();
        runeName.text = "";
        runeCount.text = "";
        runeDescription.text = "";
    }

    public void OnClickSlot()
    {
        if (!IsEmpty())
            OnSlotClicked?.Invoke(_data);
    }
}