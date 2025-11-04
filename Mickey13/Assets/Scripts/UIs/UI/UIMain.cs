using UnityEngine;
using UnityEngine.UI;

public class UIMain : UIBase
{
    [Header("상단 메뉴")]
    [SerializeField] Button settingBtn;

    [Header("하단 메뉴")]
    [SerializeField] TabPanel tabPanel;

    [Header("하단 메뉴 패널")]
    [SerializeField] private EnhancePanel enhancePanel; // 강화 탭 - 편성 캐릭터 슬롯UI
    [SerializeField] private InventoryPanel inventoryPanel;
    [SerializeField] private GachaPanel gachaPanel;
    [SerializeField] private StorePanel storePanel;

    public EnhancePanel EnhancePanel => enhancePanel;

    public override void Init()
    {
        settingBtn.onClick.RemoveAllListeners();
        settingBtn.onClick.AddListener(OnClickSettingBtn);
        tabPanel.ClickTab(0); // 시작시 항상 0번 패널 활성화
        // gachaPanel.Init();
        enhancePanel.Init();
        inventoryPanel.Init();
        storePanel.Init();
    }

    public void OnClickSettingBtn()
    {
        UIManager.Instance.Open<PopupSetting>();
    }
}
