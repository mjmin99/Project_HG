using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] GameObject signUpPanel;
    [SerializeField] GameObject lobbyPanel;

    [SerializeField] TMP_InputField idInput;
    [SerializeField] TMP_InputField passInput;

    [SerializeField] Button signUpButton;
    [SerializeField] Button loginButton;


    private void Awake()
    {
        signUpButton.onClick.AddListener(SignUp);
        loginButton.onClick.AddListener(Login);
    }

    private void SignUp()
    { 
        signUpPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    private void Login()
    {
        FirebaseManager.Auth.SignInWithEmailAndPasswordAsync(idInput.text, passInput.text)
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("로그인이 취소됨");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"로그인이 실패함. 이유 : {task.Exception}");
                return;
            }

            Debug.Log("로그인 성공");
            AuthResult result = task.Result;
            FirebaseUser User = result.User;
            Debug.Log($"-----유저 정보-----");
            Debug.Log($"유저 이름 : {User.DisplayName}");
            Debug.Log($"유저ID : {User.UserId}");
            Debug.Log($"이메일 : {User.Email}");
            Debug.Log($"이메일 인증여부 : {User.IsEmailVerified}");

            lobbyPanel.SetActive(true);
            gameObject.SetActive(false);
        });


    }
}
