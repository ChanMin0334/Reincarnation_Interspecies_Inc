using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    public bool isActiveOnLoad = false;
    public bool isDestroyOnClosed = false;

    public abstract void Init();
    public virtual void SetData(object data) { }
    public virtual void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public virtual void OnClickClose()
    {
        UIManager.Instance.Close(gameObject.name);
    }
}
