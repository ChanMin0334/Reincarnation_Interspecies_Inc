using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterManager : Singleton<CharacterManager>
{
    // 모든 캐릭터들의 프리팹과 인스턴스 관리
    [SerializeField] private List<Character> charPrefabs;
    private Dictionary<string, GameObject> prefabDict;

    //배치 한 캐릭터들
    //private Dictionary<string, Character> battleCharacterDict = new();

    public List<Character> CharPrefabs => charPrefabs;
    public Dictionary<string, GameObject> PrefabDict => prefabDict;

    public Dictionary<string, Character> BattleCharacterDict => User.Instance.battleCharacterDict;
    
    // 최적화용 리스트
    public List<Character> BattleCharacterList = new List<Character>(4);

    protected override void Awake()
    {
        base.Awake();
        prefabDict = new Dictionary<string, GameObject>();

        // Resources/Prefab/Entity/Character 경로에서 모든 Character 프리팹 자동 로드
        //charPrefabs = Resources.LoadAll<Character>("Prefab/Entity/Character").ToList();
        charPrefabs = Resources.LoadAll<Character>("Prefab/Character").ToList();
        foreach (var ch in charPrefabs)
        {
            var id = ch.Id;
            if (!prefabDict.ContainsKey(id))
                prefabDict[id] = ch.gameObject;
        }
    }

    private void Update()
    {
        //Debug.Log($"CharacterManager{BattleCharacterDict.Count}개의 캐릭터 ");
    }
    private void OnEnable()
    {
        EventManager.Instance.StartListening(EventType.StartReincarnate, OnReincanate);
        //EventManager.Instance.StartListening(EventType.FormationChanged, OnFormationChanged);
    }
    private void OnDisable()
    {
        if(EventManager.Instance != null)
            EventManager.Instance.StopListening(EventType.StartReincarnate, OnReincanate);
        //EventManager.Instance.StopListening(EventType.FormationChanged, OnFormationChanged);
    }


    public CharacterSO GetCharacterSO(string id)
    {
        return CharPrefabs.Find(c => c.Id == id).Definition as CharacterSO;
    }

    public GameObject GetPrefab(string id)
    {
        return prefabDict.TryGetValue(id, out var prefab) ? prefab : null;
    }

    public void OnReincanate()
    {
        BattleCharacterDict.Clear();
        BattleCharacterList.Clear();
    }
}
