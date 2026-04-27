using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class LoginSceneController : MonoBehaviour
{
    public static LoginSceneController Instance { get; private set; }

    [SerializeField] private GameObject GPGSLoginPanel;
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
        loginPanel.SetActive(false);

        // GPGS 로그인 대기 패널 켜기
        GPGSLoginPanel.SetActive(true);
    }
    
    private void ClickOptionBtn()
    {
        UIManager.Instance.OpenUI<OptionPanel>("OptionPanel");
    }
}
