using UnityEngine;
using UnityEngine.UI;

public class MainSceneUIController : MonoBehaviour
{
    [SerializeField] private Button characterUIButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        characterUIButton.onClick.AddListener(OpenCharacterUI);
        exitButton.onClick.AddListener(CloseCharacterUI);
    }

    // 메인씬 캐릭터 버튼
    public void OpenCharacterUI()
    {
        UIManager.Instance.OpenUI<PartySetupPanel>("PartySetupPanel");
    }

    // 닫기 버튼
    public void CloseCharacterUI()
    {
        UIManager.Instance.CloseTop(); // PartySetupPanel 하나만 닫음
    }
}
