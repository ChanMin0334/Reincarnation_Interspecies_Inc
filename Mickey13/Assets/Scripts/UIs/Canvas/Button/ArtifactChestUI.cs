using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ArtifactChestType
{
    normal,
    special,
}

public class ArtifactChestUI : MonoBehaviour
{
    [SerializeField] ArtifactChestType chestType;
    [SerializeField] TextMeshProUGUI chestCountText;
    [SerializeField] Button ChestBtn;
    [SerializeField] Image ChestIcon;

    public ArtifactChestType ChestType => chestType;

    private void Start()
    {
        ChestBtn.onClick.AddListener(HandleChestClicked);
    }

    public void UpdateUI(int count)
    {
        bool hasChests = count > 0;

        this.gameObject.SetActive(hasChests);

        if(hasChests)
            chestCountText.text = count.ToString();
    }

    private void HandleChestClicked()
    {
        ArtifactManager.Instance.OpenChest(chestType, ChestIcon.sprite);
    }
}
