using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FormationManager : Singleton<FormationManager>
{
    List<CharacterUIData> currentFormations = new(); // 현재 편성중인 캐릭터 목록
    List<CharacterUIData> confirmFormations = new(); // 편성 완료된 캐릭터 목록

    public List<CharacterUIData> CurrentFormations => currentFormations;

    public event Action<List<CharacterUIData>> OnFormationUpdated;

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < 4; i++)
        {
            if (currentFormations == null)
                return;
            currentFormations.Add(null);
            confirmFormations.Add(null);
        }
        EventManager.Instance.TriggerEvent(EventType.FormationChanged);
    }
    private void OnEnable()
    {
        EventManager.Instance.StartListening(EventType.StartReincarnate, OnReincarnate);
    }

    private void OnDisable()
    {
        if(EventManager.Instance != null)
            EventManager.Instance.StopListening(EventType.StartReincarnate, OnReincarnate);
    }

    // 편성 슬롯에 캐릭터 추가
    public void AddCharacterToFormation(CharacterUIData charData)
    {
        if (currentFormations.Any(data => data != null && data.ID == charData.ID))
        {
            Debug.Log("이미 편성된 캐릭터입니다.");
            return;
        }

        int emptySlotIndex = currentFormations.FindIndex(slot => slot == null); // null을 가진 첫번째 슬롯의 인덱스 탐색
        if (emptySlotIndex != -1) // -1 은 null인 슬롯을 못찾았다는 의미. 즉 슬롯이 null이라면 로직 실행
        {
            currentFormations[emptySlotIndex] = charData;
            Debug.Log($"{charData.SO.Name}을(를) 빈 슬롯에 추가");
            OnFormationUpdated?.Invoke(currentFormations);
        }
        else
        {
            Debug.Log("편성창이 가득 찼습니다.");
        }
    }

    // 편성 슬롯에서 캐릭터 제거
    public void RemoveCharacterFromFormation(CharacterUIData charData)
    {
        for (int i = 0; i < currentFormations.Count; i++)
        {
            if (currentFormations[i] != null && currentFormations[i].ID == charData.ID)
            {
                currentFormations[i] = null;
                Debug.Log($"{charData.SO.Name}을 편성 슬롯에서 제거");
                break;
            }
        }
        OnFormationUpdated?.Invoke(currentFormations);
    }

    // 편성 완료
    public void CompleteConfirm()
    {
        confirmFormations = new(currentFormations);

        if (confirmFormations.All(slot => slot == null))
        {
            UIManager.Instance.Open<PopupAlert>().ShowAlert("현재 편성된 캐릭터가 없습니다.");
            return;
        }

        //추가 
        ApplyConfirmFormation();
        
        EventManager.Instance.TriggerEvent(EventType.FormationChanged);
        UIManager.Instance.Open<UIMain>();
        GameManager.Instance.Tutorial.FormationExitTutorial();
        SaveManager.Instance.SaveUser();
    }

    public void SpawnField()
    {
        Debug.Log($"[SpawnField 호출] FormationManager.SpawnField() 실행됨, 호출스택:\n{Environment.StackTrace}");

        // 필드 초기화(제거 또는 교체)
        BattleManager.Instance.ClearField();

        // 편성된 캐릭터 스폰
        for (int i = 0; i < confirmFormations.Count; i++)
        {
            var character = confirmFormations[i];
            if (character != null)
                BattleManager.Instance.SpwanCharacter(character.CharData, i);
            else continue;
        }

        var ui = UIManager.Instance.GetUI<UIMain>();
        ui.EnhancePanel.SetFormation(confirmFormations.Select(c => c?.CharData).ToList());
    }

    // 편성 초기화
    public void OnClickReset()
    {
        currentFormations = new(confirmFormations);
        Debug.Log("편성 슬롯 초기화");
        OnFormationUpdated?.Invoke(currentFormations);
    }

    // 캐릭터 인벤토리에서 캐릭터 편성 여부 동기화용 메서드
    public bool IsCharacterInConfirmFormation(string charID, out int index)
    {
        for (int i = 0; i < confirmFormations.Count; i++)
        {
            if (confirmFormations[i] != null && confirmFormations[i].ID == charID)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }
    
    private void ApplyConfirmFormation() // 캐릭터 스폰 및 매니저 등록 
    {
        User.Instance.battleCharacterDict.Clear();
        if(CharacterManager.Instance != null)
            CharacterManager.Instance.BattleCharacterList.Clear(); // 최적화 추가

        foreach(var ch in confirmFormations)
        {
            if(ch?.CharData != null)
            {
                User.Instance.battleCharacterDict[ch.CharData.ID] = null;
            }
        }

        SpawnField();

        for(int i = 0; i <  confirmFormations.Count; i++)
        {
            var ch = confirmFormations[i];
            if(ch?.CharData != null && i < BattleManager.Instance.SpwanCharacters.Count)
            {
                var character = BattleManager.Instance.SpwanCharacters[i];
                User.Instance.battleCharacterDict[ch.CharData.ID] = character;
                
                if(CharacterManager.Instance != null)
                    CharacterManager.Instance.BattleCharacterList.Add(character); // 최적화 추가
            }
        }
    }

    #region 세이브 및 환생
    public List<CharacterUIData> GetConfirmFormation() // 세이브용 
    {
        return confirmFormations;
    }

    public void LoadFromSaveData(List<EntityData> loadedFormationData)
    { 
        confirmFormations.Clear();
        currentFormations.Clear();

        for(int i = 0; i < 4;  i++)
        {
            if (i < loadedFormationData.Count && loadedFormationData[i] != null)
            {
                var entityData = loadedFormationData[i];
                var so = DataManager.Instance.GetData<CharacterSO>(entityData.id);
                var uiData = new CharacterUIData(so, entityData);
                confirmFormations.Add(uiData);
                currentFormations.Add(uiData);
            }
            else
            {
                confirmFormations.Add(null);
                currentFormations.Add(null);
            }
        }
        OnFormationUpdated?.Invoke(currentFormations);

        ApplyConfirmFormation();
    }

    public void OnReincarnate()
    {
        confirmFormations.Clear();
        currentFormations.Clear();

        for (int i = 0; i < 4; i++)
        {
            confirmFormations.Add(null);
            currentFormations.Add(null);
        }

        OnFormationUpdated?.Invoke(currentFormations);

        var ui = UIManager.Instance.GetUI<UIMain>();
        ui.EnhancePanel.SetFormation(new List<EntityData>());
    }
    #endregion
}

