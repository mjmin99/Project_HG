using UnityEngine;

public class LoginSceneController : MonoBehaviour
{
    public static LoginSceneController Instance { get; private set; }

    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject nicknamePanel;
    [SerializeField] GameObject emailPanel;

    private void Awake()
    {
        Instance = this;
    }

    public void ReturnToLogin()
    {
        // 모든 로그인 관련 패널 끄기
        lobbyPanel.SetActive(false);
        nicknamePanel.SetActive(false);
        emailPanel.SetActive(false);

        // 로그인 패널만 켜기
        loginPanel.SetActive(true);
    }
}
