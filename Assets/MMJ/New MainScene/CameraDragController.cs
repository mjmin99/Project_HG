using UnityEngine;
using UnityEngine.EventSystems;

public class CameraDragController : MonoBehaviour
{
    [Header("Drag")]
    [SerializeField] private float dragSpeed = 0.01f;

    [Header("Limit (World X)")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;

    private Vector3 lastMousePos;
    private bool isDragging;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        HandleDrag();
    }

    private void HandleDrag()
    {
        //UI 위에 있으면 드래그 막음
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            isDragging = false;
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (!isDragging)
            return;

        Vector3 delta = Input.mousePosition - lastMousePos;
        lastMousePos = Input.mousePosition;

        // 마우스를 오른쪽으로 끌면 화면은 왼쪽으로 이동
        float moveX = -delta.x * dragSpeed;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x + moveX, minX, maxX);
        transform.position = pos;
    }
}
