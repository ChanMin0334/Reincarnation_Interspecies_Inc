using System.Collections.Generic;
using UnityEngine;

public class TabPanel : MonoBehaviour
{
    [Header("탭 버튼들 (탭 패널 순서와 맞춰야 함)")]
    public List<TabButton> tabBtns; // 탭버튼 리스트

    [Header("탭 패널들 (탭 버튼 순서와 맞춰야 함)")]
    public List<GameObject> contentsPanels; // 열리는 패널 리스트
    int selectedTabIdx = 0;

    private void Start()
    {
        ClickTab(selectedTabIdx);
    }

    /// <summary>
    /// idx 번호와 같은 번호의 패널 활성화, 다른 번호의 패널 비활성화
    /// </summary>
    /// <param name="idx"></param>
    public void ClickTab(int idx)
    {
        for(int i = 0; i < tabBtns.Count; i++)
        {
            if(i == idx)
            {
                selectedTabIdx = i;
                contentsPanels[i].SetActive(true);
                tabBtns[i].Selected();
            }
            else
            {
                contentsPanels[i].SetActive(false);
                tabBtns[i].DeSelected();
            }
        }
    }
}
