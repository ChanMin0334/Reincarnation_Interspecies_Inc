using System;
using UnityEngine;
using UnityEngine.UI;

public class EntityUI : MonoBehaviour
{
    [SerializeField] public GameObject UIPrefab;  // UI 캔버스 (Entity 자식으로 생성)
    [SerializeField] public GameObject UICanvas;  // 체력바 컨테이너
    [SerializeField] private Entity targetEntity; // 체력 정보를 받을 Entity
    [SerializeField] private Image hpFillImage;   // FillAmount로 제어할 Image

    private void Awake()
    {
        //만약 Enemy 레이어라면 Enemy가져오게하기
        if (this.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            targetEntity = GetComponent<Enemy>();
        }
        else
        {
            targetEntity = GetComponent<Character>();
        }
    }

    public void Init()
    {
        hpFillImage = UICanvas.transform.Find("HPBar/CurHP").GetComponent<Image>();
    }

    private void OnEnable()
    {
        if (targetEntity != null)
            targetEntity.OnHpChanged += OnHpChanged;
        
        if (hpFillImage != null)
        {
            hpFillImage.fillAmount = 1f;
            UpdateHpBarColor(1f); // 색상도 최대치(초록색)로 강제 설정
        }
    }

    private void OnDisable()
    {
        if (targetEntity != null)
            targetEntity.OnHpChanged -= OnHpChanged;
    }
    
    private void OnHpChanged(BigNumeric curHP, BigNumeric maxHP)
    {
        if (hpFillImage != null && maxHP > 0)
        {
            float ratio = curHP.Ratio(maxHP);
            hpFillImage.fillAmount = ratio;
            UpdateHpBarColor(ratio);
        }
    }

    // 수동 갱신용 (예: 인스펙터에서 강제로 호출)
    public void Refresh()
    {
        if (targetEntity != null && hpFillImage != null && targetEntity.Data.MaxHP.value > 0)
        {
            float ratio = targetEntity.Data.curHP.value.Ratio(targetEntity.Data.MaxHP);
            hpFillImage.fillAmount = ratio;
            UpdateHpBarColor(ratio);
        }
    }

    // 체력 비율에 따라 색상 변경
    private void UpdateHpBarColor(float ratio)
    {
        if (ratio >= 1f)
            hpFillImage.color = Color.green;
        else if (ratio >= 0.6f)
            hpFillImage.color = Color.yellow;
        else if (ratio >= 0.3f)
            hpFillImage.color = new Color(1f, 0.65f, 0f); // 주황색 (Orange)
        else
            hpFillImage.color = Color.red;
    }
}