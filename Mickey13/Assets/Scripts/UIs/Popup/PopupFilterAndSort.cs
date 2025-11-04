using System; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupFilterAndSort : PopupBase
{
    [Header("Popup 버튼")]
    [SerializeField] private Button applyBtn; // 확인 버튼
    [SerializeField] private Button resetBtn; // 재설정 버튼
    [SerializeField] private Button closeBtn; // 닫기 버튼

    [Header("오브젝트풀")]
    [SerializeField] private GameObject filterGroupPrefab;
    [SerializeField] private GameObject sortGroupPrefab;
    [SerializeField] private Transform filterSortParent;

    private List<FilterGroupController> activeFilterGroups = new();
    private SortGroupController activeSortGroup;

    public event Action OnApplyClicked;

    public override void Init()
    {
        base.Init();
        applyBtn.onClick.AddListener(OnApplyBtnClicked);
        resetBtn.onClick.AddListener(OnResetBtnClicked);
        closeBtn.onClick.AddListener(OnCloseBtnClicked);
    }

    public override void SetData(object data)
    {
        if(data is FilterSortData initData)
        {
            ClearGroups();
            // 필터 그룹 대여
            foreach(var categoryData in initData.config.filterCategories)
            {
                var groupPrefab = PoolingManager.Instance.Get(filterGroupPrefab, filterSortParent);
                var group = groupPrefab.GetComponentInChildren<FilterGroupController>();

                initData.currentFilters.TryGetValue(categoryData.filterType, out var currentSelected);
                group.Init(categoryData, currentSelected);
                activeFilterGroups.Add(group);
            }
            // 정렬 그룹 대여
            if (initData.config.sortCategories.Count > 0)
            {
                var groupPrefab = PoolingManager.Instance.Get(sortGroupPrefab, filterSortParent);
                var group = groupPrefab.GetComponentInChildren<SortGroupController>();
                group.Init(initData.config.sortCategories, initData.currentSortType);
                activeSortGroup = group;
            }
        }
    }

    private void ClearGroups()
    {
        foreach (var filterGroup in activeFilterGroups)
        {
            filterGroup.ClearButtons(); 
            PoolingManager.Instance.Release(filterGroup.gameObject);
        }
        activeFilterGroups.Clear();

        if (activeSortGroup != null)
        {
            activeSortGroup.ClearButtons(); 
            PoolingManager.Instance.Release(activeSortGroup.gameObject);
            activeSortGroup = null;
        }
    }

    public Dictionary<FilterType, HashSet<string>> GetCurrentFilterData()
    {
        var allFilterData = new Dictionary<FilterType, HashSet<string>>();
        foreach (var filter in activeFilterGroups)
        {
            allFilterData.Add(filter.Type, new HashSet<string>(filter.GetSelectedFilters()));
        }
        return allFilterData;
    }

    public SortType GetCurrentSortData()
    {
        return activeSortGroup != null ? activeSortGroup.GetCurrentSortOption() : SortType.Rarity;
    }

    private void OnApplyBtnClicked()
    {
        OnApplyClicked?.Invoke();
        ClearGroups();
        popupAnimation.PlayCloseAnimation(() => 
            UIManager.Instance.Close<PopupFilterAndSort>());
    }

    private void OnResetBtnClicked()
    {
        foreach (var controller in activeFilterGroups)
        {
            controller.ResetToDefault();
        }
        activeSortGroup?.ResetToDefault();
    }
    private void OnCloseBtnClicked()
    {
        ClearGroups();
        popupAnimation.PlayCloseAnimation(() => 
            UIManager.Instance.Close<PopupFilterAndSort>());
    }
}
