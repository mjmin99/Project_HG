using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using System.Collections.Generic;
/// <summary>
/// 이건 테스트용으로 사용하는 스크립트이기때문에 자세히 볼 필요 전혀 없음!!
/// </summary>
public class CharacterSelectionManager : MonoBehaviour
{
    [System.Serializable]
    public class PlayerSelectUI
    {
        public PlayerController player;  // 적용할 PlayerController
        public Button upButton;             // 캐릭터 증가 버튼
        public Button downButton;           // 캐릭터 감소 버튼

        [HideInInspector] public int currentIndex = 0;
    }

    [Header("3명의 플레이어 선택 UI")]
    public PlayerSelectUI[] playerSelectors;   // 반드시 size=3

    [Header("저장 & 불러오기 버튼")]
    public Button btnSave;
    public Button btnLoad;

    private List<CharacterModelRuntime> characterDB => GameManager.Instance.CharacterDB;

    private void Start()
    {
        // 초기 파티 상태 불러오기
        for (int i = 0; i < playerSelectors.Length; i++)
        {
            int index = i;  // 버그 방지용

            // 저장된 파티 정보
            int savedID = SaveManager.Instance.CurrentData.partySet[index];
            if (savedID < 0 || savedID >= characterDB.Count)
                savedID = 0;

            playerSelectors[index].currentIndex = savedID;

            // 플레이어 모델 적용
            playerSelectors[index].player.ApplyModel(characterDB[savedID]);

            // 버튼 이벤트 등록
            playerSelectors[index].upButton.onClick.AddListener(() => ChangeCharacter(index, +1));
            playerSelectors[index].downButton.onClick.AddListener(() => ChangeCharacter(index, -1));
        }

        // 저장 버튼
        btnSave.onClick.AddListener(SaveParty);

        // 불러오기 버튼
        btnLoad.onClick.AddListener(LoadParty);
    }

    // ===========================================================
    // 캐릭터 변경 (up/down 버튼에서 호출)
    //  - direction : +1 → 다음 캐릭터
    //                -1 → 이전 캐릭터
    // ===========================================================
    private void ChangeCharacter(int playerIndex, int direction)
    {
        var ui = playerSelectors[playerIndex];

        ui.currentIndex += direction;

        if (ui.currentIndex >= characterDB.Count)
            ui.currentIndex = 0;
        if (ui.currentIndex < 0)
            ui.currentIndex = characterDB.Count - 1;

        // 플레이어 모델 변경
        ui.player.ApplyModel(characterDB[ui.currentIndex]);

        Debug.Log($"Player {playerIndex} 캐릭터 변경: {ui.currentIndex}");
    }



    // ===========================================================
    // 파티 저장
    //  - SaveData.partySet[] 업데이트 후 Firebase에 업로드
    // ===========================================================
    private void SaveParty()
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;

        for (int i = 0; i < playerSelectors.Length; i++)
        {
            SaveManager.Instance.CurrentData.partySet[i] = playerSelectors[i].currentIndex;
        }

        SaveManager.Instance.SaveToFirebase(user.UserId);

        Debug.Log("파티 저장 완료!");
    }



    // ===========================================================
    // Firebase에서 파티 불러오기
    //  - LoadFromFirebase 완료 후 UI + PlayerController에 반영
    // ===========================================================

    private void LoadParty()
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;

        SaveManager.Instance.LoadFromFirebase(user.UserId, () =>
        {
            for (int i = 0; i < playerSelectors.Length; i++)
            {
                int id = SaveManager.Instance.CurrentData.partySet[i];

                if (id == -1) id = 0;

                playerSelectors[i].currentIndex = id;
                playerSelectors[i].player.ApplyModel(characterDB[id]);
            }

            Debug.Log("파티 불러오기 완료!");
        });
    }
}
