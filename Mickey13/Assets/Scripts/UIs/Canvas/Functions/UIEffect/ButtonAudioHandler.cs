using UnityEngine;
using UnityEngine.UI;

public class ButtonAudioHandler : MonoBehaviour
{
    [SerializeField] float sustainSoundInterval = 0.01f;

    public Button button;
    private float lastSustainSoundTime;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void PlayClickSound()
    {
        if (!CanPlaySound()) return;
        
        AudioManager.Instance.PlaySFX(SfxType.Button_Click);
    }

    public void PlayLongPressSound()
    {
        if (!CanPlaySound()) return;
        
        if (Time.time - lastSustainSoundTime > sustainSoundInterval)
        {
            lastSustainSoundTime = Time.time;
            AudioManager.Instance.PlaySFX(SfxType.Button_Click);
        }
    }

    private bool CanPlaySound()
    {
        if (button == null || button.interactable == false)
        {
            return false;
        }
        return true;
    }
}
