using System;
using TMPro; 
using UnityEngine;
using UnityEngine.UI;

public class SortOptionButton : MonoBehaviour
{
    [SerializeField] Image buttonImage;
    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite pressedSprite;
    [SerializeField] TextMeshProUGUI displayName;

    private Button button;

    public event Action<SortType> OnSortTypeSelected;

    public SortType Type { get; private set; }


    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => { OnSortTypeSelected?.Invoke(Type); });
    }

    public void Init(SortCategory data)
    {
        this.Type = data.sortType;
        displayName.text = data.displayName;

        UpdateButton(false);
    }

    public void UpdateButton(bool isSelected)
    {
        buttonImage.sprite = isSelected ? pressedSprite : normalSprite;
    }

    public void ResetBtn()
    {
        OnSortTypeSelected = null;
    }
}
