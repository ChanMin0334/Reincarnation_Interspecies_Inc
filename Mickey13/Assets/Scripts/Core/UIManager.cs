using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eUIPosition // UI 종류별 생성 위치
{
    UI,
    Popup,
}
public class UIManager : Singleton<UIManager>
{
    [SerializeField] private List<Transform> uiPosition; // UI오브젝트 또는 Popup 오브젝트의 자식으로 생성

    List<UIBase> uiList = new List<UIBase>();

    #region UI Generic

    public T Open<T>() where T : UIBase
    {
        return Open<T>(null);
    }

    public T Open<T>(object data) where T : UIBase
    {
        UIBase ui = uiList.Find(obj => obj.name == typeof(T).Name); // UI리스트에서 타입이 일치는 UI 찾기

        if(ui == null) // UI가 없다면 Resources에서 로드
        {
            ui = Instantiate(Resources.Load<T>("UI/" + typeof(T).Name), uiPosition[0].parent); // 타입 이름과 프리팹 이름 일치해야함
            ui.name = ui.name.Replace("(Clone)", ""); // clone 문자열 제거
            uiList.Add(ui); // 리스트에 UI 추가
            ui.Init(); // UI 초기화
        }

        bool isPopup = ui.name.Contains("Popup"); // Ui / Popup 구분

        ui.transform.SetParent(uiPosition[ui.name.Contains("Popup") ? (int)eUIPosition.Popup : (int)eUIPosition.UI]); //UI를 자식으로 생성할 부모 오브젝트

        foreach (var old in uiList) // UI 변경시 기존 UI 비활성화
        {
            if(!isPopup && !old.name.Contains("Popup") )
            {
                old.SetActive(false);
            }
        }

        if(data != null)
        {
            ui.SetData(data);
        }
        ui.transform.SetAsLastSibling();
        ui.SetActive(true); // UI 활성화

        return (T)ui;
    }

    public void Close<T>() where T : UIBase
    {
        UIBase ui = uiList.Find(obj => obj is T); // UI리스트에서 타입이 일치하는 UI 찾기
        
        if( ui != null )
        {
            if(ui.isDestroyOnClosed) // UI가 닫히고 파괴어야한다면
            {
                uiList.Remove(ui);
                Destroy(ui.gameObject); // 파괴
            }
            else
            {
                ui.SetActive(false); // 아니면 비활성화
            }
        }
    }

    /// <summary>
    /// 특정 UI를 참조해야할 때 사용
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetUI<T>() where T: UIBase // UI를 참조해야하는 경우
    {
        UIBase ui = uiList.Find(obj => obj is T);
        return(T)ui;
    }

    public bool IsOpened<T>() where T : UIBase
    {
        UIBase ui = uiList.Find(obj => obj is T);
        return ui.gameObject.activeInHierarchy;
    }

    #endregion

    public void Close(string name)
    {
        UIBase ui = uiList.Find(obj => obj.gameObject.name == name);
        if (ui != null)
        {
            ui.SetActive(false);
            uiList.Remove(ui);
        }
    }
}
