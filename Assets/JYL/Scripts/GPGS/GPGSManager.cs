using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UniRx;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Extensions;

public class GPGSManager : MonoBehaviour
{
    [SerializeField] private Button playGamesButton;
    private void Awake()
    {
        // 최신 GPGS는 Activate()를 먼저 호출해야 함
        PlayGamesPlatform.Activate();
        // 버튼에 이벤트 구독
        playGamesButton.OnClickAsObservable()
            .Subscribe(_ => GPGSAuth())
            .AddTo(this);
    }

    private void GPGSAuth() => PlayGamesPlatform.Instance.Authenticate(OnAuthenticated);

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
                    return;
                }

                Debug.Log("AuthCode 획득 성공: " + authCode);
                
                // 3. 발급받은 AuthCode로 Firebase 로그인 시도
                LoginToFirebase(authCode);
            });
        }
        else
        {
            Debug.LogWarning("GPGS 로그인 실패: " + status);
        }
    }
    
    private void LoginToFirebase(string authCode)
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        
        // GPGS에서 얻은 AuthCode를 Firebase Credential로 변환
        Credential credential = PlayGamesAuthProvider.GetCredential(authCode);

        // Firebase Auth 로그인 처리
        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Firebase 연동 실패: " + task.Exception);
                return;
            }

            // 최신 Firebase SDK (8.0 이상)에서는 task.Result.User 형태를 사용합니다.
            FirebaseUser newUser = task.Result.User;
            Debug.LogFormat("Firebase 연동 성공! 유저 이름: {0}, UID: {1}", newUser.DisplayName, newUser.UserId);

            // 4. 연동된 UID를 사용하여 세이브 데이터 로드 및 생성
            LoadOrCreateSaveData(newUser.UserId);
        });
    }

    private void LoadOrCreateSaveData(string uid)
    {
        // TODO: Firebase Realtime Database나 Firestore에 uid를 Key값으로 데이터를 요청합니다.
        Debug.Log($"[{uid}] 유저의 세이브 데이터를 파이어베이스에서 조회합니다.");
        
        // 예: DB에서 uid 경로를 조회 -> 데이터가 없으면 초기 Save 데이터를 생성해서 push
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
