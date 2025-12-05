using System.Collections;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class EmailPanel : MonoBehaviour
{
    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject nicknamePanel;

    [SerializeField] Button backButton;

    private void Awake()
    {
        backButton.onClick.AddListener(Back);
    }

    private void OnEnable()
    {
        FirebaseManager.Auth.CurrentUser.SendEmailVerificationAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("인증 이메일 전송이 취소됨");
                }
                if (task.IsFaulted)
                {
                    Debug.Log($"인증 이메일 전송이 실패. 이유 : {task.Exception}");
                }
                Debug.Log("인증 이메일 전송 성공");

                emailVerificaionRoutine = StartCoroutine(EmailVerificaionRoutine());
                
            });
    }

    private void Back()
    {
        FirebaseManager.Auth.SignOut();
        loginPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    Coroutine emailVerificaionRoutine;

    IEnumerator EmailVerificaionRoutine()
    { 
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;
        WaitForSeconds deley = new WaitForSeconds(2f);

        while (true)
        {
            yield return deley;

            user.ReloadAsync();
            if (user.IsEmailVerified)
            {
                Debug.Log("인증완료");
                nicknamePanel.SetActive(true);
                gameObject.SetActive(false);
                StopCoroutine(emailVerificaionRoutine);
            }
            else 
            {
                Debug.Log("인증 대기중...");
            }
        }
    }
}
