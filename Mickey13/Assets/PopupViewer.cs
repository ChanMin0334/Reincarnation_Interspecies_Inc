using UnityEngine;
using UnityEngine.UI;

public class PopupViewer : PopupBase
{
    [SerializeField] private Image charImage;
    [SerializeField] private Button exitBtn;

    private void OnEnable()
    {
        exitBtn.onClick.AddListener(OnClickExit);
        
    }
    private void OnDisable()
    {
        exitBtn.onClick.RemoveListener(OnClickExit);
    }

    public void Setup(CharacterUIData charData)
    {
        Clear();
        //charImage.sprite = charData.SO.Sprite;
        charImage.sprite = charData.SO.FullSprite;
    }

    private void Clear()
    {  
        charImage.sprite = null;
    }

    private void OnClickExit()
    {
        Clear();
        UIManager.Instance.Close<PopupViewer>();
    }
}
