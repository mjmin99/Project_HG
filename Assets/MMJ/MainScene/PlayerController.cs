using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("캐릭터 모델 위치")]
    [SerializeField] private Transform modelRoot;

    [Header("UI")]
    [SerializeField] private CharacterStatusView statusUI;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    private GameObject currentModelObject;
    private int characterId = -1;
    private CharacterStats stats;

    private float attackTimer = 0f;

    private void Update()
    {
        HandleMove();
        HandleAutoAttack();
    }

    //  캐릭터 적용
    public void ApplyCharacter(int id, CharacterStats stats)
    {
        this.characterId = id;
        this.stats = stats;

        // 기존 모델 제거
        if (currentModelObject != null)
        {
            Destroy(currentModelObject);
        }

        // 모델 불러오기
        var model = Manager.Character.models[id];
        if (model.prefab != null)
        {
            currentModelObject = Instantiate(model.prefab, modelRoot);
        }

        // UI 갱신
        statusUI?.UpdateView(model.characterName, stats.hp, stats.attack);

        Debug.Log($"플레이어에 캐릭터 적용됨: {model.characterName}, HP={stats.hp}, ATK={stats.attack}");
    }



    //  캐릭터 제거 (파티 슬롯 비어있을 때)
    public void ClearCharacter()
    {
        characterId = -1;
        stats = new CharacterStats();

        if (currentModelObject != null)
            Destroy(currentModelObject);

        statusUI?.UpdateView("Empty", 0, 0);
    }



    //  이동 처리 테스트로 만들었던 것
    private void HandleMove()
    {
        if (characterId < 0) return; // 캐릭터 없으면 이동 금지

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0f, v).normalized;

        if (dir.sqrMagnitude > 0.01f)
        {
            transform.position += dir * moveSpeed * Time.deltaTime;
            transform.forward = dir;
        }
    }



    //  자동 공격 처리 (기본 구조)
    private void HandleAutoAttack()
    {
        if (characterId < 0) return;

        attackTimer += Time.deltaTime;

        float attackInterval = 1f / stats.attackSpeed;

        // if (attackTimer >= attackInterval)
        // {
        //     attackTimer = 0f;
        // 
        //     // 실제 공격 로직은 CombatCharacter 추가 후 작성 예정 여기 말고 전투땐 전투 캐릭터 플레이어1 로 만들 수도 있음.
        //     Debug.Log($"자동 공격 ! (ATK = {stats.attack})");
        // }
    }
}
