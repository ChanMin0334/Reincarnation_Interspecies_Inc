using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotUI : MonoBehaviour // 공용 슬롯
{
    [Header("Slot 정보")]
    [SerializeField] Image displayImage; // 슬롯에 표시되어할 무언가의 이미지
    [SerializeField] SlotImageType imageTypeToShow; // 화면에 표시될 이미지 타입
    [SerializeField] GameObject countBox;
    [SerializeField] GameObject UniqueEffect;
    [SerializeField] TextMeshProUGUI countText; // 갯수 표시
    [SerializeField] Image frame_Effect; //프래임 이펙트
    [SerializeField] Image frame_Img; //프래임 이미지

    [Header("Slot 프래임")]
    [SerializeField] Sprite commonFrame;
    [SerializeField] Sprite rareFrame;
    [SerializeField] Sprite epicFrame;
    [SerializeField] Sprite legendaryFrame;
    [SerializeField] Sprite uniqueFrame;
    [SerializeField] Sprite noneFrame;

    public ISlotUIData Data { get; private set; }

    public event Action<SlotUI> OnSlotClicked; // 슬롯 클릭시 발생할 이벤트 구독

    private void Awake()
    {
        countBox.SetActive(false);
        UniqueEffect.SetActive(false);
    }

    public void Setup(ISlotUIData data, bool hasCount = false, int amount = 0) // 슬롯에 정보 표시
    {
        Data = data;
        displayImage.sprite = data.GetSprite(imageTypeToShow); // 보여줄 이미지 타입
        UpdateFrame(data.Rarity);
        UpdateCount(hasCount,amount);
    }
    public void Setup(GameData data, bool hasCount = false, int amount = 0) // 슬롯에 정보 표시
    {
        Data = null;
        displayImage.sprite = data.Sprite;
        UpdateFrame(data.Rarity);
        UpdateCount(hasCount,amount);
    }

    private void UpdateFrame(RarityEnum rarity)
    {
        //if(Data == null)
        //{
        //    //frame.color = Color.white;
        //    frame.sprite = noneFrame;
        //    return;
        //}

        switch (rarity)
        {
            case RarityEnum.Common:
                frame_Effect.color = new Color32(0xbf, 0xbf, 0xbf, 0xFF);
                frame_Img.sprite = commonFrame;
                frame_Img.color = new Color32(0xbf, 0xbf, 0xbf, 0xFF);
                break;
            case RarityEnum.Rare:
                frame_Effect.color = new Color32(0x3a, 0x82, 0xf7, 0xFF);
                frame_Img.sprite = rareFrame;
                frame_Img.color = new Color32(0x3a, 0x82, 0xf7, 0xFF);
                break;
            case RarityEnum.Epic:
                frame_Effect.color = new Color32(0xa3, 0x35, 0xee, 0xFF);
                frame_Img.sprite = epicFrame;
                frame_Img.color = new Color32(0xa3, 0x35, 0xee, 0xFF);
                break;
            case RarityEnum.Legendary:
                frame_Effect.color = new Color32(0xff, 0x88, 0x00, 0xFF);
                frame_Img.sprite = legendaryFrame;
                frame_Img.color = new Color32(0xff, 0x88, 0x00, 0xFF);
                break;
            case RarityEnum.Unique:
                frame_Effect.color = new Color32(0xe5, 0x2b, 0x50, 0xFF);
                UniqueEffect.SetActive(true);
                frame_Img.sprite = uniqueFrame;
                frame_Img.color = new Color32(0xe5, 0x2b, 0x50, 0xFF);
                break;
            default:
                frame_Effect.color = Color.clear;
                frame_Img.sprite = noneFrame;
                break;
        }
    }

    private void UpdateCount(bool hasCount, int amount)
    {
        if (hasCount)
        {
            if (amount > 1)
            {
                countText.text = amount.ToString();
                countBox.SetActive(true);
            }
            else
            {
                countBox.SetActive(false);
            }
        }
        else
        {
            countText.text = "0";
            countBox.SetActive(false);
        }
    }

    public void Clear()
    {
        Data = null;
        displayImage.sprite = noneFrame;
        countText.text = string.Empty;
        countBox.SetActive(false);
        frame_Img.sprite = noneFrame;
        frame_Effect.color = Color.clear;
        frame_Img.color = Color.clear;
        UniqueEffect.SetActive(false);
    }

    public void OnClickSlot() // 인스펙터에서 직접 할당
    {
        if (!IsEmpty())
            OnSlotClicked?.Invoke(this);
    }

    public bool IsEmpty()
    {
        return Data == null;
    }
}
