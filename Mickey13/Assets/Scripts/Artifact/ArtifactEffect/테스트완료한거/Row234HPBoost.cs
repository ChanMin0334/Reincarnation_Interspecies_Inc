using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Row234HPBoost : ArtifactEffect
{
    private int[] targetRows = { 1, 2, 3 };
    private float applyPercent = 0;

    private List<Entity> boostedCharacters = new();

    public override void RegisterEvent()
    {
        base.RegisterEvent();
        EventManager.Instance.StartListening(EventType.FormationChanged, OnFormationChanged);
        ApplyHPBoost();
    }

    public override void UnregisterEvent()
    {
        RemoveHPBoost();
        if(EventManager.Instance != null)
            EventManager.Instance.StopListening(EventType.FormationChanged, OnFormationChanged);
    }

    private void OnFormationChanged()
    {
        RemoveHPBoost();
        ApplyHPBoost();
    }

    private void ApplyHPBoost()
    {
        float ratio = so.Value;
        applyPercent = data.count * ratio;
        boostedCharacters.Clear();

        foreach (int rowIndex in targetRows)
        {
            var character = CharacterTeam.ReturnCharacter(rowIndex);
            if (character == null) continue;

            character.Data.Artifacts.HP.value += applyPercent;
            boostedCharacters.Add(character);

            Debug.Log($"Row234HPBoost {rowIndex}열 {character.Name} HP + {applyPercent} 적용");
        }
    }

    private void RemoveHPBoost()
    {
        foreach (var ch in boostedCharacters)
        {
            if (ch != null)
            {
                ch.Data.Artifacts.HP.value -= applyPercent;
                Debug.Log($"[Row234HPBoost] {ch.Name} HP -{applyPercent}");
            }      
        }

        boostedCharacters.Clear();
        applyPercent = 0f;
    }
}
