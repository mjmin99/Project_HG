using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("모델 붙일 위치")]
    [SerializeField] private Transform modelRoot;

    [Header("이동 속도")]
    [SerializeField] private float moveSpeed = 5;

    [Header("총알 발사 관련")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform muzzlePoint;

    [Header("UI")]
    [SerializeField] private CharacterStatusView statusView;

    private GameObject currentModelInstance;
    private CharacterModel currentModel;
    private int currentHP;



    private void Update()
    {
        HandleMove();
        HandleShoot();
        
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


    public void ApplyModel(CharacterModel model)
    {
        currentModel = model;

        // 기존 모델 제거
        if (currentModelInstance != null)
            Destroy(currentModelInstance);

        // 새 모델 생성
        currentModelInstance = Instantiate(model.prefab, modelRoot);

        // 스탯 적용
        currentHP = model.maxHP;

        // UI 업데이트
        statusView?.UpdateView(model);
    }
}
