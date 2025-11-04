using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    Image background;
    public Sprite idleImage; // 누르지 않았을 때 탭 모양
    public Sprite selectedImage; // 눌렀을 탭 모양

    private void Awake()
    {
        background = GetComponent<Image>();
    }

    public void Selected()
    {
        if(background == null)
            background = GetComponent<Image>();
        
        if (selectedImage != null)
        {
            background.sprite = selectedImage;
            background.color = new Color(1f,1f,1f,1f);
        }
    }

    public void DeSelected()
    {
        if(background == null)
            background = GetComponent<Image>();
        
        if (idleImage != null)
        {
            background.sprite = idleImage;
            background.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        }
    }
}
