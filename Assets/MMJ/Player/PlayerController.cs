using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("캐릭터 모델들")]
    [SerializeField] private List<CharacterModel> characterModels;

    [Header("모델 붙일 위치")]
    [SerializeField] private Transform modelroot;

    [Header("이동 속도")]
    [SerializeField] private float moveSpeed = 5;

    [Header("총알 발사 관련")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzlePoint;

    [Header("UI")]
    [SerializeField] private CharacterStatusView statusView;

    private int currentIndex = 0;
    private GameObject currentModelInstance;

    private void Start()
    {
        if (characterModels.Count > 0)
        {
            if (characterModels.Count > 0)
            {
                ApplyCharacter(0); // 첫 캐릭터로 시작
            }
        }
    }

    private void Update()
    {
        HandleMove();
        HandleShoot();
        HandleChangeCharacter();
    }

    private void HandleMove() // 움직임 관련
    {
        float h = Input.GetAxisRaw("Horizontal"); // A,D / 좌,우
        float v = Input.GetAxisRaw("Vertical");   // W,S / 상,하

        Vector3 dir = new Vector3(h, 0f, v).normalized;

        if (dir.sqrMagnitude > 0f)
        {
            transform.position += dir * moveSpeed * Time.deltaTime;
            transform.forward = dir; // 이동 방향으로 바라보기
        }
    }

    private void HandleShoot() // 총알 발사 관련
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (bulletPrefab != null && muzzlePoint != null)
            {
                Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);
            }
        }
    }

    private void HandleChangeCharacter() // 캐릭터 변경 관련
    {
        if (characterModels.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.Z)) // Z 키 누르면 이전 캐릭터
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = characterModels.Count - 1;

            ApplyCharacter(currentIndex);
        }
        else if (Input.GetKeyDown(KeyCode.X)) // X키 누르면 다음 캐릭터
        {
            currentIndex++;
            if (currentIndex >= characterModels.Count)
                currentIndex = 0;

            ApplyCharacter(currentIndex);
        }

    }

    private void ApplyCharacter(int index) // 모델 적용과 UI갱신
    {
        CharacterModel model = characterModels[index];

        if (currentModelInstance != null)
        {
            Destroy(currentModelInstance);
        }

        if (model.characterPrefab != null && modelroot != null)
        {
            currentModelInstance = Instantiate(
                model.characterPrefab,
                modelroot.position,
                modelroot.rotation,
                modelroot
                );
        }

        if (statusView != null) // UI 갱신
        {
            statusView.UadateView(model);
        }

    }


}
