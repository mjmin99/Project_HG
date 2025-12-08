using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject editPanel;
    
    [SerializeField] TMP_Text emailContent;
    [SerializeField] TMP_Text nameContent;
    [SerializeField] TMP_Text userIDContent;

    [SerializeField] Button logoutButton;
    [SerializeField] Button editProfileButton;
    [SerializeField] Button deleteUserButton;

    // --- 테스트 중인 기능

    [SerializeField] Button gameStartButton;

    // ---


    private void Awake()
    {
        logoutButton.onClick.AddListener(Logout);
        editProfileButton.onClick.AddListener(EditProfile);
        deleteUserButton.onClick.AddListener(DeleteUser);
        gameStartButton.onClick.AddListener(GameStart);
    }

    private void OnEnable()
    {
        // 로비 패널이 활성화 되었다는 뜻은 로그인이 성공했다는 뜻
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;
        emailContent.text = user.Email;
        nameContent.text = user.DisplayName;
        userIDContent.text = user.UserId;
    }


    private void Logout()
    {
        FirebaseManager.Auth.SignOut();
        loginPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    private void EditProfile()
    { 
        editPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    private void DeleteUser()
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;
        user.DeleteAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("유저 삭제 취소됨");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError($"유저 삭제 실패함. 이유 : {task.Exception}");
                    return;
                }
                Debug.LogError($"유저 삭제 성공함");
                FirebaseManager.Auth.SignOut();
                loginPanel.SetActive(true);
                gameObject.SetActive(false);
            });
    }

    // --- 테스트 중인 기능

    private void GameStart()
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;

        SaveManager.Instance.LoadFromFirebase(user.UserId, () =>
        {
            SceneChanger.Instance.LoadScene("MainScene");
        });
    }
    // ---

}
