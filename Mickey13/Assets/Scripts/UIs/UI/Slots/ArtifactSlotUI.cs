using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactSlotUI : SlotBase<ArtifactSO>
{
    [SerializeField] SlotUI artifactIcon; // 유물 이미지
    [SerializeField] TextMeshProUGUI artifactName;  // 유물 이름
    [SerializeField] TextMeshProUGUI artifactCount; // 유물 갯수 
    [SerializeField] TextMeshProUGUI artifactEffect; // 유물 효과

    [SerializeField] GameObject slotHighlight; // 슬롯 선택시 하이라이트
    [SerializeField] Button slot; // 슬롯 클릭 버튼

    private int _ownedCount; // 소지한 유물 갯수 캐싱
    private ArtifactData _inventoryData;

    public event Action<ArtifactSO> OnSlotClicked;

    private void OnEnable()
    {
        SetSelected(false);
        slot.onClick.AddListener(OnClickSlot);
        EventManager.Instance.StartListening(EventType.AddArtifactToInventory, UpdateUI);
    }

    private void OnDisable()
    {
        slot.onClick.RemoveAllListeners();
        EventManager.Instance.StopListening(EventType.AddArtifactToInventory, UpdateUI);
    }

    public void Setup(ArtifactSO artifact, int ownedCount = 0, ArtifactData data = null)
    {
        _ownedCount = ownedCount;
        _inventoryData = data;
        base.Setup(artifact);
    }

    protected override void UpdateUI() 
    {
        if(_data == null) return;
        
        artifactIcon.Setup((ISlotUIData)_data);
        artifactName.text = _data.Name;
        artifactCount.text = $"{_ownedCount}/{_data.MaxCount}";
        
        if (_inventoryData != null)
        {
            artifactEffect.text = $"{_data.Description}\n( <size=110%><color=red>x{_ownedCount}</color></size> )";
        }
        else
        {
            artifactEffect.text = _data.Description;
        }
    }

    public override void Clear()
    {   
        base.Clear();
        artifactIcon.Clear();
        artifactName.text = "";
        artifactCount.text = "";
        artifactEffect.text = "";
        SetSelected(false);
    }

    public void SetSelected(bool isSelected) // 슬롯 선택 시 하이라이트 유무
    {
        slotHighlight.SetActive(isSelected);
    }

    public void OnClickSlot()
    {
        if(!IsEmpty())
            OnSlotClicked?.Invoke(_data);
    }
}