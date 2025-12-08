using System.IO;
using Firebase.Database;
using Firebase.Extensions;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 유저별 세이브 데이터를 클라우드(Firebase)에 저장/로드하는 전담 관리자
/// </summary>
public class SaveManager : MonoBehaviour
{
    // 전역에서 접근 가능한 싱글톤 인스턴스
    public static SaveManager Instance;

    // 현재 메모리에 로드되어 있는 유저의 세이브 데이터
    // - 게임 내 어디서든 SaveManager.Instance.CurrentData 로 접근
    public SaveData CurrentData { get; private set; }

    // Firebase Realtime Database의 루트 레퍼런스
    private DatabaseReference db;

    private void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Firebase DB의 루트 노드 레퍼런스 획득
            db = FirebaseDatabase.DefaultInstance.RootReference;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // ==============================================================
    // Firebase에서 유저의 세이브 데이터를 불러오는 함수
    // - userId : Firebase Auth에서 받은 UserId
    // - onComplete : 로드가 끝난 뒤 호출할 콜백 (MainScene 전환 등)
    // ==============================================================
    public void LoadFromFirebase(string userId, System.Action onComplete)
    {
        // "users/{userId}/saveData" 경로에서 데이터 읽어오기
        db.Child("users").Child(userId).Child("saveData") 
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("파이어베이스 로드 오류 -> 신규 Save 생성");
                    CurrentData = new SaveData();
                    SaveToFirebase(userId);
                    onComplete?.Invoke();
                    return;
                }

                DataSnapshot snap = task.Result;

                if (snap.Exists)
                {
                    string json = snap.GetRawJsonValue();
                    CurrentData = JsonUtility.FromJson<SaveData>(json);
                }
                else 
                {
                    // 신규유저
                    CurrentData = new SaveData();
                    SaveToFirebase(userId);
                }
                onComplete?.Invoke();
            });
    }



    // ==============================================================
    // 현재 메모리의 SaveData(CurrentData)를 Firebase에 저장하는 함수
    // - userId : Firebase Auth UserId
    // ==============================================================
    public void SaveToFirebase(string userId)
    {
        // SaveData → JSON 직렬화
        string json = JsonUtility.ToJson(CurrentData);

        // "users/{userId}/saveData" 노드에 JSON 그대로 저장
        db.Child("users").Child(userId).Child("saveData")
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogError("세이브 데이터 저장 실패");
                }
                Debug.Log("세이브 데이터 저장 성공");
            });
    }
}
