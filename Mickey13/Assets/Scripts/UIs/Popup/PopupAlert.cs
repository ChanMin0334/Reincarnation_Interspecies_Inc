using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupAlert : PopupBase
{
    [SerializeField] TextMeshProUGUI alertText; // 알림 문구
    [SerializeField] Button exitBtn;
    
    public override void Init()
    {
        base.Init();
        exitBtn.onClick.AddListener(OnClickExit);
    }

    public void ShowAlert(string message)
    {
        gameObject.SetActive(true);
        SetMesssage(message);
    }

    private void SetMesssage(string message)
    {
        if(alertText != null)
        {
            alertText.text = message;
        }
    }

    private void OnClickExit()
    {
        popupAnimation.PlayCloseAnimation(() => UIManager.Instance.Close<PopupAlert>());
    }
}
