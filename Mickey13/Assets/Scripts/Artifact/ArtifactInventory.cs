using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class ArtifactInventory : MonoBehaviour
{
    [SerializeField] private List<ArtifactData> ownedArtifact = new();
    public IReadOnlyList<ArtifactData> OwnedArtifact => ownedArtifact;

    public void AddArtifact(string artifactID, int count = 1)
    {
        var artifact = ownedArtifact.Find(a => a.id == artifactID);

        if(artifact != null)
        {
            int maxCount = artifact.Definition.MaxCount;
            int beforeCount = artifact.count;

            artifact.count = Mathf.Min(artifact.count + count, maxCount);

            if(artifact.count == beforeCount)
            {
                Debug.Log($"ArtifactInventory : {artifactID}는 이미 최대보유량{maxCount}입니다.");
                return;
            }

            Debug.Log($"[ArtifactInventory] {artifactID} 중복 획득 → count = {artifact.count}");
            EventManager.Instance.TriggerEvent(EventType.UpdateArtifactToInventory, artifact);
        }
        else
        {
            var newArtifact = new ArtifactData(artifactID, count);
            newArtifact.count = Mathf.Min(newArtifact.count, newArtifact.Definition.MaxCount);
            ownedArtifact.Add(newArtifact);

            Debug.Log($"[ArtifactInventory] {artifactID} 신규 획득 (count = {count})");
            EventManager.Instance.TriggerEvent(EventType.AddArtifactToInventory, newArtifact);
        }
    }

    public void Init(List<ArtifactData> artifacts)
    {
        ownedArtifact.Clear();
        foreach (var a in artifacts)
            ownedArtifact.Add(a);
    }

    public int GetCount(string artifactID)
    {
        if (string.IsNullOrEmpty(artifactID))
            return 0;

        var artifact = ownedArtifact.Find(a => a.id == artifactID);

        if (artifact != null)
            return artifact.count;

        return 0;
    }

    public bool HasSameArtifact(string name)
    {
        foreach(var artifact in ownedArtifact)
        {
            if (artifact.Definition.Name == name && GetCount(artifact.Definition.ID) >= artifact.Definition.MaxCount)
                return true;
        }

        return false;
    }
}
