using UnityEngine;
using UnityEngine.UI;

public class StageSelectPanel : UIPanel
{
    [Header("Buttons")]
    [SerializeField] private Button btnClose;

    [Header("World Tabs")]
    [SerializeField] private Button[] worldButtons;      // size = 5
    [SerializeField] private GameObject[] worldTabs;     // size = 5

    private GameObject currentTab;

    protected override void Awake()
    {
        base.Awake();

        btnClose.onClick.AddListener(() =>
        {
            UIManager.Instance.CloseTop();
        });

        for (int i = 0; i < worldButtons.Length; i++)
        {
            int worldIndex = i; // 클로저 방지
            worldButtons[i].onClick.AddListener(() =>
            {
                OpenWorld(worldIndex);
            });
        }
    }

    public override void OnOpen()
    {
        base.OnOpen();
        OpenWorld(0); // 기본 월드 = 1월드
    }

    private void OpenWorld(int index)
    {
        if (currentTab == worldTabs[index]) return;

        if (currentTab != null)
            currentTab.SetActive(false);

        currentTab = worldTabs[index];
        currentTab.SetActive(true);
    }
}
