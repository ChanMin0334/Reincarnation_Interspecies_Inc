using System;
using System.Collections.Generic;
using UnityEngine;

public enum LootKind
{
    Gold,
    RelicChest
}

[Serializable]
public struct DropEntry
{
    public LootKind kind;
    public float prob;
    public int minAmount;
    public int maxAmount;
}

[CreateAssetMenu(menuName = "Game/DropTable")]
public class DropTable : ScriptableObject
{
    //0번 인덱스는 골드
    //1번 인덱스는 유물상자
    public List<DropEntry> entries = new();
}
