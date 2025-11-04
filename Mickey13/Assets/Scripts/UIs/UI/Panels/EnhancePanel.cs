using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnhancePanel : MonoBehaviour
{
    [SerializeField] List<EnhanceSlot> slots; // 강화창 캐릭터 목록(현재 4명)

    [SerializeField] Button organizeBtn; // 편성하기 버튼   
    [SerializeField] Button reincarnateBtn; // 환생하기 버튼

    private UIAnimation uiAnimation;
    
    private void OnEnable()
    {
        if (!GameManager.Instance.Tutorial.HasPlayedTutorial(TutorialType.Intro))
        {
            GameManager.Instance.Tutorial.IntroTutorial();
        }
        else
        {
            GameManager.Instance.Tutorial.EnhanceTutorial();
        }
    }

    public void Init()
    {
        uiAnimation = GetComponentInParent<UIAnimation>();
        organizeBtn.onClick.RemoveAllListeners();
        reincarnateBtn.onClick.RemoveAllListeners();
        organizeBtn.onClick.AddListener(OnClickOrganize);
        reincarnateBtn.onClick.AddListener(OnClickReincanate);

        foreach(var slot in slots)
        {
            // slot.OnEnhanceClicked -= HandleSlotEnhanceClicked;
            // slot.OnEnhanceClicked += HandleSlotEnhanceClicked;
            slot.OnIconClicked -= HandleSlotIconClicked;
            slot.OnIconClicked += HandleSlotIconClicked;
        }
    }

    public void ClearSlot() // 슬롯 초기화
    {
        foreach(var slot in slots)
            slot.Clear();
    }

    public void SetFormation(List<EntityData> datas) // 편성 캐릭터 데이터 주입
    {
        ClearSlot();

        for (int i = 0; i < datas.Count && i < slots.Count ; i++)
        {
            if (datas[i] != null)
            {
                User.Instance.battleCharacterDict.TryGetValue(datas[i].id, out Character character);
                slots[i].Setup(datas[i], character);
            }
            else
                slots[i].Clear();
        }
    }
    private void OnClickOrganize()
    {
        //OnOrganizeClicked?.Invoke();
        uiAnimation.PlayCloseAnimation(() =>
            UIManager.Instance.Open<UIFormation>());

    }

    public void OnClickReincanate()
    {
        //OnReincanateClicked?.Invoke();
        UIManager.Instance.Open<PopupReincarnate>();
    }

    // private void HandleSlotEnhanceClicked(EnhanceSlot clickedSlot,int levelToUpgrade)
    // {
    //     var data = clickedSlot.Data;
    //     if (data == null) return;
    //     
    //     UpgradeManager.Instance.GoldCharaUpgrade(clickedSlot.Data, levelToUpgrade);
    // }

    private void HandleSlotIconClicked(EnhanceSlot clickedSlot)
    {
        if (clickedSlot.Data is EntityData charData)
        {
            var so = DataManager.Instance.GetData<CharacterSO>(charData.id);
            var data = new CharacterUIData(so, charData);

            UIManager.Instance.Open<PopupCharDetailStat>(data);
        }
    }
}