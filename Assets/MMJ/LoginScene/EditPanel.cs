using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditPanel : MonoBehaviour
{
    [SerializeField] GameObject lobbyPanel;

    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TMP_InputField passInput;
    [SerializeField] TMP_InputField passConfirmInput;

    [SerializeField] TMP_Text emailContent;
    [SerializeField] TMP_Text userIdContent;

    [SerializeField] Button backButton;
    [SerializeField] Button nicknameConfirmButton;
    [SerializeField] Button passConfirmButton;

    private void Awake()
    {
        backButton.onClick.AddListener(Back);
        nicknameConfirmButton.onClick.AddListener(ChangeNicknameConfirm);
        passConfirmButton.onClick.AddListener(ChangePasswordConfirm);
    }

    private void OnEnable()
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;

        emailContent.text = user.Email;
        userIdContent.text = user.UserId;
        nameInput.text = user.DisplayName;
    }

    private void Back()
    {
        lobbyPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    private void ChangeNicknameConfirm()
    { 
        UserProfile profile = new UserProfile();
        profile.DisplayName = nameInput.text;

        FirebaseUser user = FirebaseManager.Auth.CurrentUser;
        user.UpdateUserProfileAsync(profile)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("닉네임 변경 취소");
                    ToastUtil.Error("닉네임 변경이 취소되었습니다.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"닉네임 변경 실패. 이유 : {task.Exception}");
                    ToastUtil.Error("닉네임 변경에 실패했습니다.");
                    return;
                }
                Debug.Log("닉네임 변경 완료");
                ToastUtil.Success("닉네임 변경 완료!");
            });
        
    }

    private void ChangePasswordConfirm()
    {
        if (passInput.text != passConfirmInput.text)
        {
            Debug.LogError("비밀번호가 비밀번호 확인과 일치하지 않음");
            ToastUtil.Error("비밀번호가 서로 일치하지 않습니다.");
            return;
        }

        FirebaseUser user = FirebaseManager.Auth.CurrentUser;
        user.UpdatePasswordAsync(passInput.text)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("비밀번호 변경 취소");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"비밀번호 변경 실패. 이유 : {task.Exception}");
                    return;
                }
                Debug.Log("비밀번호 변경 완료");
                ToastUtil.Success("비밀번호 변경 완료!");
            });

    }

}
