using UnityEngine;
using Firebase.Extensions;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class FirebaseManager : MonoBehaviour
{
    private static FirebaseManager instance;
    public static FirebaseManager Instance { get { return instance; } }

    private static FirebaseApp app;
    public static FirebaseApp App { get { return app; } }

    private static FirebaseAuth auth;
    public static FirebaseAuth Auth { get { return auth; } }

    private static FirebaseDatabase database;
    public static FirebaseDatabase Database { get { return database; } }

    public static bool IsInitialized { get; private set; } = false;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        { 
            Destroy(gameObject);
        }
    }
    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Debug.Log("파이어베이스 설정 완료");
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                database = FirebaseDatabase.DefaultInstance;

                IsInitialized = true;
            }
            else
            {
                Debug.LogError($"Firebase 초기화 실패: {dependencyStatus}");
                IsInitialized = false;
            }
        });
    }
}
