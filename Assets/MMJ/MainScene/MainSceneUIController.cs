using UnityEngine;
using UnityEngine.UI;

public class MainSceneUIController : MonoBehaviour
{
    [SerializeField]private Button characterUIButton;

    private void Awake()
    {
        characterUIButton.onClick.RemoveAllListeners();

        characterUIButton.onClick.AddListener(OpenCharacterUI);
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
