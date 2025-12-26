using Firebase.Extensions;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDeleteUserPopup : UIPopup
{
    [SerializeField] Button confirmButton;
    [SerializeField] Button cancelButton;

    private void Awake()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);
    }

    private void OnConfirm()
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;

        if (user == null)
        {
            ToastUtil.Error("유저 정보가 없습니다.");
            return;
        }

        user.DeleteAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    ToastUtil.Error("계정 삭제가 취소되었습니다.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError(task.Exception);
                    ToastUtil.Error("계정 삭제에 실패했습니다. 다시 로그인해주세요.");
                    return;
                }

                ToastUtil.Success("계정이 삭제되었습니다.");
                FirebaseManager.Auth.SignOut();

                UIManager.Instance.CloseTop();
                LoginSceneController.Instance.ReturnToLogin();
            });
    }

    private void OnCancel()
    {
        UIManager.Instance.CloseTop();
    }
}
