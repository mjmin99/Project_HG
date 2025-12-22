using UnityEngine;

// List로 TimeInfo를 다루기에는
// RemoveAt(0)이 자주 호출 되기 때문에
// 직접 원형 버퍼(Circular buffer) 구현
public class TimeRecorder
{
    private readonly TestTimeInfo[] buffer;
    private int head;    // 다음에 기록할 위치 (가장 최신 데이터의 다음 칸)
    private int count;   // 현재 저장된 데이터 개수
    private readonly int capacity; // 최대 저장 개수

    // 생성자: 미리 고정된 크기의 배열을 생성 (메모리 재할당 없음)
    public TimeRecorder(float recordTime, float fixedDeltaTime)
    {
        capacity = Mathf.CeilToInt(recordTime / fixedDeltaTime); // 올림 함수. 3.9 => 4
        buffer = new TestTimeInfo[capacity];
        head = 0;
        count = 0;
    }

    // 기록하기 (Push) - O(1)
    public void Record(Vector3 position, float hp, float shield)
    {
        buffer[head].position = position;
        buffer[head].hp = hp;
        buffer[head].shield = shield;
        
        // 원형으로 돌기 위해 모듈러 연산 (%) 사용
        // head가 capacity - 1 과 일치한다는 것은 꽉찼다는 뜻
        head = (head + 1) % capacity; 

        if (count < capacity)
        {
            count++;
        }
    }

    // 되감기 데이터 꺼내기 (Pop) - O(1)
    public TestTimeInfo Pop()
    {
        if (count == 0) return default;

        // head는 '다음'을 기록할 위치이므로, 최신 데이터는 head - 1에 있음
        // head가 0일 경우, 배열의 끝(_capacity - 1)으로 돌아가야 함
        head--;
        if (head < 0) head = capacity - 1;

        count--;
        return buffer[head];
    }
    
    // 가장 최신 데이터 확인 (삭제 안 함) - 보간용
    public TestTimeInfo Peek()
    {
        if (count == 0) return default;

        int index = head - 1;
        if (index < 0) index = capacity - 1;

        return buffer[index];
    }

    public bool HasHistory() => count > 0;
    
    public void Clear()
    {
        head = 0;
        count = 0;
    }
}