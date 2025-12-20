using UnityEngine;
using UnityEngine.UI;

public class MainSceneUIController : MonoBehaviour
{
    [SerializeField]private Button characterUIButton;
    [SerializeField]private Button exitButton;

    private void Awake()
    {
        characterUIButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();

        characterUIButton.onClick.AddListener(OpenCharacterUI);
        exitButton.onClick.AddListener(CloseCharacterUI);
    }

    public void OpenCharacterUI()
    {
        //  열고 싶은 패널은 CharacterListUI가 아니라, 그걸 포함한 UIPanel 래퍼
        UIManager.Instance.OpenUI<CharacterListPanel>("CharacterListPanel");
    }

    public void CloseCharacterUI()
    {
        UIManager.Instance.CloseTop();
    }
}
