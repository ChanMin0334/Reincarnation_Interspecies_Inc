using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupCharDetailStat : PopupBase
{
    [Header("CurrentChar Info")]
    [SerializeField] SlotUI portrait; //캐릭터 초상화
    [SerializeField] TextMeshProUGUI charName; // 캐릭터 이름
    [SerializeField] TextMeshProUGUI charLevel; // 캐릭터 레벨
    [SerializeField] TextMeshProUGUI charClass; // 캐릭터 클래스
    [SerializeField] TextMeshProUGUI charRarity; // 캐릭터 레어도
    [SerializeField] TextMeshProUGUI charBreakthrough; // 캐릭터 돌파 상태
    [SerializeField] TextMeshProUGUI breakthroughCostAmount; // 돌파에 필요한 재화 갯수(현재 갯수 / 필요 갯수)
    [SerializeField] TextMeshProUGUI charDescription; // 캐릭터 설명

    [Header("Character Stat & Skill Info")]
    [SerializeField] StatusPanel statusPanel;
    [SerializeField] SkillPanel skillPanel;
    
    [Header("DataBase")]
    [SerializeField] private StatDatabaseSO _statData; // 스탯 정보 데이터
    private CharacterUIData _charData; // 캐릭터 정보(so, EntityData)

    private List<StatSlotUI> _statSlots = new();
   

    [Header("Buttons")]
    [SerializeField] Button breakthroughBtn; // 돌파 버튼
    [SerializeField] Button exitBtn; // 나가기 버튼
    [Header("TabButtons")]
    [SerializeField] TabPanel tabPanel;


    private void OnEnable()
    {
        if (!GameManager.Instance.Tutorial.HasPlayedTutorial(TutorialType.CharStats))
        {
            StartCoroutine(TutorialRoutine());
        }

        portrait.OnSlotClicked += HandleClickSlot;
    }

    private void OnDisable()
    {
        portrait.OnSlotClicked -= HandleClickSlot;
    }

    private IEnumerator TutorialRoutine()
    {
        yield return new WaitForEndOfFrame();
        GameManager.Instance.Tutorial.CharStatsTutorial();
    }

    public override void Init() 
    {
        base.Init();
        exitBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.AddListener(OnClickExit);
        
        breakthroughBtn.interactable = false;
    }

    public override void SetData(object data)
    {
        if (data is CharacterUIData charData)
        {
            Setup(charData);
        }
    }

    public void Setup(CharacterUIData charData)
    {
        _charData = charData;

        SetupCharInfo();
        statusPanel.InitStatData(_statData,_charData);
        skillPanel.InitSkillData(_charData);
        tabPanel.ClickTab(0);
    }

    private void SetupCharInfo() // 대상캐릭터 정보 출력
    {
        charName.text = _charData.Name;
        portrait.Setup(_charData);
        SetCharClassKorea();
        SetCharRarityKorea();
        charLevel.text = "LV." + _charData.CharData.level.ToString();
        // charClass.text = _charData.SO.CharClass.ToString();
        // charRarity.text = _charData.Rarity.ToString();
        charDescription.text = _charData.CharData.Definition.Description;
        breakthroughCostAmount.text = $"기억의 조각 : {_charData.CharData.pieceOfMemory}";
    }
    
    private void OnClickExit()
    {
        popupAnimation.PlayCloseAnimation(() =>
            UIManager.Instance.Close<PopupCharDetailStat>());
    }

    private void HandleClickSlot(SlotUI slot)
    {
        UIManager.Instance.Open<PopupViewer>().Setup(_charData);
    }
    
    // 임시 코드 추후 수정 예정
    private void SetCharClassKorea()
    {
        switch (_charData.SO.CharClass)
        {
            case CharacterClassEnum.Warrior:
                charClass.text = "전사";
                break;
            case CharacterClassEnum.Mage:
                charClass.text = "마법사";
                break;
            case CharacterClassEnum.Archer:
                charClass.text = "궁수";
                break;
            default:
                charClass.text = string.Empty;
                break;
        }
        
    }

    private void SetCharRarityKorea()
    {
        switch (_charData.Rarity)
        {
            case RarityEnum.Common:
                charRarity.text = "커먼";
                break;
            case RarityEnum.Rare:
                charRarity.text = "레어";
                break;
            case RarityEnum.Epic:
                charRarity.text = "에픽";
                break;
            case RarityEnum.Legendary:
                charRarity.text = "레전더리";
                break;
            case RarityEnum.Unique:
                charRarity.text = "유니크";
                break;
            default:
                charRarity.text = string.Empty;
                break;
        }
    }
    
}
