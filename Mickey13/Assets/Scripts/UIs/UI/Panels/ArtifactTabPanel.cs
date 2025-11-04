using System;
using System.Collections.Generic;
public class ArtifactTabPanel : BaseInventoryUI<ArtifactData, ArtifactSlotUI>
{
    protected override void OnEnable()
    {
        base.OnEnable();
        EventManager.Instance.StartListening(EventType.AddArtifactToInventory, OnArtifactAdded);
        EventManager.Instance.StartListening(EventType.UpdateArtifactToInventory, OnArtifactUpdated);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventManager.Instance.StopListening(EventType.AddArtifactToInventory, OnArtifactAdded);
        EventManager.Instance.StopListening(EventType.UpdateArtifactToInventory, OnArtifactUpdated);
    }

    private void OnArtifactAdded()
    {
        Refresh();
    }

    private void OnArtifactUpdated(object data)
    {
        if (data is ArtifactData updatedArtifact)
        {
            if (activeSlots.TryGetValue(updatedArtifact.id, out ArtifactSlotUI slotToUpdate))
            {
                slotToUpdate.Setup(updatedArtifact.Definition, updatedArtifact.count, updatedArtifact);
            }
        }
    }

    protected override void InitFilterSort()
    {
        filterSelectors = new Dictionary<FilterType, Func<ArtifactData, string>>
        {
            {FilterType.Rarity, a => a.Definition.Rarity.ToString() },
        };

        sortSelectors = new Dictionary<SortType, Func<ArtifactData, object>>
        {
            {SortType.Rarity, a => a.Definition.Rarity },
            {SortType.Level, a => a.count}
        };
    }

    protected override IEnumerable<ArtifactData> GetInventoryData()
    {
        return User.Instance.artifactInven.OwnedArtifact;
    }

    protected override void SetupSlot(ArtifactSlotUI slot, ArtifactData data)
    {
        slot.Setup(data.Definition, data.count, data);
    }

    protected override ArtifactSlotUI CreateSlot()
    {
        return PoolingManager.Instance.Get(prefab.gameObject, content).GetComponent<ArtifactSlotUI>();
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
