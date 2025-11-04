using UnityEngine;

public abstract class SlotBase<T> : MonoBehaviour where T : class
{
    // 공통으로 사용할 데이터
    protected T _data;
    public T Data => _data;

    public virtual void Setup(T data)
    {
        _data = data;

        if (_data == null)
        {
            // 데이터가 null이면 Clear 로직을 타도록 처리
            Clear();
            return;
        }

        gameObject.SetActive(true);
        UpdateUI();
    }

    protected abstract void UpdateUI();

    public virtual void Clear()
    {
        _data = null;
    }

    public bool IsEmpty()
    {
        return _data == null;
    }
}
