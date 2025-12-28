using Firebase.Extensions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResetPasswordPopup : UIPopup
{
    [SerializeField] TMP_InputField emailInput;
    [SerializeField] Button confirmButton;
    [SerializeField] Button cancelButton;

    private void Awake()
    {
        base.Awake();
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);
    }

    private void OnConfirm()
    {
        string email = emailInput.text.Trim();

        if (string.IsNullOrEmpty(email))
        {
            ToastUtil.Error("이메일을 입력해주세요.");
            return;
        }

        FirebaseManager.Auth
            .SendPasswordResetEmailAsync(email)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    ToastUtil.Error("비밀번호 재설정이 취소되었습니다.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError(task.Exception);
                    ToastUtil.Error("이메일 전송에 실패했습니다.");
                    return;
                }

                ToastUtil.Success("비밀번호 재설정 이메일을 전송했습니다.");
                Close();
            });
    }

    private void OnCancel()
    {
        Close();
    }

    private void Close()
    {
        UIManager.Instance.CloseTop();
    }
}
