using System;
using System.Collections.Generic; 
using UnityEngine;

public class CharacterTabPanel : BaseInventoryUI<EntityData, CharCardUI>
{
    public event Action<CharCardUI> OnCharCardClicked;

    public IReadOnlyDictionary<string, CharCardUI> GetActiveSlot()
    {
        return activeSlots;
    }
    

    protected override void OnEnable()
    {
        base.OnEnable();
        EventManager.Instance.StartListening(EventType.AddCharacterToInventory, OnCharacterAdded);
        EventManager.Instance.StartListening(EventType.UpdateCharacterToInventory, OnCharacterUpdated);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventManager.Instance.StopListening(EventType.AddCharacterToInventory, OnCharacterAdded);
        EventManager.Instance.StopListening(EventType.UpdateCharacterToInventory, OnCharacterUpdated);
    }
    protected override void InitFilterSort()
    {
        filterSelectors = new Dictionary<FilterType, Func<EntityData, string>>
        {
            {FilterType.Class, c => (c.Definition as CharacterSO)?.CharClass.ToString() },
            {FilterType.Rarity, c => c.Rarity.ToString() },
        };

        sortSelectors = new Dictionary<SortType, Func<EntityData, object>>
        {
            {SortType.Rarity, c => c.Rarity },
            {SortType.Class, c => ( c.Definition as CharacterSO)?.CharClass },
            {SortType.Level, c => c.level },
            {
                SortType.Formation, c => // 편성중 여부
                {
                    int index = FormationManager.Instance.CurrentFormations.FindIndex(fomChar => fomChar != null && fomChar.ID == c.ID);
                    return index != -1 ? index : int.MaxValue;
                }
            }
        };
    }

    protected override IEnumerable<EntityData> GetInventoryData()
    {
        return User.Instance.charInven.SaveCharacters;
    }

    protected override CharCardUI CreateSlot() // 캐릭터 카드 생성
    {
        return PoolingManager.Instance.Get(prefab.gameObject, content).GetComponent<CharCardUI>();
    }

    protected override void SetupSlot(CharCardUI slot, EntityData data)
    {
        CharacterUIData charData = new CharacterUIData(data.Definition as CharacterSO, data);
        slot.Setup(charData);
        slot.OnLongPress -= HandleCharCardClicked;
        slot.OnLongPress += HandleCharCardClicked;
        slot.OnClick -= HandleCardClick;
        slot.OnClick += HandleCardClick;

        if (FormationManager.Instance.IsCharacterInConfirmFormation(data.ID, out int slotIndex))
        {
            slot.SetInFormation(true, slotIndex);
        }
        else
        {
            slot.SetInFormation(false);
        }
    }

    private void OnCharacterAdded(object data)
    {
        if (data is EntityData newCharacter)
        {
            Refresh();
        }
    }

    private void OnCharacterUpdated(object data)
    {
        if (data is EntityData updateCharacter)
        {
            if (activeSlots.TryGetValue(updateCharacter.id, out CharCardUI slotToUpdate))
            {
                SetupSlot(slotToUpdate, updateCharacter);
            }
        }
    }

    private void HandleCharCardClicked(CharCardUI card)
    {
        if (card.Data is CharacterUIData charData)
        {
            var ui = UIManager.Instance.Open<PopupCharDetailStat>(charData);
        }
    }

    private void HandleCardClick(CharCardUI card)
    {
        OnCharCardClicked?.Invoke(card);
    }

    protected override void ClearAllSlots()
    {
        if (activeSlots.Count == 0) return;
        foreach (var slot in activeSlots.Values)
        {
            PoolingManager.Instance.Release(slot.gameObject);
        }
        activeSlots.Clear();
    }
}
