using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseInventoryUI<TData, TSlot> : MonoBehaviour where TData : IInventoryData where TSlot : Component
{ 
    [Header("필터/정렬 설정")]
    [SerializeField] protected FilterSortSO filterSortSO;
    [SerializeField] protected Button filterSortBtn; // 필터/정렬 팝업 버튼
    [SerializeField] protected Toggle sortToggle; // 오름차순 내림차순 정렬 토글

    protected Dictionary<FilterType, HashSet<string>> currentFilters = new(); // 필터 규칙 저장용 딕셔너리
    protected SortType currentSortType; // 기본 정렬값
    protected bool isAscending; // 기본 정렬 순서 (내림차순)

    protected Dictionary<FilterType, Func<TData, string>> filterSelectors = new(); // 필터 로직 저장용 딕셔너리
    protected Dictionary<SortType, Func<TData, object>> sortSelectors = new(); // 정렬 로직 저장용 딕셔너리

    protected Dictionary<string, TSlot> activeSlots = new(); // 업데이트가 필요한 슬롯 목록

    [Header("오브젝트풀")]
    [SerializeField] protected TSlot prefab; // 슬롯 프리팹
    [SerializeField] protected Transform content; // 슬롯 프리팹 생성 위치

    public event Action OnInventoryRefreshed;

    protected virtual void Awake()
    {
        LoadSortSetting();
        InitFilterSort();
    }

    protected virtual void OnEnable()
    {
        Refresh();
        filterSortBtn.onClick.AddListener(OnFilterSortBtnClick);
        sortToggle.onValueChanged.AddListener(HandleSortToggleChanged);
    }

    protected virtual void OnDisable()
    {
        ClearAllSlots();
        filterSortBtn.onClick.RemoveAllListeners();
        sortToggle.onValueChanged.RemoveAllListeners();
    }

    /// <summary>
    /// 해당하는 인벤토리에 맞는 필터/정렬 규칙 설정
    /// </summary>
    protected abstract void InitFilterSort();

    /// <summary>
    /// 헤딩하는 인벤토리 목록 데이터 가져오기
    /// </summary>
    protected abstract IEnumerable<TData> GetInventoryData();

    /// <summary>
    /// 생성된 슬롯에 데이터 주입
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="data"></param>
    protected abstract void SetupSlot(TSlot slot, TData data);

    /// <summary>
    /// 인벤토리 데이터를 담을 슬롯 생성(Instantiate or Release(오브젝트풀링)
    /// </summary>
    protected abstract TSlot CreateSlot();

    /// <summary>
    /// 슬롯 제거 (Destroy or Release(오브젝트풀링)
    /// </summary>
    protected abstract void ClearAllSlots();

    protected void Refresh()
    {
        ClearAllSlots();

        IEnumerable<TData> items = GetInventoryData();

        // 필터 조건
        if (currentFilters != null)
        {
            foreach (var filter in currentFilters)
            {
                if (filter.Value.Count == 0) continue; // 선택한 필터가 없으면 통과

                var selector = filterSelectors[filter.Key];
                items = items.Where(c => filter.Value.Contains(selector(c)));
            }
        }

        // 정렬 조건
        if (sortSelectors.TryGetValue(currentSortType, out var selectorKey))
        {
            items = isAscending ?
                items.OrderBy(selectorKey) :
                items.OrderByDescending(selectorKey);
        }

        foreach (var item in items)
        {
            TSlot slot = CreateSlot();
            SetupSlot(slot, item);

            activeSlots.Add(item.ID, slot);
        }

        OnInventoryRefreshed?.Invoke();
    }

    public void OnFilterSortBtnClick()
    {
        var filterSortData = new FilterSortData
        {
            config = filterSortSO,
            currentFilters = this.currentFilters,
            currentSortType = this.currentSortType,
        };

        var popup = UIManager.Instance.Open<PopupFilterAndSort>(filterSortData);
        popup.OnApplyClicked += HandleApplyFilterAndSort;
    }

    private void HandleApplyFilterAndSort()
    {
        var popup = UIManager.Instance.GetUI<PopupFilterAndSort>();
        if (popup == null) return;

        currentFilters = popup.GetCurrentFilterData();
        currentSortType = popup.GetCurrentSortData();
        isAscending = sortToggle.isOn;

        popup.OnApplyClicked -= HandleApplyFilterAndSort;

        SaveSortSetting();
        Refresh();
    }
    private void HandleSortToggleChanged(bool isOn)
    {
        isAscending = isOn;
        SaveSortSetting();
        Refresh();
    }

    #region 필터/정렬 설정 저장 PlayerPrefs

    private void LoadSortSetting()
    {
        currentSortType = (SortType)PlayerPrefs.GetInt(("InventorySortType"), (int)SortType.Rarity);
        isAscending = PlayerPrefs.GetInt(("InventorySortAscending"), 0) == 1;

        if(sortToggle != null)
        {
            sortToggle.isOn = isAscending;
        }
    }

    private void SaveSortSetting()
    {
        PlayerPrefs.SetInt(("InventorySortType"), (int)currentSortType);
        PlayerPrefs.SetInt(("InventorySortAscending"), isAscending ? 1 : 0 );
    }

    #endregion
}
