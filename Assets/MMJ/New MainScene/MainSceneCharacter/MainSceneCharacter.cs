using System.Collections;
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

    [Header("Speech")]
    [SerializeField] private GameObject speechBubblePrefab;
    [SerializeField] private Transform bubbleAnchor; // 머리 위 위치
    [SerializeField] private float talkDuration = 2f;

    private GameObject currentBubble;
    private Coroutine talkCoroutine;

    private MainSceneCharacterState state = MainSceneCharacterState.Idle;
    private int moveDir = 1;

    private void Start()
    {
        ChangeState(MainSceneCharacterState.Run);
    }

    private void Update()
    {
        if (state == MainSceneCharacterState.Run)
            UpdateMove();
    }

    // ==============================
    // FSM
    // ==============================

    private void ChangeState(MainSceneCharacterState newState)
    {
        if (state == newState)
            return;

        state = newState;

        // Animator는 상태를 "표현"만 함
        animator.SetBool("IsRun", state == MainSceneCharacterState.Run);
    }

    // ==============================
    // Move Logic
    // ==============================

    private void UpdateMove()
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
    }

    private void SetDirection(int dir)
    {
        moveDir = dir;
        spriteRenderer.flipX = moveDir < 0;
    }

    // ==============================
    // External Control
    // ==============================

    public void StartMove()
    {
        ChangeState(MainSceneCharacterState.Run);
    }

    public void StopMove()
    {
        ChangeState(MainSceneCharacterState.Idle);
    }

    // ==============================
    // Click
    // ==============================

    private void OnMouseDown()
    {
        // 이동 멈춤
        StopMove();

        // 말풍선 표시
        ShowSpeechBubble();

        // 기존 코루틴 정리
        if (talkCoroutine != null)
            StopCoroutine(talkCoroutine);

        talkCoroutine = StartCoroutine(TalkRoutine());
    }

    private void ShowSpeechBubble()
    {
        if (currentBubble != null)
            Destroy(currentBubble);

        currentBubble = Instantiate(
            speechBubblePrefab,
            bubbleAnchor.position,
            Quaternion.identity,
            bubbleAnchor
        );
    }

    private IEnumerator TalkRoutine()
    {
        yield return new WaitForSeconds(talkDuration);

        if (currentBubble != null)
            Destroy(currentBubble);

        StartMove();
    }
}
