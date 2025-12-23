using UnityEngine;

public class UITestRunner : MonoBehaviour
{
    private void Start()
    {
        Debug.Log(Resources.Load("UI/Popups/OptionPopup"));
        Debug.Log(Resources.Load("UI/Toasts/SimpleToast"));
    }

    private void Update()
    {
        // 1 → 플레이어 상태 패널 열기
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var panel = UIManager.Instance
                .OpenUI<PlayerStatusPanel>("PlayerStatusPanel");

            panel.SetData(new PlayerData
            {
                name = "Hero",
                hp = 320,
                attack = 75
            });
        }

        // 2 → 팝업 열기
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UIManager.Instance
                .OpenUI<UIPopup>("OptionPopup");
        }

        // 3 → 토스트
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UIManager.Instance.ShowToast(
                "SimpleToast",
                "테토! 테스트 토스트란뜻~!",
                2f
            );
        }
    }
}
