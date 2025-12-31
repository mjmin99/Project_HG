using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Cysharp.Threading.Tasks;

public class LobbyPanel : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject editPanel;

    [Header("User Info UI")]
    [SerializeField] TMP_Text emailContent;
    [SerializeField] TMP_Text nameContent;
    [SerializeField] TMP_Text userIDContent;

    [Header("Buttons")]
    [SerializeField] Button logoutButton;
    [SerializeField] Button editProfileButton;
    [SerializeField] Button deleteUserButton;
    [SerializeField] Button gameStartButton;

    private void Awake()
    {
        logoutButton.onClick.AddListener(Logout);
        editProfileButton.onClick.AddListener(EditProfile);
        deleteUserButton.onClick.AddListener(DeleteUser);
        gameStartButton.onClick.AddListener(GameStart);
    }

    private void OnEnable()
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;
        emailContent.text = user.Email;
        nameContent.text = user.DisplayName;
        userIDContent.text = user.UserId;
    }

    private async UniTaskVoid CheckFirstRun()
    {
        if (!Manager.Dialog.CheckDialogCondition(DialogCondition.IsFirstRun))
        {
            Debug.Log($"다이얼로그 재생 시작함{DialogCondition.IsFirstRun}");
            await Manager.Dialog.StartDialog(DialogKey.Prologue);
            Manager.Dialog.MarkDialogCondition(DialogCondition.IsFirstRun);
        }
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
        UIManager.Instance.OpenUI<UIPopup>("ConfirmDeleteUserPopup");
    }

    // 옛날 유저 삭제
    // private void DeleteUser()
    // {
    //     FirebaseUser user = FirebaseManager.Auth.CurrentUser;
    //     user.DeleteAsync()
    //         .ContinueWithOnMainThread(task =>
    //         {
    //             if (task.IsCanceled)
    //             {
    //                 Debug.LogError("[LobbyPanel] 유저 삭제 취소됨");
    //                 return;
    //             }
    //             if (task.IsFaulted)
    //             {
    //                 Debug.LogError($"[LobbyPanel] 유저 삭제 실패: {task.Exception}");
    //                 return;
    //             }
    // 
    //             Debug.Log("[LobbyPanel] 유저 삭제 성공");
    //             FirebaseManager.Auth.SignOut();
    //             loginPanel.SetActive(true);
    //             gameObject.SetActive(false);
    //         });
    // }

    private void GameStart()
    {
        StartCoroutine(InitializeGameData());
    }

    private IEnumerator InitializeGameData()
    {
        Debug.Log("[LobbyPanel] ========== 게임 데이터 로딩 시작 ==========");

        gameStartButton.interactable = false;

        // 1단계: CSV 로드
        if (Manager.Character.models.Count == 0)
        {
            Debug.Log("[LobbyPanel] CSV 로드 시작");
            var models = CharacterCSVLoader.Load();

            if (models.Count == 0)
            {
                Debug.LogError("[LobbyPanel] CSV 로드 실패! 게임을 시작할 수 없습니다.");
                ToastUtil.Error("캐릭터 데이터 로드에 실패했습니다.");
                gameStartButton.interactable = true;
                yield break;
            }

            Manager.Character.LoadModels(models);

            // Prefab 연결
            foreach (var model in Manager.Character.models.Values)
            {
                model.prefab = Resources.Load<GameObject>($"Characters/{model.characterName}");
                if (model.prefab == null)
                    Debug.LogWarning($"[LobbyPanel] Prefab not found: {model.characterName}");
            }

            Debug.Log($"[LobbyPanel] CSV 로드 완료: {models.Count}개 캐릭터");
        }
        else
        {
            Debug.Log("[LobbyPanel] CSV 이미 로드되어 있음");
        }

        yield return null;

        // 2단계: Firebase 유저 확인
        if (FirebaseManager.Auth == null)
        {
            Debug.LogError("[LobbyPanel] Firebase Auth가 null입니다!");
            gameStartButton.interactable = true;
            yield break;
        }

        var user = FirebaseManager.Auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("[LobbyPanel] 로그인된 유저가 없습니다!");
            gameStartButton.interactable = true;
            yield break;
        }

        Debug.Log($"[LobbyPanel] 유저 확인 완료: {user.UserId}");
        yield return null;

        // 3단계: Firebase 세이브 로드
        bool loadComplete = false;

        Manager.Save.InitForUser(user.UserId, () =>
        {
            loadComplete = true;
        });

        float timeout = 10f;
        float elapsed = 0f;

        while (!loadComplete && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!loadComplete)
        {
            Debug.LogError("[LobbyPanel] 세이브 로드 타임아웃!");
            gameStartButton.interactable = true;
            yield break;
        }

        Debug.Log("[LobbyPanel] 세이브 로드 완료");
        yield return null;

        // 4단계: 메인씬 이동
        Debug.Log("[LobbyPanel] ========== 로딩 완료! MainScene 이동 ==========");
        SceneChanger.Instance.LoadScene("MainScene");
        // 5단계: 스테이지 세이브 서비스 초기화
        Manager.Game.StageServiceInit();
        _ = CheckFirstRun();
    }
}