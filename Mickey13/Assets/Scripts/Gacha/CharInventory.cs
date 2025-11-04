using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharInventory : MonoBehaviour
{
    [SerializeField] private List<EntityData> charSaveData = new(); //보유한 캐릭터의 저장 값, 지역 변수
    [SerializeField] private Dictionary<string, Character> charSaved = new(); //보유 한 캐릭터들

    [SerializeField] private bool debugMode = false; //디버그 모드

    public List<EntityData> SaveCharacters => charSaveData;
    public Dictionary<string, Character> CharSaved => charSaved;

    private void Start()
    {
        if (User.Instance == null) return;
        if (User.Instance.runeInven != null)
        {
            EventManager.Instance.StartListening(EventType.AddRuneToInventory, RefreshRuneStats);

        }
        if ( User.Instance.artifactInven != null)
        {
            EventManager.Instance.StartListening(EventType.AddArtifactToInventory, RefreshArtifactStats);
        }
        
    }

    private void OnDestroy()
    {
        if(User.Instance != null)
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.StopListening(EventType.AddArtifactToInventory, RefreshArtifactStats);
                EventManager.Instance.StopListening(EventType.AddRuneToInventory, RefreshRuneStats);
            }
        }    
    }

    private void RefreshRuneStats()
    {
        foreach(var character in charSaveData)
        {
            var charSO = DataManager.Instance.GetData<CharacterSO>(character.id);
            character.Rune = User.Instance.runeInven.GetTotalRuneStat(charSO);
        }
    }

    private void RefreshArtifactStats()
    {
        foreach (var character in charSaveData)
        {
            var charSO = DataManager.Instance.GetData<CharacterSO>(character.id);
        }
    }
    

    public List<EntityData> SaveCharactersToData()
    {
        foreach (var pair in User.Instance.battleCharacterDict)
        {
            var saveData = charSaveData.Find(c => c.id == pair.Key);
            if (saveData != null)
            {
                //saveData.UpdateFromCharacter(EntityData.Convert(pair.Value));
                saveData.UpdateFromCharacter(pair.Value.Data);
            }
        }

        return charSaveData;
    }

    public void LoadCharacters(List<EntityData> list)
    {
        if (list == null) return;
        charSaveData.Clear();

        foreach (var character in list)
        {
            var so = DataManager.Instance.GetData<CharacterSO>(character.id);
            character.Definition = so;

            charSaveData.Add(character);
        }
    }

    public (EntityData, bool isNew) AddCharacter(CharacterSO so)
    {
        var existing = charSaveData.Find(c => c.id == so.ID); //중복되는거 임시재화 +
        if(existing != null)
        {
            existing.pieceOfMemory += 1; //임시 환생재화 +
            //Debug.Log($"{so.name} 중복 뽑기 → 기억의 조각 +1 (현재 {existing.pieceOfMemory})");
            return (existing, false);
        }


        var saveData = new EntityData
        {
            id = so.ID,
            name = so.Name,
            MetaPermanent = StatModel.Zero(),
            Rune = User.Instance.runeInven.GetTotalRuneStat(so),
            RunProgress = so.BaseStat.Value,
            //Artifacts = StatModel.One(),
            curHP = so.BaseStat.Value.HP,
            level = 1,
            pieceOfMemory = 0,
            Definition = so
        };

        charSaveData.Add(saveData);
        EventManager.Instance.TriggerEvent(EventType.AddCharacterToInventory, saveData);
        Debug.Log($"{so.name} 캐릭터를 인벤토리에 추가");

        return (saveData,true);
    }

    public EntityData GetCharacterByID(string id)
    {
        return charSaveData.Find(c => c.id == id);
    }
}
