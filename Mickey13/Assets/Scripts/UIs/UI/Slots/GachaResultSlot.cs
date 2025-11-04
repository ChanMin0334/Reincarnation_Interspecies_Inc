using System;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class GachaResultSlot : SlotBase<ISlotUIData>
{
    [Header("Database")] 
    [SerializeField] private StatDatabaseSO statDatabase;
    
    [Header("UI Groups")]
    [SerializeField] GameObject characterInfoGroup;
    [SerializeField] GameObject runeInfoGroup;

    [Header("Common UI")]
    [SerializeField] SlotUI resultIcon; // 가챠 결과 이미지
    [SerializeField] Image borderImage; // 등급 테두리 이미지

    [Header("Effect UI")]
    [SerializeField] Button coverBtn; // 커버 버튼
    [SerializeField] GachaResultOpenEffect resultOpenEffect;
    [SerializeField] GameObject newBadge; // new뱃지

    [Header("Character Info")]
    [SerializeField] TextMeshProUGUI charName; // 캐릭터 이름
    [SerializeField] TextMeshProUGUI charRarity; // 캐릭터 등급 
    [SerializeField] TextMeshProUGUI charClass; // 캐릭터 클래스 

    [Header("Rune Info")]
    [SerializeField] TextMeshProUGUI runeName; // 룬 이름
    [SerializeField] TextMeshProUGUI runeRarity; // 룬 등급
    [SerializeField] TextMeshProUGUI runeEffect; // 룬 효과
    [SerializeField] TextMeshProUGUI runeCount; // 룬 갯수

    public event Action OnRevealed; // 슬롯 공개 이벤트

    public bool IsAlreadyRevealed // 건너뛰기 시 이미 소리가 재생됬는지 여부 접근용 프로퍼티
    {
        get
        {
            if (resultOpenEffect == null) return true;
            return resultOpenEffect.IsRevealed;
        }
    }

    public void Setup(ISlotUIData data, Action onRevealedCallback)
    {
        OnRevealed = null;
        OnRevealed += onRevealedCallback;

        coverBtn.interactable = true;
        newBadge.SetActive(false);
        //borderImage.gameObject.SetActive(false);

        base.Setup(data);

        if (resultOpenEffect != null)
            resultOpenEffect.ResetEffect();

        coverBtn.onClick.RemoveAllListeners();
        coverBtn.onClick.AddListener(OnClickCover);
    }

    protected override void UpdateUI()
    {
        resultIcon.Setup(_data);
        UpdateBorderColor(_data.Rarity);

        if (_data is CharacterUIData charData)
        {
            characterInfoGroup.SetActive(true);
            runeInfoGroup.SetActive(false);

            charName.text = charData.Name;
            SetupCharInfoKorea(charData);
        }
        else if (_data is RuneData runeData)
        {
            characterInfoGroup.SetActive(false);
            runeInfoGroup.SetActive(true);

            runeName.text = runeData.Definition.Name;
            SetupRuneInfoKorea(runeData);
            runeCount.text = $"{runeData.count}";
        }
    }

    private void SetupCharInfoKorea(CharacterUIData data)
    {
        switch (data.Rarity)
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
        }

        switch (data.SO.CharClass)
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

    private void SetupRuneInfoKorea(RuneData runeData)
    {
        if (statDatabase == null)
        {
            runeEffect.text = $"{runeData.Definition.EffectType} : {runeData.Definition.EffectValue}";
            return;
        }
        StatDefinition statDef = statDatabase.GetDefinition(runeData.Definition.EffectType);

        if (statDef != null)
        {
            runeEffect.text = $"{statDef.statName} : {runeData.Definition.EffectValue}{statDef.valueSuffix}";
        }
        else
        {
            runeEffect.text = string.Empty;
        }

        switch (runeData.Rarity)
        {
            case RarityEnum.Common:
                runeRarity.text = "커먼";
                break;
            case RarityEnum.Rare:
                runeRarity.text = "레어";
                break;
            case RarityEnum.Epic:
                runeRarity.text = "에픽";
                break;
            case RarityEnum.Legendary:
                runeRarity.text = "레전더리";
                break;
            case RarityEnum.Unique:
                runeRarity.text = "유니크";
                break;
        }
    }

    private void UpdateBorderColor(RarityEnum rarity)
    {
        switch (rarity)
        {
            case RarityEnum.Rare:
                borderImage.color = new Color32(0x3a, 0x82, 0xf7, 0xFF);
                break;
            case RarityEnum.Epic:
                borderImage.color = new Color32(0xa3, 0x35, 0xee, 0xFF);
                break;
            case RarityEnum.Legendary:
                borderImage.color = new Color32(0xff, 0x88, 0x00, 0xFF);
                break;
            case RarityEnum.Unique:
                borderImage.color = new Color32(0xe5, 0x2b, 0x50, 0xFF);
                break;
            default:
                borderImage.color = Color.clear;
                break;
        }
    }

    public void ClearUI()
    {
        newBadge.SetActive(false);
        resultIcon.Clear();
        charName.text = "";
        charRarity.text = "";
        charClass.text = "";

        runeName.text = "";
        runeRarity.text = "";
        runeEffect.text = ""; 
        runeCount.text = "";
    }

    private void OnClickCover()
    {
        if(!coverBtn.interactable) return;
        
        Reveal(true);
    }

    public void Reveal(bool playSound)
    {
        if (resultOpenEffect != null)
        {
            resultOpenEffect.PlayEffect(() =>
            {
                //borderImage.gameObject.SetActive(true);
                newBadge.SetActive(_data.IsNew);
                coverBtn.interactable = false;

                OnRevealed?.Invoke();
            }, playSound);
        }
    }
}
