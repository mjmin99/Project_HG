using UnityEngine;

public class MainSceneCharacter : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float minX = -3f;
    [SerializeField] private float maxX = 3f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    private int moveDir = 1; // 1 = 오른쪽, -1 = 왼쪽
    private bool isPaused = false;

    private void Update()
    {
        if (isPaused)
            return;

        Move();
    }

    private void Move()
    {
        Vector3 pos = transform.position;
        pos.x += moveDir * moveSpeed * Time.deltaTime;

        if (pos.x >= maxX)
        {
            pos.x = maxX;
            SetDirection(-1);
        }
        else if (pos.x <= minX)
        {
            pos.x = minX;
            SetDirection(1);
        }

        transform.position = pos;

        UpdateVisual();
    }

    private void SetDirection(int dir)
    {
        moveDir = dir;
    }

    private void UpdateVisual()
    {
        // 좌우 반전
        spriteRenderer.flipX = moveDir < 0;

        // 애니메이션 파라미터 (있을 경우)
        if (animator != null)
        {
            animator.SetBool("IsMoving", true);
            animator.SetFloat("MoveDir", moveDir);
        }
    }

    // === 외부 제어용 ===

    public void PauseMove()
    {
        isPaused = true;

        if (animator != null)
            animator.SetBool("IsMoving", false);
    }

    public void ResumeMove()
    {
        isPaused = false;

        if (animator != null)
            animator.SetBool("IsMoving", true);
    }
}
