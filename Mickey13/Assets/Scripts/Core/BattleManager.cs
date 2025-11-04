using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using Unity.VisualScripting;
using DamageNumbersPro;

public class BattleManager : Singleton<BattleManager>
{
    [SerializeField] public DamageNumber DamagePrefab;
    [SerializeField] public DamageNumber CriticalDamagePrefab;
    [SerializeField] public DamageNumber HealPrefab;
    [SerializeField] public Transform teamPos;
    [SerializeField] Transform[] spwanPoints; // 캐릭터 스폰 위치

    private List<Character> spwanCharacters = new();  
    public List<Character> SpwanCharacters => spwanCharacters;

    private void Start()
    {
        StartCoroutine(TickLoop());
    }

    private void OnEnable()
    {
        EventManager.Instance.StartListening(EventType.Reincarnating, OnReincarnate);
        EventManager.Instance.StartListening(EventType.AllCharacterDead, OnRespawnOnPoint);
        EventManager.Instance.StartListening(EventType.OnRespawn, OnRespawn);
        EventManager.Instance.StartListening(EventType.CharacterDead, CheckAllCharcterDead);
    }

    private void OnDisable()
    {
        EventManager.Instance.StopListening(EventType.Reincarnating, OnReincarnate);
        EventManager.Instance.StopListening(EventType.AllCharacterDead, OnRespawnOnPoint);
        EventManager.Instance.StopListening(EventType.OnRespawn, OnRespawn);
        EventManager.Instance.StopListening(EventType.CharacterDead, CheckAllCharcterDead);
    }

    //캐릭터 필드 초기화(교체, 재편성 등)
    public void ClearField()
    {
        foreach(var character in spwanCharacters)
        {
            if(character != null)
                Destroy(character.gameObject); // TODO : 오브젝트 풀링
        }
        spwanCharacters.Clear();

        Debug.Log("캐릭터 필드 초기화");
    }

    public void SpwanCharacter(EntityData data, int idx)
    {
        Debug.Log($"[SpwanCharacter 호출] {data.id} 캐릭터 스폰 시도");

        var parents = spwanPoints[idx];
        var character = EntityFactor.CharSpawn(data, parents);
        if( character != null)
        {
            spwanCharacters.Add(character);

            //임시 추가
            //User.Instance.battleCharacterDict[data.id] = character;

            Debug.Log($"{data.id} 캐릭터 스폰");
        }
    }

    void CheckAllCharcterDead()
    {
        foreach(var cha in spwanCharacters)
        {
            if (!cha.IsDead)
                return;
        }
        // AudioManager.Instance.StopBGM(BgmType.Boss_Default);
        StartCoroutine(HandleAllCharacterDead());
    }

    private IEnumerator HandleAllCharacterDead()
    {
        // 모든 캐릭터가 사망했을 때 잠시 대기
        yield return new WaitForSeconds(2f);

        // 모든 캐릭터가 사망했을 때의 처리
        EventManager.Instance.TriggerEvent(EventType.AllCharacterDead);
        EventManager.Instance.TriggerEvent(EventType.OnRespawn);
    }

    public void OnRespawn()
    {
        foreach (var character in spwanCharacters)
        {
            character.gameObject.SetActive(true);
            character.OnRespawn();
        }

        foreach (var character in User.Instance.charInven.SaveCharacters)
        {
            character.OnRespawn();
        }
    }

    public void OnRespawnOnPoint()
    {
        int targetpos = Convert.ToInt32(Math.Truncate(User.Instance.CurAchievementKm / 30f));
        teamPos.position = new Vector3(targetpos * 90f, teamPos.position.y, teamPos.position.z);
    }

    public void OnReincarnate()
    {
        teamPos.position = new Vector3(0f, teamPos.position.y, teamPos.position.z);
        ClearField();
    }

    private IEnumerator TickLoop()
    {
        while(true)
        {
            Debug.Log("TickEvent Trigger중.");
            EventManager.Instance.TriggerEvent(EventType.Tick, null);
            yield return new WaitForSeconds(0.5f);
        }
    }
}