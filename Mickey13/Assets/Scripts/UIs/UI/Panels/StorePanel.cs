using UnityEngine;

public class StorePanel : MonoBehaviour
{
    [Header("상점 전환 탭")]
    [SerializeField] TabPanel storeTab;

    public void Init()
    {
        storeTab.ClickTab(0);
    }

    private void OnEnable()
    {
        GameManager.Instance.Tutorial.StoreTutorial();
    }
}
