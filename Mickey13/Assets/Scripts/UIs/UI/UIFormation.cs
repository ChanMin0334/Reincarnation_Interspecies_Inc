using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIFormation : UIBase //?
{
    [Header("formationSlots")]
    // [SerializeField] List<SlotUI> formationSlots; // 편성목록 (인스펙터에서 직접 넣기)
    [SerializeField] List<CharCardUI> formationSlots; // 편성목록 (인스펙터에서 직접 넣기)

    [Header("Buttons")]
    [SerializeField] private Button confirmBtn; // 편성완료 버튼
    [SerializeField] private Button resetBtn;  // 편성 취소(초기화)
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button closeBtn;

    [Header("Character List Panel")]
    [SerializeField] private CharacterTabPanel characterListPanel;

    private void OnEnable()
    {
        characterListPanel.OnInventoryRefreshed += HandlePanelRefreshed;
        characterListPanel.OnCharCardClicked += HandleCharCardClicked;
        FormationManager.Instance.OnFormationUpdated += UpdateUI;
        confirmBtn.onClick.AddListener(OnClickConfirm);
        resetBtn.onClick.AddListener(OnClickReset);
        settingBtn.onClick.AddListener(OnClickSetting);
        closeBtn.onClick.AddListener(OnClickExit);
    }

    private void OnDisable()
    {
        characterListPanel.OnInventoryRefreshed -= HandlePanelRefreshed;
        characterListPanel.OnCharCardClicked -= HandleCharCardClicked;
        FormationManager.Instance.OnFormationUpdated -= UpdateUI;
        confirmBtn.onClick.RemoveAllListeners();
        resetBtn.onClick.RemoveAllListeners();
        settingBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
    }

    public override void Init()
    {
        GameManager.Instance.Tutorial.FormationEnterTutorial();
    }

    private void UpdateUI(List<CharacterUIData> formation)
    {
        for (int i = 0; i < formationSlots.Count; i++)
        {
            if (i < formation.Count && formation[i] != null)
            {
                formationSlots[i].Setup(formation[i]);

                formationSlots[i].OnClick -= HandleFormationSlotClicked;
                formationSlots[i].OnClick += HandleFormationSlotClicked;
            }
            else
            {
                formationSlots[i].Clear();
                formationSlots[i].OnClick -= HandleFormationSlotClicked;
            }
        }

        foreach (var card in characterListPanel.GetActiveSlot().Values)
        {
            int slotIndex = formation.FindIndex(slot => slot != null && slot.ID == card.Data.ID);
            if (slotIndex != -1)
            {
                card.SetInFormation(true, slotIndex);
            }
            else
            {
                card.SetInFormation(false);
            }

        }
    }

    #region 버튼 및 클릭 이벤트

    private void HandleCharCardClicked(CharCardUI charCard)
    {
        bool isAlreadyInFormation = FormationManager.Instance.CurrentFormations.Any(card => card != null && card.ID == charCard.Data.ID);

        if (isAlreadyInFormation)
        {
            FormationManager.Instance.RemoveCharacterFromFormation(charCard.Data);
        }
        else
        {
            FormationManager.Instance.AddCharacterToFormation(charCard.Data);
        }

    }

    private void HandleFormationSlotClicked(CharCardUI clickedSlot)
    {
        if(clickedSlot.Data is CharacterUIData charData)
        {
            FormationManager.Instance.RemoveCharacterFromFormation(charData);
        }
    }

    private void OnClickConfirm()
    {
        FormationManager.Instance.CompleteConfirm();
    }
    private void OnClickExit()
    {
        FormationManager.Instance.OnClickReset();
        UIManager.Instance.Open<UIMain>();
    }

    private void OnClickReset()
    {
        FormationManager.Instance.OnClickReset();
    }
    private void OnClickSetting()
    {
        UIManager.Instance.Open<PopupSetting>();
    }

    #endregion

    #region UI 레이아웃 그룹 실행 순서 및 동기화 문제 해결용 메서드
    private void HandlePanelRefreshed()
    {
        StartCoroutine(UpdateUIAfterLayout());
    }

    private System.Collections.IEnumerator UpdateUIAfterLayout()
    {
        yield return new WaitForEndOfFrame();
        UpdateUI(FormationManager.Instance.CurrentFormations);
    }
    #endregion
}
