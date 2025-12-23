using UnityEngine;

public class BackgroundDragController : MonoBehaviour
{
    [Header("Drag")]
    [SerializeField] private float dragSpeed = 0.01f;

    [Header("Limit")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    private Vector3 lastMousePos;
    private bool isDragging;

    void Update()
    {
        HandleDrag();
    }

    private void HandleDrag()
    {
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

        float moveX = delta.x * dragSpeed;

        Vector3 pos = transform.position;
        pos.x += moveX;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);

        transform.position = pos;
    }
}
