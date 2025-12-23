using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NicknamePanel : MonoBehaviour
{
    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject lobbyPanel;

    [SerializeField] TMP_InputField nicknameInput;

    [SerializeField] Button confirmButton;
    [SerializeField] Button backButton;

    private void Awake()
    {
        confirmButton.onClick.AddListener(Confirm);
        backButton.onClick.AddListener(Back);
    }

    private void Confirm()
    {
        UserProfile profile = new UserProfile();
        profile.DisplayName = nicknameInput.text;

        FirebaseUser user = FirebaseManager.Auth.CurrentUser;
        user.UpdateUserProfileAsync(profile)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("유저 닉네임 설정 취소됨");
                    ToastUtil.Error("닉네임 설정이 취소되었습니다.");
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"유저 닉네임 설정 실패. 이유 : {task.Exception}");
                    ToastUtil.Error("닉네임 설정에 실패했습니다.");
                }
                Debug.Log("유저 닉네임 설정 성공");
                ToastUtil.Success("닉네임 설정 완료!");
                lobbyPanel.SetActive(true);
                gameObject.SetActive(false);
            });

    }

    private void Back()
    {
        FirebaseManager.Auth.SignOut();
        loginPanel.SetActive(true);
        gameObject.SetActive(false);
    }

}
