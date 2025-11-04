using System.Collections.Generic;

public class FilterSortData
{
    public FilterSortSO config; // 필터/정렬 설정 파일
    public Dictionary<FilterType, HashSet<string>> currentFilters; // 현재 적용중인 필터
    public SortType currentSortType; // 현재 적용중인 정렬 기준
}
