using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactChestController : MonoBehaviour
{
    [SerializeField] List<ArtifactChestUI> chests;

    private void Start()
    {
        if (ArtifactManager.Instance != null)
        {
            ArtifactManager.Instance.OnChestCountChanaged += OnChestCountChanged;
        }

        EventManager.Instance.StartListening(EventType.EndReincarnate, InitChests);

        // 시작 시 실제 상자 개수로 UI 갱신
        RefreshChestUI();
    }

    private void OnChestCountChanged(ArtifactChestType type, int count)
    {
        foreach(var chest in chests)
        {
            if( chest.ChestType == type )
            {
                chest.UpdateUI(count);
            }
        }
    }

    // 환생 시 상자 초기화
    private void InitChests()
    {
        foreach (var chest in chests)
        {
            chest.UpdateUI(0);
            chest.gameObject.SetActive(false);
        }
    }

    // 현재 상자 개수로 UI 갱신 (로드 후 호출)
    public void RefreshChestUI()
    {
        if (ArtifactManager.Instance == null) return;

        foreach (var chest in chests)
        {
            int count = ArtifactManager.Instance.GetChestCount(chest.ChestType);
            chest.UpdateUI(count);
            Debug.Log($"[ArtifactChestController] UI 갱신 - {chest.ChestType}: {count}개");
        }
    }
}
