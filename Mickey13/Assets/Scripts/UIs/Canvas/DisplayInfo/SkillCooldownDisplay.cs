using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownDisplay : MonoBehaviour
{
    [SerializeField] Image cooldownOverlay;
    [SerializeField] TextMeshProUGUI cooldownText;
    
    private SkillInstance _skillInstance;

    private void Awake()
    {
        if (cooldownOverlay == null || cooldownText == null) return;
        
        cooldownOverlay.type = Image.Type.Filled;
        cooldownOverlay.fillMethod = Image.FillMethod.Radial360;
        cooldownOverlay.fillAmount = 0;
        cooldownOverlay.fillClockwise = false;
        cooldownOverlay.gameObject.SetActive(false);
    }

    public void Setup(SkillInstance skillInstance)
    {
        Clear();

        if (skillInstance == null) return;
        cooldownText.text = skillInstance.GetCoolDown().ToString("N0");
        skillInstance.OnSkillUse += StartCooldownAnimation;

    }

    private void StartCooldownAnimation(float duration)
    {
        if(cooldownOverlay == null) return;
        cooldownOverlay.gameObject.SetActive(true);
        cooldownOverlay.DOKill();
        cooldownOverlay.fillAmount = 1f;
        cooldownOverlay.DOFillAmount(0f, duration)
            .SetEase(Ease.Linear);
    }

    public void Clear()
    {
        if (_skillInstance != null)
        {
            _skillInstance.OnSkillUse -= StartCooldownAnimation;
        }

        _skillInstance = null;

        if (cooldownOverlay != null)
        {
            cooldownOverlay.DOKill();
            cooldownOverlay.fillAmount = 0;
        }
        cooldownText.text = string.Empty;
        cooldownOverlay.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Clear();
    }
}
