using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OptionPanel : UIPanel
{
    [Header("Buttons")]
    [SerializeField] private Button btnClose;

    [Header("Tabs")]
    [SerializeField] private Button btnSound;
    [SerializeField] private Button btnHowToPlay;

    [Header("Tab Contents")]
    [SerializeField] private GameObject soundTab;
    [SerializeField] private GameObject howToPlayTab;

    private GameObject currentTab;

    protected override void Awake()
    {
        base.Awake();

        btnClose.onClick.AddListener(() =>
        {
            UIManager.Instance.CloseTop();
        });

        btnSound.onClick.AddListener(() =>
        {
            OpenTab(soundTab);
        });

        btnHowToPlay.onClick.AddListener(() =>
        {
            OpenTab(howToPlayTab);
        });
    }

    public override void OnOpen()
    {
        base.OnOpen();
        OpenTab(soundTab); // 기본 진입 탭
    }

    private void OpenTab(GameObject tab)
    {
        if (currentTab == tab) return;

        if (currentTab != null)
            currentTab.SetActive(false);

        currentTab = tab;
        currentTab.SetActive(true);
    }
}
