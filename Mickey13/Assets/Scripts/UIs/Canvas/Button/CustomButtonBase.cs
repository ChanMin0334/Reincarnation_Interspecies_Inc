using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CustomButtonBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public virtual void OnPointerEnter(PointerEventData eventData) { }

    public virtual void OnPointerClick(PointerEventData eventData) { }
    // {
    //     AudioManager.Instance.PlaySFX(SfxType.Button_Click);
    // }

    public virtual void OnPointerExit(PointerEventData eventData) { }
}
