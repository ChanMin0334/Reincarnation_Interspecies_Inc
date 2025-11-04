using UnityEngine;

public abstract class GameData : ScriptableObject //다른 SO스크립트들은 이 GameData.cs를 상속받게 설계 => DataManager에서 Dictionary 하나로 모두 관리
{
    [SerializeField] protected string id; //id
    [SerializeField] protected string name; //name
    [SerializeField] protected GameDataType dataType;
    [SerializeField] protected Sprite sprite; //스프라이트
    [SerializeField] protected string description; //설명
    
    public string ID => id;
    public string Name => name;
    public GameDataType DataType => dataType;
    public Sprite Sprite => sprite;
    public string Description => description;
    public virtual RarityEnum Rarity => RarityEnum.None; //하위에서 rarity 가져오려면 override해서 사용가능
}
