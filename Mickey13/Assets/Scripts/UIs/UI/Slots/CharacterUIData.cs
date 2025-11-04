using UnityEngine;

public class CharacterUIData : ISlotUIData
{
    public CharacterSO SO {  get; private set; } // 캐릭터 고유 정보
    public EntityData CharData { get; private set; } // 저장된 개별 캐릭터의 현재 정보

    public string ID => SO.ID;
    public string Name => SO.Name;

    public RarityEnum Rarity => SO.Rarity;

    public bool IsNew {  get; private set; }
    public Sprite GetSprite(SlotImageType imageType) => CharData.GetSprite(imageType);

    public CharacterUIData(CharacterSO charSO, EntityData charData, bool isNew = false)
    {
        SO = charSO;
        CharData = charData;
        this.IsNew = isNew;
    }
}
