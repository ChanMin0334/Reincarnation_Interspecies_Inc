public abstract class PopupBase : UIBase
{
    protected PopupAnimation popupAnimation;
    public override void Init()
    {
        popupAnimation = GetComponentInChildren<PopupAnimation>();
    }
}
