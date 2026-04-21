using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraDragController : MonoBehaviour
{
    [Header("Drag")]
    [SerializeField] private float dragSpeed = 0.01f;

    [Header("Limit (World X)")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;

    private Vector3 lastMousePos;
    private bool isDragging;

    private void Update()
    {
        HandleDrag();
    }
    private void HandleDrag()
    {
        // 1. 현재 활성화된 포인터(마우스 또는 첫 번째 터치) 가져오기
        Pointer currentPointer = Pointer.current;
    
        // 포인터가 없으면(입력이 없으면) 리턴
        if (currentPointer == null) return;

        // 2. UI 위에 있는지 확인
        // New Input System에서는 터치 시 PointerId를 넘겨주는 것이 정확합니다.
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            isDragging = false;
            return;
        }

        // 3. 드래그 시작 (클릭 또는 터치 시작)
        if (currentPointer.press.wasPressedThisFrame)
        {
            isDragging = true;
            lastMousePos = currentPointer.position.ReadValue();
        }

        // 4. 드래그 종료 (클릭 또는 터치 뗌)
        if (currentPointer.press.wasReleasedThisFrame)
        {
            isDragging = false;
        }

        if (!isDragging)
            return;

        // 5. 현재 위치와 이전 위치의 차이 계산
        Vector2 currentPos = currentPointer.position.ReadValue();
        Vector2 delta = currentPos - (Vector2)lastMousePos;
        lastMousePos = currentPos;

        // 마우스를 오른쪽으로 끌면 화면은 왼쪽으로 이동
        float moveX = -delta.x * dragSpeed;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x + moveX, minX, maxX);
        transform.position = pos;
    }
    
    // 인풋 시스템 적용으로 인한 코드 변경. 아래는 인풋 매니저 시절 코드
    // private void HandleDrag()
    // {
    //     //UI 위에 있으면 드래그 막음
    //     if (EventSystem.current != null &&
    //         EventSystem.current.IsPointerOverGameObject())
    //     {
    //         isDragging = false;
    //         return;
    //     }
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         isDragging = true;
    //         lastMousePos = Input.mousePosition;
    //     }
    //
    //     if (Input.GetMouseButtonUp(0))
    //     {
    //         isDragging = false;
    //     }
    //
    //     if (!isDragging)
    //         return;
    //
    //     Vector3 delta = Input.mousePosition - lastMousePos;
    //     lastMousePos = Input.mousePosition;
    //
    //     // 마우스를 오른쪽으로 끌면 화면은 왼쪽으로 이동
    //     float moveX = -delta.x * dragSpeed;
    //
    //     Vector3 pos = transform.position;
    //     pos.x = Mathf.Clamp(pos.x + moveX, minX, maxX);
    //     transform.position = pos;
    // }
}
