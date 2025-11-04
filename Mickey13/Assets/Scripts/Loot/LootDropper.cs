// LootDropper.cs

using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 이거 나중에 통합하면서 Enemy 쪽에 넣어도 됨, Target같은거는 매니저같은거로 따로 빼는게?
/// </summary>
public class LootDropper : MonoBehaviour
{
    [SerializeField] DropTable table;
    [SerializeField] Transform dropOrigin;

    [Header("UI")]
    [SerializeField] RectTransform goldUiTarget; // 안 써도 되지만 남겨둬도 OK
    [SerializeField] Camera uiCamera;            // Overlay면 null
    [SerializeField] GameObject coinPrefab;

    [Header("World Anchor")]
    [SerializeField] Transform goldWorldAnchor; 

    bool _handled = false;
    
    private void OnEnable()
    {
        _handled =  false;
    }

    public void Init
        (DropTable _table, Transform _dropOrigin,
        RectTransform _goldUiTarget, Camera _uiCamera, 
        GameObject _coinPrefab, Transform _goldWorldAnchor)
    {
        table = _table;
        dropOrigin = _dropOrigin;
        goldUiTarget = _goldUiTarget;
        uiCamera = _uiCamera;
        coinPrefab = _coinPrefab;
        goldWorldAnchor = _goldWorldAnchor;
    }

    public void OnDeath(int Level)
    {
        // 한 번만 처리
        if (_handled) return; 
        _handled = true; //테스트위해서 주석

        foreach (var e in table.entries)
        {
            if (Random.value > e.prob) continue;
            int amount = Random.Range(e.minAmount, e.maxAmount + 1);

            //amount *= Level; // 레벨 비례 드랍
            amount = (int)Math.Round(amount * Math.Pow(1.03f, Level - 1)); //레벨마다 일정 배율만큼 복리로 증가?
            if (e.kind == LootKind.Gold)
            {
                GoldSpawner.SpawnBurst
                (
                    dropOrigin.position, amount,
                    goldUiTarget, uiCamera,
                    coinPrefab, 
                    goldWorldAnchor,
                    e.minAmount, e.maxAmount
                );
            }
            // 유물상자는 나중에
            //else if (e.kind == LootKind.RelicChest)
            //{
            //    RelicChestSpawner.Spawn(transform.position);
            //}
        }
    }

#if UNITY_EDITOR
    void OnValidate() { if (!dropOrigin) dropOrigin = transform; }
#endif
}
