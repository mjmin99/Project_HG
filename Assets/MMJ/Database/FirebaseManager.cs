using UnityEngine;
using Firebase.Extensions;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UniRx;

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

    public static BoolReactiveProperty IsInitialized { get; private set; } = new (false);

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
        Debug.Log("[FirebaseManager] 초기화 시작...");

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("[FirebaseManager] 설정 완료!");
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                database = FirebaseDatabase.DefaultInstance;

                IsInitialized.Value = true;
            }
            else
            {
                Debug.LogError($"[FirebaseManager] 초기화 실패: {dependencyStatus}");
                app = null;
                auth = null;
                database = null;

                IsInitialized.Value = false;
            }
        });
    }
}