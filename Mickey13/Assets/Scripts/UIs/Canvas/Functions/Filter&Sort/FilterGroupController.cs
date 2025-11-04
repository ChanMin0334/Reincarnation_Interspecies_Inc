using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FilterGroupController : MonoBehaviour
{
    [Header("오브젝트풀")]
    [SerializeField] private FilterOptionButton buttonPrefab;
    [SerializeField] Transform  buttonContent;

    [SerializeField] TextMeshProUGUI filterCategoryText;

    private List<FilterOptionButton> activeBtns = new();
    private FilterOptionButton defaultBtn;

    private HashSet<string> selectedFilters = new();
    public FilterType Type { get; private set; }

    public void Init(FilterCategory data, HashSet<string> currentSelectedFilters)
    {
        this.Type = data.filterType;
        filterCategoryText.text = data.displayName;
        ClearButtons();

        defaultBtn = CreateButton("기본", "전체", true);

        foreach(var filterOption in data.options)
        {
            CreateButton(filterOption.key, filterOption.displayName, false);
        }

        if ((currentSelectedFilters == null || currentSelectedFilters.Count == 0))
        {
            SelectedDefaultBtn();
        }
        else
        {
            this.selectedFilters = new HashSet<string>(currentSelectedFilters);
            defaultBtn.UpdateButton(false);
        }
        UpdateAllBtn();
    }

    // 필터 옵션 선택 버튼 클릭
    private void HandleFilterOptionBtnClicked(FilterOptionButton clickedBtn)
    {
        if(clickedBtn.IsDefaultBtn)
        {
            SelectedDefaultBtn();
        }
        else
        {
            defaultBtn.UpdateButton(false);

            if(selectedFilters.Contains(clickedBtn.FilterValue))
            {
                selectedFilters.Remove(clickedBtn.FilterValue);
            }
            else
            {
                selectedFilters.Add(clickedBtn.FilterValue);
            }
        }

        if(selectedFilters.Count == 0)
        {
            SelectedDefaultBtn();
        }

        UpdateAllBtn();
    }

    // 기본 필터 선택시
    private void SelectedDefaultBtn()
    {
        selectedFilters.Clear();
        defaultBtn.UpdateButton(true);
    }

    private void UpdateAllBtn()
    {
        foreach (var btn in activeBtns)
        {
            if(btn.IsDefaultBtn) continue;

            btn.UpdateButton(selectedFilters.Contains(btn.FilterValue));
        }
    }

    // 필터 기준 초기화
    public void ResetToDefault()
    {
        SelectedDefaultBtn();
        UpdateAllBtn();
    }

    // 현재 선택 필터 내보내기
    public HashSet<string> GetSelectedFilters()
    {
        return selectedFilters;
    }

    private FilterOptionButton CreateButton(string value, string text, bool isDefault)
    {
        var btnPrefab = PoolingManager.Instance.Get(buttonPrefab.gameObject, buttonContent);
        var btn = btnPrefab.GetComponent<FilterOptionButton>();

        btn.Init(value, text, isDefault);
        btn.OnButtonClicked += HandleFilterOptionBtnClicked;

        activeBtns.Add(btn);
        return btn;

    }

    public void ClearButtons()
    {
        foreach (var btn in activeBtns)
        {
            btn.OnButtonClicked -= HandleFilterOptionBtnClicked;
            btn.ResetBtn();
            PoolingManager.Instance.Release(btn.gameObject);
        }
        activeBtns.Clear();
    }
}
