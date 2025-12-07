using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseTester : MonoBehaviour
{
    [SerializeField] Button testButton;

    [SerializeField] SaveData data;

    private void Awake()
    {
        testButton.onClick.AddListener(Test);    
    }

    private void Test()
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;

        DatabaseReference root = FirebaseManager.Database.RootReference;
        DatabaseReference userInfo = root.Child("Userdata").Child(user.UserId);

        string json = JsonUtility.ToJson(data);
        Debug.Log("json");

        userInfo.SetRawJsonValueAsync(json);
    }
}
