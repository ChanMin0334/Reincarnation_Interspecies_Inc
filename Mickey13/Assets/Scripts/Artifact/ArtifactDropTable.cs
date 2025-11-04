using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Artifact/DropTable", fileName = "ArtifactDropTable")]
public class ArtifactDropTable : ScriptableObject
{
    [SerializeField] private List<ArtifactSO> entries = new();

    [SerializeField] private float commonPercent = 50f;
    [SerializeField] private float rarePercent = 35f;
    [SerializeField] private float epicPercent = 12f;
    [SerializeField] private float legendaryPercent = 2.9f;
    [SerializeField] private float uniquePercent = 0.1f;

    private ArtifactSO GetRandomArtifact()
    {
        RarityEnum selectedRarity = RollRarity();

        List<ArtifactSO> rarityList = entries.FindAll(e => e.Rarity == selectedRarity);
        if (rarityList.Count == 0) return null;

        int index = UnityEngine.Random.Range(0, rarityList.Count);
        return rarityList[index];
    }

    private RarityEnum RollRarity()
    {
        float total = commonPercent + rarePercent + epicPercent + legendaryPercent + uniquePercent;
        float roll = UnityEngine.Random.Range(0, total);

        if (roll < commonPercent) return RarityEnum.Common;
        roll -= commonPercent;

        if (roll < rarePercent) return RarityEnum.Rare;
        roll -= rarePercent;

        if (roll < epicPercent) return RarityEnum.Epic;
        roll -= epicPercent;

        if (roll < legendaryPercent) return RarityEnum.Legendary;
        roll -= legendaryPercent;

        return RarityEnum.Unique;
    }

    public List<ArtifactSO> GetRandomCandidates(int count = 3, bool allowDuplicates = false)
    {
        List<ArtifactSO> candiates = new();

        while(candiates.Count < count)
        {
            var candiate = GetRandomArtifact();
            if (candiate == null) continue;

            if (User.Instance.artifactInven.HasSameArtifact(candiate.Name)) continue;

            if (!allowDuplicates && candiates.Contains(candiate)) continue;

            candiates.Add(candiate);      
        }

        return candiates;
    }
    

}
