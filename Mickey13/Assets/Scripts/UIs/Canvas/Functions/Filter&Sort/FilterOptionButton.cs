using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FilterOptionButton : MonoBehaviour
{
    [SerializeField] Image buttonImage;
    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite pressedSprite;
    [SerializeField] TextMeshProUGUI displayName;

    public bool IsDefaultBtn {  get; private set; }
    public string FilterValue { get; private set; }

    private Button button;

    public event Action<FilterOptionButton> OnButtonClicked;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => { OnButtonClicked?.Invoke(this); });
    }

    public void Init(string filterValue, string displayText, bool isDefaultBtn = false)
    {
        FilterValue = filterValue;
        IsDefaultBtn = isDefaultBtn;
        displayName.text = displayText;

        UpdateButton(isDefaultBtn);
    }

    public void UpdateButton(bool isSelected)
    {
        buttonImage.sprite = isSelected ? pressedSprite : normalSprite;
    }

    public void ResetBtn()
    {
        OnButtonClicked = null;
    }
}
