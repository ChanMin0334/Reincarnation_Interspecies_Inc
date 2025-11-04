using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupArtifactReward : PopupBase
{
    [Header("Chest")]
    [SerializeField] Image artifactBoxIcon; // 보물상자 아이콘
    [SerializeField] ArtifactSlotUI[] slots; // 유물 슬롯 배열
    [SerializeField] TextMeshProUGUI artifactBoxPopupName;

    [Header("Buttons")]
    [SerializeField] Button artifactProbabilityBtn; // 보물 확률 확인 버튼
    [SerializeField] Button choiceBtn; // 회수하기(선택하기) 버튼
    [SerializeField] Button retryBtn; // 다시시도 버튼

    public UnityEvent OnConfirm;
    public UnityEvent OnRetry;
    
    private ArtifactManager artifactManager;

    private void OnEnable()
    {
        if (!GameManager.Instance.Tutorial.HasPlayedTutorial(TutorialType.CharStats))
        {
            StartCoroutine(TutorialRoutine());
        }
    }
    private IEnumerator TutorialRoutine()
    {
        yield return new WaitForEndOfFrame();
        GameManager.Instance.Tutorial.ArtifactTutorial();
    }

    public override void Init()
    {
        base.Init();
        choiceBtn.onClick.AddListener(OnClickChoice);
        retryBtn.onClick.AddListener(OnClickRetry);
    }

    public void Setup(ArtifactManager manager, ArtifactChestType chestType, Sprite icon)
    {
        this.artifactManager = manager;
        SetArtifactBoxIInfo(chestType, icon);
        
        if (artifactManager != null)
        {
            artifactManager.OnSelectionChanged += HandleSelectionChanged;
            artifactManager.OnChoicesUpdated += HandleChoicesUpdated;
            artifactManager.OnPopupClosed += ClosePopup;
            
            var initialChoices = artifactManager.CurrentArtifactsList;
            if (initialChoices != null && initialChoices.Count > 0)
            {
                HandleChoicesUpdated(initialChoices);
            }
        }
    }
    private void OnDisable()
    {
       if (artifactManager != null)
       {
           artifactManager.OnSelectionChanged -= HandleSelectionChanged;
           artifactManager.OnChoicesUpdated -= HandleChoicesUpdated;
           artifactManager.OnPopupClosed -= ClosePopup;
       }
    }

    private void UpdateRerollBtnText()
    {
        retryBtn.gameObject.GetComponentInChildren<TextMeshProUGUI>().text =
            $"<size=100%>다시뽑기\n <size=80%>{artifactManager.CurrentRerollCount}/{artifactManager.MaxRerollCount} 100 환생석";
    }
    
    public void SetArtifactBoxIInfo(ArtifactChestType type, Sprite icon)
    {
        artifactBoxIcon.sprite = icon;

        switch (type)
        {
            case ArtifactChestType.normal:
                artifactBoxPopupName.text = "일반 유물상자";
                break;
            case ArtifactChestType.special:
                artifactBoxPopupName.text = "특별 유물상자";
                break;
        }
    }

    private void HandleChoicesUpdated(IReadOnlyList<ArtifactSO> artifacts) // 3가지 유물 화면에 표시
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if(i < artifacts.Count)
            {
                var artifact = artifacts[i];

                slots[i].gameObject.SetActive(true);

                int ownedCount = User.Instance.artifactInven.GetCount(artifact.ID); // 갯수확인을 위해 유저가 지니고 있는 유물의 id와 So의 id 비교
                slots[i].Setup(artifacts[i], ownedCount);

                slots[i].OnSlotClicked -= HandleSlotClicked;
                slots[i].OnSlotClicked += HandleSlotClicked;
            }

            slots[i].SetSelected(false);
            UpdateRerollBtnText();
        }
    }
    
    private void HandleSelectionChanged(ArtifactSO artifact) // 선택유물 강조
    {
        foreach (var slot in slots)
        {
            if(slot.gameObject.activeSelf)
                slot.SetSelected(slot.Data == artifact);
        }
    }

    private void HandleSlotClicked(ArtifactSO artifact)
    {
       artifactManager.SelectedArtifact(artifact);
    }

    private void OnClickChoice()
    {
        Debug.Log("회수하기 버튼 클릭");
        OnConfirm?.Invoke();
    }

    private void OnClickRetry()
    {
        Debug.Log("다시뽑기 버튼 클릭");
        OnRetry?.Invoke();
        UpdateRerollBtnText();
    }

    private void ClosePopup()
    {
        popupAnimation.PlayCloseAnimation(() => 
            UIManager.Instance.Close<PopupArtifactReward>());
    }
}
