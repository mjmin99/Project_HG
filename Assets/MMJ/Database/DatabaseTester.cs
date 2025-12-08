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
        // 여기에 작성한 방식은 한번에 모든 내용을 다 보냄 
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;

        DatabaseReference root = FirebaseManager.Database.RootReference;
        DatabaseReference userInfo = root.Child("Userdata").Child(user.UserId);

        string json = JsonUtility.ToJson(data);
        Debug.Log(json);

        userInfo.SetRawJsonValueAsync(json);

        // 하지만 하나씩 따로 따로 저장하는 방식이 좋은데 이유는 많은 데이터를 전송하지 않을 수 있어서
        // 속도도 빠르고 데이터를 적게씀


    }
}
