using UnityEngine;

/// <summary>
/// SlotUI프리팹의 크기마다 사용할 이미지 크기가 다르기 때문에 이넘으로 이미지 종류/크기 목록 생성
/// </summary>
public enum SlotImageType
{
    Icon, // 아이콘(정사각형)
    Banner, // 배너(직사각형)
    FullBody, // 전신 일러스트

}

/// <summary>
/// 캐릭터, 유물, 룬 등 슬롯을 사용하는 모든 곳에서 사용 가능하도록 인터페이스 추가
/// </summary>
public interface ISlotUIData
{
    string ID { get; } // ID 검색용
    string Name { get; } // 이름
    Sprite GetSprite(SlotImageType imageType); // 이미지
    RarityEnum Rarity { get; }
    bool IsNew => false;
}
