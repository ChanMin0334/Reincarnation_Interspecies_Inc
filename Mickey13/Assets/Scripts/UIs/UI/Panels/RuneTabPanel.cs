using System;
using System.Collections.Generic;
public class RuneTabPanel : BaseInventoryUI<RuneData, RuneSlotUI>
{
    protected override void OnEnable()
    {
        base.OnEnable();
        EventManager.Instance.StartListening(EventType.AddRuneToInventory, OnRuneAdded);
        EventManager.Instance.StartListening(EventType.UpdateRuneToInventory, OnRuneUpdated);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventManager.Instance.StopListening(EventType.AddRuneToInventory, OnRuneAdded);
        EventManager.Instance.StopListening(EventType.UpdateRuneToInventory, OnRuneUpdated);
    }

    private void OnRuneAdded(object data)
    {
        if (data is RuneData newRune)
        {
            Refresh();
        }
    }

    private void OnRuneUpdated(object data)
    {
        if (data is RuneData updatedRune)
        {
            if (activeSlots.TryGetValue(updatedRune.id, out RuneSlotUI slotToUpdate))
            {
                slotToUpdate.Setup(updatedRune, updatedRune.count);
            }
        }
    }

    protected override void InitFilterSort()
    {
        filterSelectors = new Dictionary<FilterType, Func<RuneData, string>>
        {
             {FilterType.Rarity, a => a.Definition.Rarity.ToString() },
        };

        sortSelectors = new Dictionary<SortType, Func<RuneData, object>>
        {
            {SortType.Rarity, a => a.Definition.Rarity },
            {SortType.Level, a => a.count}
        };
    }

    protected override IEnumerable<RuneData> GetInventoryData()
    {
        return User.Instance.runeInven.OwnedRunes;
    }

    protected override void SetupSlot(RuneSlotUI slot, RuneData data)
    {
        slot.Setup(data,data.count);
    }

    protected override RuneSlotUI CreateSlot()
    {
        return PoolingManager.Instance.Get(prefab.gameObject, content).GetComponent<RuneSlotUI>();
    }

    protected override void ClearAllSlots()
    {
        if (activeSlots.Count == 0) return;
        foreach (var slot in activeSlots.Values)
        {
            slot.Clear();
            PoolingManager.Instance.Release(slot.gameObject);
        }
        activeSlots.Clear();
    }
}
