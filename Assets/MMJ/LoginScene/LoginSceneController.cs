using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class LoginSceneController : MonoBehaviour
{
    public static LoginSceneController Instance { get; private set; }

    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject nicknamePanel;
    [SerializeField] GameObject emailPanel;
    [SerializeField] private Button optionBtn;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        optionBtn.OnClickAsObservable().Subscribe(_=>ClickOptionBtn()).AddTo(this);
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
    
    private void ClickOptionBtn()
    {
        UIManager.Instance.OpenUI<OptionPanel>("OptionPanel");
    }
}
