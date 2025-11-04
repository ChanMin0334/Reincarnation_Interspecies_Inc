using System; 
using System.Collections.Generic;
using UnityEngine;

public class SortGroupController : MonoBehaviour
{
    [Header("오브젝트풀")]
    [SerializeField] private SortOptionButton buttonPrefab;
    [SerializeField] Transform buttonContent;

    private List<SortOptionButton> activeBtns = new();
    private SortType _currentSortType = SortType.Rarity;

    public event Action<SortType> onSortTypeChanged;

    public void Init(List<SortCategory> sortCategories, SortType currentSortType)
    {
        ClearButtons();

        _currentSortType = currentSortType;
        foreach(var sortData in sortCategories)
        {
            var btnPrefab = PoolingManager.Instance.Get(buttonPrefab.gameObject, buttonContent);
            var btn = btnPrefab.GetComponent<SortOptionButton>();

            btn.Init(sortData);
            btn.OnSortTypeSelected += HandleSortTypeBtnClicked;

            activeBtns.Add(btn);
        }
        UpdateAllBtn();
    }

    // 정렬 옵션 선택 버튼 클릭
    private void HandleSortTypeBtnClicked(SortType selectedSortType)
    {
        if (_currentSortType == selectedSortType) return;

        _currentSortType = selectedSortType;
        SaveLastSortType();

    }

    private void UpdateAllBtn()
    {
        foreach (var btn in activeBtns)
        {
            btn.UpdateButton(btn.Type == _currentSortType);
        }
    }

    // 현재 정렬 기준 내보내기
    public SortType GetCurrentSortOption()
    {
        return _currentSortType;
    }

    // 정렬 기준 초기화
    public void ResetToDefault()
    {
        _currentSortType = SortType.Rarity;
        SaveLastSortType();
    }

    private void SaveLastSortType()
    {
        PlayerPrefs.SetInt("LastSortType", (int)_currentSortType);

        UpdateAllBtn();
        onSortTypeChanged?.Invoke(_currentSortType);
    }

    public void ClearButtons()
    {
        foreach (var btn in activeBtns)
        {
            btn.OnSortTypeSelected -= HandleSortTypeBtnClicked;
            btn.ResetBtn();
            PoolingManager.Instance.Release(btn.gameObject);
        }
        activeBtns.Clear();
    }
}
