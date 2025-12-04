using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] GameObject loginPanel;
    
    [SerializeField] TMP_Text emailContent;
    [SerializeField] TMP_Text nameContent;
    [SerializeField] TMP_Text userIDContent;

    [SerializeField] Button logoutButton;
    [SerializeField] Button editProfileButton;

    private void Awake()
    {
        logoutButton.onClick.AddListener(Logout);
        editProfileButton.onClick.AddListener(EditProfile);
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
        
    }
}
