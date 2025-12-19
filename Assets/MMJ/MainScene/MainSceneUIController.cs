using UnityEngine;
using UnityEngine.UI;

public class MainSceneUIController : MonoBehaviour
{
    [Header("Character UI")]
    public GameObject characterPanelRoot; // PartyUI + CharacterListUI 부모

    [SerializeField] Button characterUIButton;
    [SerializeField] Button exitButton;

    private void Awake()
    {
        characterUIButton.onClick.AddListener(OpenCharacterUI);
        exitButton.onClick.AddListener(CloseCharacterUI);
    }

    private void Start()
    {
        // 메인씬 진입 시 닫힌 상태
        characterPanelRoot.SetActive(false);
    }

    // 메인씬 캐릭터 버튼
    public void OpenCharacterUI()
    {
        UIManager.Instance.OpenUI<PartyPanel>("PartyPanel");
        UIManager.Instance.OpenUI<CharacterListPanel>("CharacterListPanel");
    }

    // 닫기 버튼
    public void CloseCharacterUI()
    {
        UIManager.Instance.CloseTop(); // List
        UIManager.Instance.CloseTop(); // Party
    }
}
