using UnityEngine;
using UnityEngine.UI;

public class GameSpeedController : MonoBehaviour
{
    [SerializeField] private Toggle gameSpeedToggle;

    private void Start()
    {
        gameSpeedToggle.isOn = false;
        gameSpeedToggle.onValueChanged.AddListener(HandleSpeedChange);
    }

    private void HandleSpeedChange(bool isOn)
    {
        if(isOn)
            GameManager.Instance.SpeedLevel = 2f; // 속도 초기화
        else
            GameManager.Instance.SpeedLevel = 1f; // 2배속

        Time.timeScale = GameManager.Instance.SpeedLevel;
    }

    private void OnDestroy()
    {
        if (gameSpeedToggle != null)
        {
            gameSpeedToggle.onValueChanged.RemoveListener(HandleSpeedChange);
        }
    }
    
}
