using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using NUnit.Framework;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    // Firebase에서 저장/로드한 SaveData(CurrentData)에 접근하기 위해 사용
    public SaveManager saveManager;

    // PlayerController는 ApplyModel()을 통해 캐릭터 모델을 변경함
    public PlayerController[] players;

    // 전역 접근용 싱글톤
    public static GameManager Instance;

    // 전체 캐릭터 데이터베이스(CharacterModelRuntime 리스트)
    // - CSV에서 로드한 결과가 들어있음
    public List<CharacterModelRuntime> CharacterDB => characterDB;
    private List<CharacterModelRuntime> characterDB;

    // 플레이어가 생성될 위치(optional)
    public Transform[] playerSpawnPoint;

    private void Awake()
    {
        Instance = this; // 싱글톤 설정

        // Firebase에서 불러온 SaveData.partySet 정보를 기반으로
        // 현재 파티를 PlayerController에게 적용
        characterDB = CharacterCSVLoader.LoadFromCSV();
    }

    private void Start()
    {
        LoadParty();
    }

    /// 저장된 파티 세팅을 기반으로 각 PlayerController에게 캐릭터 모델을 적용하는 함수
    /// SaveManager.CurrentData.partySet 배열을 확인
    /// -1이면 빈 슬롯 → ClearModel()
    /// 유효한 캐릭터 ID이면 characterDB에서 해당 모델을 찾아 ApplyModel()
    private void LoadParty()
    {
        // 디버그용 로그
        // Debug.Log("SaveManager.Instance = " + SaveManager.Instance);
        // Debug.Log("CurrentData = " + SaveManager.Instance?.CurrentData);

        // Firebase에서 가져온 현재 파티 정보
        int[] party = SaveManager.Instance.CurrentData.partySet;

        // players[] 배열의 순서대로 파티 모델을 적용
        for (int i = 0; i < players.Length; i++)
        {
            int id = party[i];

            if (id == -1)
            {
                // -1 자리는 빈 슬롯으로 처리
                players[i].ClearModel();    
                continue;
            }

            // characterDB에서 캐릭터 ID로 Runtime 모델 찾기
            var model = characterDB.Find(c => c.id == id);

            // PlayerController가 해당 모델로 캐릭터를 재구성
            players[i].ApplyModel(model);
        }
    }
}

