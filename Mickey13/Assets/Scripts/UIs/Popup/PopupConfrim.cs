using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupConfirm : PopupBase
{
    [SerializeField] Button confirmButton;
    [SerializeField] Button cancelButton;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI messageText;
    
    [Header("상점용")]
    [SerializeField] StoreSlotUI largeSlotPrefab;
    [SerializeField] StoreSlotUI smallSlotPrefab;
    
    public event Action OnConfirmClicked;

    private void Awake()
    {
        largeSlotPrefab.gameObject.SetActive(false);
        smallSlotPrefab.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        confirmButton.onClick.AddListener(HandleConfirmClick);
        cancelButton.onClick.AddListener(HandleCancelClick);
    }

    private void OnDisable()
    {
        confirmButton.onClick.RemoveListener(HandleConfirmClick);
        cancelButton.onClick.RemoveListener(HandleCancelClick);
        OnConfirmClicked = null;
    }

    /// <summary>
    /// 아이템 구매용
    /// </summary>
    /// <param name="item"></param>
    /// <param name="onConfirm"></param>
    public void ShowPurchase(StoreItemSO item, Action onConfirm)
    {
        OnConfirmClicked = onConfirm;
        titleText.text = "아이템 구매";
        messageText.text = "상품을 구매하시겠습니까?";
        
        if (item.popup == StoreItemPopupEnum.Large)
        {
            smallSlotPrefab.gameObject.SetActive(false); 
            largeSlotPrefab.gameObject.SetActive(true);    
            largeSlotPrefab.Setup(item);                   
        }
        else // Small 또는 기타
        {
            largeSlotPrefab.gameObject.SetActive(false);     
            smallSlotPrefab.gameObject.SetActive(true);    
            smallSlotPrefab.Setup(item);                   
        }
    }

    // 범용 확인용
    public void ShowConfirm(string title, string message, Action onConfirm)
    {
        OnConfirmClicked = onConfirm;
        titleText.text = title;
        messageText.text = message;
        smallSlotPrefab.gameObject.SetActive(false); 
        largeSlotPrefab.gameObject.SetActive(false);    
    }
    
    private void HandleConfirmClick()
    {
        OnConfirmClicked?.Invoke();
        UIManager.Instance.Close<PopupConfirm>();

    }

    private void HandleCancelClick()
    {
        UIManager.Instance.Close<PopupConfirm>();
    }
}
