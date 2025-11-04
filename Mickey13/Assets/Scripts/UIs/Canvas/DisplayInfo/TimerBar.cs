using UnityEngine;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour
{
    [SerializeField] Image linerTimer; // 리니어 타이머

    public void UpdateBar(float curTime, float maxTime)
    {
        if (maxTime > 0)
        {
            linerTimer.fillAmount = curTime / maxTime;
        }
    }
}
