using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class PopupGameResult : PopupBase
{
    [SerializeField] Button applyBtn;

    [SerializeField] ResultSlot slotPrefab;
    [SerializeField] Transform slotParent;

    [SerializeField] List<ResultSlot> activeSlots = new();

    [Header("Animation Settings")]
    [SerializeField] private float charAnimationDuration = 0.05f; 
    [SerializeField] private float delayBetweenSlots = 0.2f; 
    
    private void OnEnable()
    {
        applyBtn.onClick.AddListener(OnApplyBtnClicked);
        EventManager.Instance.TriggerEvent(EventType.Reincarnating); // 환생 중 이벤트 트리거
    }

    private void OnDisable()
    {
        applyBtn.onClick.RemoveListener(OnApplyBtnClicked);
    }

    public void ShowResult(GameResultData data)
    {
        ClearSlots();

        var resultToShow = new List<KeyValuePair<string, string>>
        {
            new("이동 거리", $"{data.MaxDistance}Km"),
            new("플레이 시간", TimeUtils.GetTimeStringFromSeconds(data.PlayTime)),
            new("처치한 적", data.enemiesDefeated.ToString("N0")),
            new("벌어들인 골드", data.goldEarned.ToString()),
            new("소지 유물 수", data.ArtifactNumbers.ToString("N0")),
            new("최고 레벨", data.MaxLevel.ToString()),
            new("총 데미지", data.TotalDamage.ToString()),
            new("아군 사망 횟수", data.DeathCount.ToString("N0")),
            new("획득 영혼석", data.soulStoneEarned.ToString())
        };

        StartCoroutine(ShowResultSequentially(resultToShow));
    }

    private IEnumerator ShowResultSequentially(List<KeyValuePair<string, string>> results)
    {
        applyBtn.interactable = false;

        foreach (var result in results)
        {
            var obj = PoolingManager.Instance.Get(slotPrefab.gameObject, slotParent);
            var slot = obj.GetComponent<ResultSlot>();
            slot.Setup(result.Key);
            activeSlots.Add(slot);

            yield return StartCoroutine(slot.AnimateValueText(result.Value, charAnimationDuration));

            yield return new WaitForSeconds(delayBetweenSlots);
        }

        applyBtn.interactable = true;
    }

    private void ClearSlots()
    {
        foreach (var slot in activeSlots)
        {
            slot.Clear();
            PoolingManager.Instance.Release(slot.gameObject);
        }
        activeSlots.Clear();  
    }

    private void OnApplyBtnClicked()
    {
        ClearSlots();
        EventManager.Instance.TriggerEvent(EventType.EndReincarnate);
        
        popupAnimation.PlayCloseAnimation(() =>
            {
                UIManager.Instance.Close<PopupGameResult>(); 
                UIManager.Instance.Open<UIMain>(); 
            });
    }
}
