using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharCardUI : SlotBase<CharacterUIData>
{
    [Header("Character Image")]
    [SerializeField] SlotUI charIcon; // 캐릭터 이미지

    [Header("Character Info")]
    [SerializeField] TextMeshProUGUI charLevel; // 캐릭터 레벨
    [SerializeField] Image charClassIcon; // 캐릭터 클래스 아이콘
    [SerializeField] Image charStar; // 캐릭터 등급 

    [Header("Character Formation")]
    [SerializeField] Button cardBtn; // 카드 버튼(클릭시 클릭한 캐릭터 편성/해제)
    [SerializeField] TextMeshProUGUI formationNumText; // 편성 번호
    [SerializeField] GameObject formationNumObj; // 편성 번호 오브젝트
    [SerializeField] CanvasGroup canvasGroup; // 편성 체크용

    [Header("RaritySprites")]
    [SerializeField] Sprite commonSprite;
    [SerializeField] Sprite rareSprite;
    [SerializeField] Sprite epicSprite;
    [SerializeField] Sprite legendarySprite;
    [SerializeField] Sprite uniqueSprite;
    
    [Header("CharClassIcons")]
    [SerializeField] Sprite warrior;
    [SerializeField] Sprite archer;
    [SerializeField] Sprite mage;
    // [SerializeField] Sprite Thief;

    public event Action<CharCardUI> OnClick;
    public event Action<CharCardUI> OnLongPress;

    [Header("정보 표시 여부")]
    [SerializeField] private bool isShowLevel; // 레벨 표시 여부
    private bool isOrganized = false; // 캐릭터 편성 여부
    private LongPressHandler longPressHandler;

    private void Awake()
    {
        longPressHandler = GetComponent<LongPressHandler>();
        if(longPressHandler != null )
        {
            longPressHandler.onClick.AddListener(HandleClick);
            longPressHandler.onLongPressStart.AddListener(HandleLongPress);
        }
        
        charLevel.transform.parent.gameObject.SetActive(false);
        charClassIcon.gameObject.SetActive(false);
        charStar.transform.parent.gameObject.SetActive(false);
    }

    protected override void UpdateUI()
    {
        charIcon.Setup(_data);
        SetCharLevel();
        SetCharClassIcon();
        SetCharRarityStar();
    }

    public override void Clear()
    {
        base.Clear();
        charIcon.Clear();
        charClassIcon.gameObject.SetActive(false);
        charLevel.text = string.Empty; // 캐릭터 레벨
        charLevel.transform.parent.gameObject.SetActive(false);
        formationNumText.text = string.Empty;
        charStar.transform.parent.gameObject.SetActive(false);
    }

    public void SetCharLevel()
    {
        if (isShowLevel)
        {
            charLevel.transform.parent.gameObject.SetActive(true);
            charLevel.text = $"Lv. {_data.CharData.level}";
        }
        else
        {
            charLevel.transform.parent.gameObject.SetActive(false);
            charLevel.text = string.Empty;
        }
    }
    
    public void SetInFormation(bool status, int slotIndex = -1)
    {
        isOrganized = status;
        formationNumObj.SetActive(status);
        if (isOrganized)
        {
            canvasGroup.alpha = 0.5f;
            formationNumText.text =  (slotIndex + 1).ToString();
        }
        else
        {
            canvasGroup.alpha = 1.0f;
        }
    }

    private void HandleClick()
    {
        OnClick?.Invoke(this);
    }

    private void HandleLongPress()
    {
        OnLongPress?.Invoke(this);
    }
    
    private void SetCharClassIcon()
    {
        charClassIcon.gameObject.SetActive(true);
        switch (_data.SO.CharClass)
        {
            case CharacterClassEnum.Warrior:
                charClassIcon.sprite = warrior;
                break;
            case CharacterClassEnum.Archer:
                charClassIcon.sprite = archer;
                break;
            case CharacterClassEnum.Mage:
                charClassIcon.sprite = mage;
                break;
            default:
                charClassIcon.gameObject.SetActive(false);
                break;
        }
    }

    // 레어도에 따른 별 표사는 현재 캐릭터에서만 사용하기로 기획쪽에서 이야기했기 때문에 switch case문으로 구현
    // 하지만 확장성을 고려한다면 레어리티 전용 SO를 만들어 관리하는게 더욱 좋아보임
    // 추후 리팩토링 고려
    private void SetCharRarityStar()
    {
        charStar.transform.parent.gameObject.SetActive(true);
        switch (_data.Rarity)
        {
            case RarityEnum.Common:
                charStar.sprite = commonSprite;
                break;
            case RarityEnum.Rare:
                charStar.sprite = rareSprite;
                break;
            case RarityEnum.Epic:
                charStar.sprite = epicSprite;
                break;
            case RarityEnum.Legendary:
                charStar.sprite = legendarySprite;
                break;
            case RarityEnum.Unique:
                charStar.sprite = uniqueSprite;
                break;
            default:
                charStar.transform.parent.gameObject.SetActive(false);
                break;
        }
    }
}
