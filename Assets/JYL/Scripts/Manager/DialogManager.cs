using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    /// 대화 상황에 맞게 대화 전체 구조체를 딕셔너리로 저장
    /// 매니저에서 함수로 특정 상황에 대한 키를 넣으면, 딕셔너리에서 꺼내와서 수행
    /// 현재 출력중인 텍스트가 음성 재생을 필요로 하는 것인지 판단 필요. 파라매터 bool 추가
    /// 대화 시작 시 Time 스케일 0 으로 변경
    /// 대화 시작 시 대화 UI 패널 오픈
    /// 대화 중, 현재 화자에 맞게 일러스트 스프라이트 하이라이트 효과
    /// 대화에 화자 추가, 대화에 화자 나가는 것을 CSV string으로 판별함
    /// 
    /// 스킵 버튼을 사용하면 대화 종료
    [SerializeField] private string csvPath = "CSV/TestDialog";

    private Dictionary<string, Dialog> dialogs; 
    void Awake()
    {
        _ = GetDialogFromCsv();
    }

    private async UniTask GetDialogFromCsv()
    {
        dialogs = await Util.ParseCsvToDialogs(csvPath);
    }
    
    
    
}
