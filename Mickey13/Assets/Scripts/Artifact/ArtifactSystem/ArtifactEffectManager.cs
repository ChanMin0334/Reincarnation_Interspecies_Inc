using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactEffectManager : Singleton<ArtifactEffectManager>
{
    [SerializeField] private ArtifactInventory inventory;

    //[SerializeField] private List<ArtifactEffect> activeEffects = new();

    private Dictionary<string, List<ArtifactEffect>> activeEffectsDict = new();
    private Dictionary<string, List<ArtifactEffect>> activeGlobalEffectsDict = new();


    private void Start()
    {
        Debug.Log("ArtifactEffectManager.Start() 실행됨");
        inventory = User.Instance.artifactInven;
        //ApplyAllArtifactsToAllCharacters(); => 주기때문에 User에서 실행중
    }

    private void OnEnable()
    {
        EventManager.Instance.StartListening(EventType.AddArtifactToInventory, OnArtifactAdded);
        EventManager.Instance.StartListening(EventType.UpdateArtifactToInventory, OnArtifactAdded); //추가
        EventManager.Instance.StartListening(EventType.FormationChanged, OnFormationChanged);

    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.StopListening(EventType.AddArtifactToInventory, OnArtifactAdded);
            EventManager.Instance.StopListening(EventType.UpdateArtifactToInventory, OnArtifactAdded); //추가
            EventManager.Instance.StopListening(EventType.FormationChanged, OnFormationChanged);
        }
        RemoveAllEffects();
    }

    private void OnFormationChanged()
    {
        Debug.Log("ArtifactEffectManager : 편성변경");

        var currentCharacters = User.Instance.battleCharacterDict;

        foreach (var kvp in activeEffectsDict.ToList())
        {
            string id = kvp.Key;
            if (!currentCharacters.ContainsKey(id))
            {
                Debug.Log($"ArtifactEffectManager : {id} 캐릭터 편성 해제");
                RemoveEffectsForCharacterId(id);
            }
        }

        foreach (var kvp in currentCharacters)
        {
            string id = kvp.Key;
            var ch = kvp.Value;

            if (ch == null)
            {
                Debug.LogWarning($"ArtifactEffectManager : {id}캐릭터가 null");
                continue;
            }

            if (activeEffectsDict.TryGetValue(id, out var effects))
            {
                foreach (var effect in effects)
                {
                    if (effect == null) continue;

                    if (effect.Owner != ch)
                    {
                        Debug.Log($"ArtifactEffectManager : {id}가 Owner 교체");
                        effect.SetOwner(ch);
                    }
                }
            }
        }

        foreach (var kvp in currentCharacters)
        {
            string id = kvp.Key;
            var ch = kvp.Value;

            if (!activeEffectsDict.ContainsKey(id))
            {
                ApplyAllArtifactsToCharacter(ch);
            }
        }
    }

    private void OnArtifactAdded(object obj)
    {
        if (obj is not ArtifactData artifact) return;

        Debug.Log("ArtifactEffectManager : 새로운 유물 획득 현재 배치된 캐릭터 유물 적용");
        User.Instance.ReincarnateData.ArtifactNumbers++;

        var so = artifact.Definition;
        if (so == null) return;

        if(so.Target == ApplyTargetTypeEnum.GlobalSystem)
        {
            ApplySystemArtifact(so, artifact);
            return;
        }

        foreach (var ch in User.Instance.battleCharacterDict.Values)
        {
            if (IsValidTarget(so, ch))
            {
                AddArtifactEffects(so, artifact, ch);
            }
        }
    }

    public void ApplyAllArtifactsToAllCharacters()
    {
        Debug.Log("ArtifactEffectManager : ApplyAllArtifactsToAllCharacters 실행됨");

        var allCharacters = User.Instance.battleCharacterDict.Values;

        foreach(var artifact in inventory.OwnedArtifact)
        {
            var so = artifact.Definition;
            if (so == null) return;

            switch(so.Target)
            {
                case ApplyTargetTypeEnum.GlobalSystem:
                    if(!activeGlobalEffectsDict.ContainsKey(so.ID))
                        ApplySystemArtifact(so, artifact);

                    break;

                case ApplyTargetTypeEnum.Global:
                    foreach(var ch in allCharacters)
                    {
                        if (ch == null || ch.IsDead) continue;
                        AddArtifactEffects(so, artifact, ch);
                    }

                    break;

                default:
                    foreach(var ch in allCharacters)
                    {
                        if (!IsValidTarget(so, ch)) continue;
                        AddArtifactEffects(so, artifact, ch);
                    }

                    break;
            }
        }

        //foreach (var ch in User.Instance.battleCharacterDict.Values)
        //{
        //    Debug.Log($"캐릭터: {ch.Name} ({ch.Data.id}) 유물 적용 시도");
        //    ApplyAllArtifactsToCharacter(ch);
        //}
    }

    public void ApplyAllArtifactsToCharacter(Character ch)
    {
        if (!activeEffectsDict.ContainsKey(ch.Data.id))
        {
            activeEffectsDict[ch.Data.id] = new List<ArtifactEffect>();
        }

        var effects = activeEffectsDict[ch.Data.id];
        effects.Clear();

        foreach (var artifact in inventory.OwnedArtifact)
        {
            var so = artifact.Definition;
            if (so == null) continue;

            if (so.Target == ApplyTargetTypeEnum.GlobalSystem) continue;

            if (!IsValidTarget(so, ch)) continue;

            AddArtifactEffects(so, artifact, ch);       
        }
    }

    private void ApplySystemArtifact(ArtifactSO so, ArtifactData artifact)
    {
        if(activeGlobalEffectsDict.ContainsKey(so.ID))
        {
            Debug.Log($"ArtifactEffectManager : System 유물 {so.Name} 이미 적용됨");
            return;
        }

        var type = Type.GetType(so.EffectClassName);
        if(type == null)
        {
            Debug.LogError($"ArtifactEffectManager : {so.EffectClassName} 클래스를 찾을 수 없습니다");
            return;
        }

        ArtifactEffect effect = Activator.CreateInstance(type) as ArtifactEffect;
        if(effect == null)
        {
            Debug.LogError($"ArtifactEffectManager : {so.EffectClassName}은 ArtifactEffect가 아님");
            return;
        }

        effect.Init(so, artifact, null); //캐릭터 없이 전역 실행
        effect.RegisterEvent();

        activeGlobalEffectsDict[so.ID] = new List<ArtifactEffect> { effect };
        Debug.Log($"ArtifactEffectManager : System 유물 {so.Name} (전역 1회) 적용 완료");
    }

    public void RemoveEffectsForCharacter(Character ch)
    {
        RemoveEffectsForCharacterId(ch.Data.id);
    }

    public void RemoveEffectsForCharacterId(string id)
    {
        if (!activeEffectsDict.TryGetValue(id, out var effects)) return;

        foreach (var effect in effects)
        {
            effect.UnregisterEvent();
        }

        activeEffectsDict.Remove(id);
    }

    public void RemoveAllEffects()
    {
        Debug.Log($"ArtifactManager : RemoveAllEffects 호출");

        foreach (var kvp in activeEffectsDict)
        {
            foreach (var effect in kvp.Value)
            {
                effect.UnregisterEvent();
            }
        }
        activeEffectsDict.Clear();

        foreach(var kvp in activeGlobalEffectsDict)
        {
            foreach (var effect in kvp.Value)
                effect.UnregisterEvent();

        }
        activeGlobalEffectsDict.Clear();
    }


    /// <summary>
    /// 유물의 타켓을 체크후 Bool값으로 반환해주는 함수
    /// </summary>
    /// <returns></returns>
    public bool IsValidTarget(ArtifactSO so, Character ch)
    {
        switch (so.Target)
        {
            case ApplyTargetTypeEnum.Global:
                return true;

            case ApplyTargetTypeEnum.Class:
                if (ch.Definition is CharacterSO charSo)
                    return charSo.CharClass == so.CharClass;
                return false;

            default:
                return false;
        }
    }

    /// <summary>
    /// 새로운 유물 추가
    /// </summary>
    public void AddArtifactEffects(ArtifactSO so, ArtifactData artifact, Character owner)
    {
        Debug.Log("AddArtifactEffects 실행");
        if (so == null || owner == null)
        {
            Debug.LogError("ArtifactEffectManager : So가 Null 또는 owner 가 null.");
            return;
        }

        string id = owner.Data.id;

        if (!activeEffectsDict.TryGetValue(id, out var list))
        {
            list = new List<ArtifactEffect>();
            activeEffectsDict[id] = list;
        }

        if (list.Any(e => e.So.ID == so.ID))
        {
            var existing = list.First(e => e.So.ID == so.ID);

            existing.AddStack(artifact);
            Debug.Log($"ArtifactEffectManager : {owner.Name}에게 {artifact.ID} 효과 스택 총 {artifact.count}회 적용됨");
            return;
        }

        var type = Type.GetType(so.EffectClassName);
        if (type == null)
        {
            Debug.LogError($"ArtifactEffectManager : {so.Name}의 클래스{so.EffectClassName}를 찾을수 없습니다.");
            return;
        }

        ArtifactEffect effect = Activator.CreateInstance(type) as ArtifactEffect;
        if(effect == null)
        {
            Debug.LogError($"ArtifactEffectManager : {so.EffectClassName}은 ArtifactEffect가 아님");
            return;
        }

        effect.Init(so, artifact, owner);
        effect.RegisterEvent();

        list.Add(effect);
        Debug.Log($"ArtifactEffectManager : {owner.Name}에게 {so.Name} 효과 등록 완료");
    }
}
