using System.Collections.Generic;
using UnityEngine;

public class Row1HPBoost : ArtifactEffect
{
    private int rowIndex = 0;
    private float applyPercent = 0f;

    private Entity boostedCharacter;

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
        
        var character = CharacterTeam.ReturnCharacter(rowIndex);
        if (character == null) return;

        boostedCharacter = character;
        character.Data.Artifacts.HP.value += applyPercent;

        Debug.Log($"Row1HPBoost {rowIndex}열 {character.Name} HP + {applyPercent} 적용");
    }

    private void RemoveHPBoost()
    {
        if(boostedCharacter != null)
        {
            boostedCharacter.Data.Artifacts.HP.value -= applyPercent;
            Debug.Log($"[Row1HPBoost] {rowIndex}열 {boostedCharacter.Name} HP -{applyPercent}");

        }

        boostedCharacter = null;
        applyPercent = 0f;
    }

}
