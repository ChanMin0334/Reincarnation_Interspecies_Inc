using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneInventory : MonoBehaviour
{
    [SerializeField] private List<RuneData> ownedRunes = new();

    public IReadOnlyList<RuneData> OwnedRunes => ownedRunes;

    public (RuneData,bool isNew) AddRune(string runeID, int count = 1)
    {
        var rune = ownedRunes.Find(r => r.id == runeID);
        if(rune != null)
        {
            rune.count = Mathf.Min(rune.count + count, rune.Definition.MaxStack);

            GetUserBonus();
            return (rune, false);
        }
        else
        {
            var newRune = new RuneData(runeID, count);
            newRune.count = Mathf.Min(newRune.count, newRune.Definition.MaxStack);
            ownedRunes.Add(newRune);

            EventManager.Instance.TriggerEvent(EventType.AddRuneToInventory);
            GetUserBonus();

            return (newRune, true);
        }


    }

    public StatModel GetTotalRuneStat(CharacterSO character)
    {
        StatModel total = StatModel.One();

        foreach(var rune in ownedRunes)
        {

            RuneSO so = rune.Definition;
            switch(so.Target)
            {
                case ApplyTargetTypeEnum.Global:
                    total += rune.GetFinalStat();
                    break;

                case ApplyTargetTypeEnum.Class:
                    if(character.CharClass == so.CharClass)
                        total += rune.GetFinalStat();
                    break;

                case ApplyTargetTypeEnum.Character:
                    if(character.ID == so.ID)
                        total += rune.GetFinalStat();
                    break;
            }
        }

        return total;
    }

    public void GetUserBonus()
    {
        User.Instance.ResetLaborAndHuntMult();

        foreach (var rune in ownedRunes)
        {
            var so = rune.Definition;
            float value = so.EffectValue * rune.count;

            if (so.Target == ApplyTargetTypeEnum.Global)
            {
                switch (so.EffectType)
                {         
                    case StatType.Labor_Gold:
                        User.Instance.AddLaborMult(value);
                        break;
                    case StatType.Hunt_Gold:
                        User.Instance.AddHuntMult(value);
                        break;
                }
            }
        }
    }

    public void Init(List<RuneData> runeDatas)
    {
        ownedRunes.Clear();
        foreach (var r in runeDatas)
            ownedRunes.Add(r);

        GetUserBonus();
    }
}
