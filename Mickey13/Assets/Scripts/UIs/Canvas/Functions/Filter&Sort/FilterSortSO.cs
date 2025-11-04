using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class option
{
    public string key;
    public string displayName;
}

[Serializable]
public class FilterCategory
{
    public FilterType filterType;
    public string displayName;
    public List<option> options;
}

[Serializable]
public class SortCategory
{
    public SortType sortType;
    public string displayName;
}


[CreateAssetMenu(fileName = "FilterSortSO",menuName = "UI/Filter Sort SO")]
public class FilterSortSO : ScriptableObject
{
    [Header("필터 카테고리")]
    public List<FilterCategory> filterCategories;

    [Header("정렬 카테고리")]
    public List<SortCategory> sortCategories;
}
