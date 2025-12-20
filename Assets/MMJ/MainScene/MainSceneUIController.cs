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
        UIManager.Instance.OpenUI<CharacterListPanel>("CharacterListPanel");
    }

    public void CloseCharacterUI()
    {
        UIManager.Instance.CloseTop();
    }
}
