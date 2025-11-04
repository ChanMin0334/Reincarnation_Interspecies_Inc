using System;
using UnityEngine;

[Serializable]
public class StatDefinition
{
    public StatType type; // 스탯 종류
    public string statName; // 스탯 이름
    public StatCategory category; // 스탯 카테고리
    public Sprite statIcon; // 스탯 아이콘
    public string valueSuffix; // 스탯 표기 단위
}
