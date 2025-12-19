using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private bool isClear = false;

    private StageDataSO stageData;
    /// <summary>
    /// 1. 캐릭터 생성
    /// 2. StageData 불러오기
    /// 3. StageData 기준으로 에너미 및 웨이브 생성
    /// 4. 맵 생성(스테이지 키 값을 가지고 맵 데이터 불러옴) - 스크롤링 기법 적용
    /// 5. 생성 포인트에 웨이브에 맞춰 적 생성
    /// 6. 생성 포인트에 플레이어 생성 및 전투 진행(클리어 시 까지)
    /// 7. 적 및 플레이어의 행동(생성, 사망)을 저장하는 Stack 컬렉션 생성(시간 되감기용)
    /// </summary>
    // 외부 스테이지 셀렉트 UI 에서 수행되는 함수
    
    // TODO : 테스트. 버튼으로 해당 함수 수행. 실제로는 OnEnable이나 Start에서 수행
    public void StartStage(string stageKey)
    {
        // Manager.Enemy.CreateEnemy(string 현재 스테이지);
        // 스테이지에 소환될 에너미들 생성하여 wave 단위로 컨트롤러들을 저장함
        // stageData = 
        // Manager.Character. model과 instances로 CharacterController 생성
    }
    
    public void StageClear() // 스테이지 클리어 시 세이브 데이터에 클리어 정보 저장
    {
        
    }

    public void RewindTime() // 시간 되감기 스킬
    {
        
    }
}
