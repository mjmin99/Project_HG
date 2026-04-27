using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UniRx;
using Firebase.Auth;
using Firebase.Extensions;

public class GPGSManager : MonoBehaviour
{
    [SerializeField] private Button playGamesButton;
    [SerializeField] GameObject nicknamePanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject loadingPanel;
    
    private void Awake()
    {
        // 최신 GPGS는 Activate()를 먼저 호출해야 함
        PlayGamesPlatform.Activate();
        
        // 버튼에 이벤트 구독
        playGamesButton.OnClickAsObservable().Subscribe(_ => GPGSAuth()).AddTo(this);
        // 파이어베이스가 초기화 되었을 때 수행
        FirebaseManager.IsInitialized
            .Where(x=>x==true)
            .Take(1)
            .Subscribe(_=>GPGSAuth())
            .AddTo(this);
    }

    private void GPGSAuth()
    {
        // 다중 입력 방지를 위한 로딩 패널 활성화
        loadingPanel.SetActive(true);
        PlayGamesPlatform.Instance.Authenticate(OnAuthenticated);
        Manager.Audio.PlaySfx("SFX_OK");
    }
        

    private void OnAuthenticated(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            Debug.Log("GPGS 로그인 성공");

            // 2. 파이어베이스 연동을 위한 Server Auth Code 요청 (최신 버전 방식)
            // 첫 번째 파라미터(forceRefreshToken): true로 설정하여 항상 새로운 코드를 받도록 권장
            PlayGamesPlatform.Instance.RequestServerSideAccess(true, authCode =>
            {
                if (string.IsNullOrEmpty(authCode))
                {
                    Debug.LogError("AuthCode를 받아오지 못했습니다. (GCP Web Client ID 셋업 확인 필요)");
                    loadingPanel.SetActive(false);
                    return;
                }

                Debug.Log("AuthCode 획득 성공: " + authCode);
                
                // 3. 발급받은 AuthCode로 Firebase 로그인 시도`
                LoginToFirebase(authCode);
            });
        }
        else
        {
            loadingPanel.SetActive(false);
            Debug.LogWarning("GPGS 로그인 실패: " + status);
            ToastUtil.Error("로그인이 필요합니다!");
        }
    }
    
    private void LoginToFirebase(string authCode)
    {
        var auth = FirebaseManager.Auth;
        
        // GPGS에서 얻은 AuthCode를 Firebase Credential로 변환
        Credential credential = PlayGamesAuthProvider.GetCredential(authCode);

        // Firebase Auth 로그인 처리
        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                loadingPanel.SetActive(false);
                Debug.LogError("Firebase 연동 실패: " + task.Exception);
                ToastUtil.Error("로그인이 먼저 필요합니다!");
                return;
            }

            // 최신 Firebase SDK (8.0 이상)에서는 task.Result 형태
            FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase 연동 성공! 유저 이름: {0}, UID: {1}", newUser.DisplayName, newUser.UserId);
            loadingPanel.SetActive(false);
            // 4. 연동된 UID를 사용하여 세이브 데이터 로드 및 생성
            LoadOrCreateSaveData(newUser.UserId);
        });
    }

    private void LoadOrCreateSaveData(string uid)
    {
        // 1. 아직 닉네임을 설정하지 않은 경우
        if (string.IsNullOrEmpty(uid))
        {
            Debug.Log("닉네임 설정이 필요합니다.");
            nicknamePanel.SetActive(true);
        }
        // 2. 이미 설정된 경우
        else
        {
            lobbyPanel.SetActive(true);
        }
        
        gameObject.SetActive(false);
    }

    // private void OnAuthenticated(SignInStatus status)
    // {
    //     if (status == SignInStatus.Success)
    //     {
    //         Debug.Log("로그인 성공");
    //         var id = PlayGamesPlatform.Instance.localUser.id;
    //         var authCode = PlayGamesPlatform.Instance.GetServerAuthCode(); // 이거 안됨
    //     }
    //     else
    //     {
    //         Debug.LogWarning("로그인 실패");
    //     }
    // }
}
