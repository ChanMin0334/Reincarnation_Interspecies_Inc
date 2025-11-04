using SpeechBubble;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupTutorial : PopupBase
{
    [SerializeField] private Image characterImg;
    [SerializeField] private SpeechBubble_TMP speechBubble;
    [SerializeField] private Button buttonNext;
    
    private List<string> messages; // 튜토리얼 매세지 리스트
    private int currentMessageIndex = 0; // 메세지 순서 인덱스
    private Action onCompleteCallback; // 튜토리얼 메세지가 끝났을 때 실행

    public override void Init()
    {
        base.Init();
        buttonNext.onClick.AddListener(OnNextButtonCLicked);
    }

    public void ShowMessage(List<string> tutorialMessages, Action onComplete = null)
    {
        this.messages = tutorialMessages;
        this.onCompleteCallback = onComplete;
        currentMessageIndex = 0;
        
        // 팝업 활성화
        UIManager.Instance.Open<PopupTutorial>();
        ShowCurrentMessage();
    }

    private void ShowCurrentMessage()
    {
        speechBubble.setDialogueText(messages[currentMessageIndex]);
    }
    
    private void OnNextButtonCLicked()
    {
        currentMessageIndex++;
        if (currentMessageIndex < messages.Count)
        {
            ShowCurrentMessage();
        }
        else
        {
            CloseTutorial();
        }
    }
    
    private void CloseTutorial()
    {
        onCompleteCallback?.Invoke();
        UIManager.Instance.Close<PopupTutorial>();
    }
    
}
