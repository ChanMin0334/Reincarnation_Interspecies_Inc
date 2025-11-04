using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "StatDatabase", menuName = "Stats/StatDatabase")]
public class StatDatabaseSO : ScriptableObject
{
    public List<StatDefinition> statDefinitions;
    public StatDefinition GetDefinition(StatType type) // 스탯 타입으로 스탯 정보 탐색
    {
        return statDefinitions.FirstOrDefault(stat => stat.type == type);
    }
}
