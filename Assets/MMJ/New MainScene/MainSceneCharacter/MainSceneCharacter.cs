using System.Collections;
using UnityEngine;

public class MainSceneCharacter : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float minX = -17f;
    [SerializeField] private float maxX = 17f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Speech")]
    [SerializeField] private GameObject speechBubblePrefab;
    [SerializeField] private Transform bubbleAnchor; // 머리 위 위치
    [SerializeField] private float talkDuration = 2f;

    [Header("Roam Timing")]
    [SerializeField] private Vector2 runTimeRange = new Vector2(2f, 5f); // 몇 초 동안 걷을지
    [SerializeField] private Vector2 idleTimeRange = new Vector2(1f, 3f); // 몇 초 멈출지

    [Header("Debug")]
    [SerializeField] private bool debugPingPong = false;

    [Header("Dialogue")]
    [SerializeField] private CharacterDialogueData dialogueData;

    private float stateTimer = 0f;
    private float currentStateDuration = 0f;

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
        if (debugPingPong)
        {
            UpdatePingPong();
            return;
        }

        // 기존 로직 (랜덤 Idle / Run)
        UpdateRoam();
    }


    // ==============================
    // FSM
    // ==============================

    private void ChangeState(MainSceneCharacterState newState)
    {
        if (state == newState)
            return;

        state = newState;

        animator.SetBool("IsRun", state == MainSceneCharacterState.Run);

        // 상태별 체류 시간 설정
        stateTimer = 0f;

        if (state == MainSceneCharacterState.Run)
        {
            currentStateDuration = Random.Range(runTimeRange.x, runTimeRange.y);
        }
        else // Idle
        {
            currentStateDuration = Random.Range(idleTimeRange.x, idleTimeRange.y);
        }
    }

    // ==============================
    // Move Logic
    // ==============================

    private void UpdateMove()
    {
        Vector3 pos = transform.position; // 이 부분이 월드 기준이라 캐릭터들이 약간 제대로 못 다니는 느낌이 났었음. 배경 기준 로컬 좌표로 수정
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

    public void SetDirection(int dir)
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
        string text = GetDialogue();
        ShowSpeechBubble(text);

        // 기존 코루틴 정리
        if (talkCoroutine != null)
            StopCoroutine(talkCoroutine);

        talkCoroutine = StartCoroutine(TalkRoutine());
    }

    private void ShowSpeechBubble(string text)
    {
        if (currentBubble != null)
            Destroy(currentBubble);

        currentBubble = Instantiate(
            speechBubblePrefab,
            bubbleAnchor.position,
            Quaternion.identity,
            bubbleAnchor
        );

        var bubble = currentBubble.GetComponent<SpeechBubble>();
        bubble.SetText(text);
    }

    private IEnumerator TalkRoutine()
    {
        yield return new WaitForSeconds(talkDuration);

        if (currentBubble != null)
            Destroy(currentBubble);

        StartMove();
    }

    private void UpdateRoam()
    {
        stateTimer += Time.deltaTime;

        if (state == MainSceneCharacterState.Run)
        {
            UpdateMove();

            if (stateTimer >= currentStateDuration)
            {
                if (Random.value < 0.4f)
                    ChangeState(MainSceneCharacterState.Idle);
                else
                    ResetRunTimer();
            }
        }
        else // Idle
        {
            if (stateTimer >= currentStateDuration)
                ChangeState(MainSceneCharacterState.Run);
        }
    }


    private void ResetRunTimer()
    {
        stateTimer = 0f;
        currentStateDuration = Random.Range(runTimeRange.x, runTimeRange.y);
    }

    private string GetDialogue()
    {
        if (dialogueData == null)
            return "...";

        return dialogueData.GetRandomDialogue();
    }


    // 테스트용

    [SerializeField] private float testMinX = -17f;
    [SerializeField] private float testMaxX = 17f;
    private void UpdatePingPong()
    {
        // 항상 달리는 상태
        if (state != MainSceneCharacterState.Run)
            ChangeState(MainSceneCharacterState.Run);

        Vector3 pos = transform.position;
        pos.x += moveDir * moveSpeed * Time.deltaTime;

        if (pos.x >= testMaxX)
        {
            pos.x = testMaxX;
            SetDirection(-1);
        }
        else if (pos.x <= testMinX)
        {
            pos.x = testMinX;
            SetDirection(1);
        }

        transform.position = pos;
    }

}
