using System;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    [Header("인벤토리 카테고리")]
    [SerializeField] CharacterTabPanel characterPanel; // 캐릭터 인벤토리
    [SerializeField] ArtifactTabPanel artifactPanel; // 유물 인벤토리
    [SerializeField] RuneTabPanel runePanel; // 룬 인벤토리

    [Header("인벤토리 전환 탭")]
    [SerializeField] TabPanel InventoryTab; // 캐릭터,유물,룬

    public void Init()
    {
        InventoryTab.ClickTab(0); // 처음 열릴때는 캐릭터 인벤토리가 먼저 열리기
    }

    private void OnEnable()
    {
        GameManager.Instance.Tutorial.InventoryTutorial();
    }
}
