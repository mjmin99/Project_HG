using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MainSceneCharacterSpawner : MonoBehaviour
{
    [Header("Spawn Range")]
    [SerializeField] private float spawnMinX = -17f;
    [SerializeField] private float spawnMaxX = 17f;
    [SerializeField] private float spawnY = 0f;

    [Header("Spawn Spacing")]
    [SerializeField] private float minSpacing = 1.5f;

    [Header("Spawn Feel")]
    [Range(0f, 1f)]
    [SerializeField] private float idleSpawnChance = 0.3f; // 처음에 멈춰 있을 확률

    [SerializeField] private Transform characterRoot;

    // 어드레서블 수정 중
    private MainSceneCharacterLoader loader;

    private readonly List<float> usedXPositions = new List<float>();

    private void Start()
    {
        loader = new MainSceneCharacterLoader();
        // 메인씬 캐릭터 스폰은 연출용이므로 await하지 않음
        SpawnOwnedCharacters().Forget();
    }

    // ==============================
    // Main
    // ==============================

    private void OnEnable()
    {
        SaveManager.OnCharacterAcquired += HandleCharacterAcquired;
    }

    private void OnDisable()
    {
        SaveManager.OnCharacterAcquired -= HandleCharacterAcquired;
    }

    // todo 어드레서블 사용하기전 버전
    // private void SpawnOwnedCharacters()
    // {
    //     var models = Manager.Character.models;
    //     var instances = Manager.Character.instances;
    // 
    //     foreach (var pair in instances)
    //     {
    //         CharacterInstance inst = pair.Value;
    // 
    //         if (!inst.isOwned)
    //             continue;
    // 
    //         if (!models.TryGetValue(inst.id, out var model))
    //             continue;
    // 
    //         if (model.prefab == null)
    //         {
    //             Debug.LogWarning($"[Spawner] prefab 없음 : {model.characterName}");
    //             continue;
    //         }
    // 
    //         float spawnX = GetRandomSpawnX();
    //         Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);
    // 
    //         GameObject obj = Instantiate(
    //             model.prefab,
    //             spawnPos,
    //             Quaternion.identity,
    //             characterRoot
    //         );
    // 
    //         ApplyInitialFeel(obj.GetComponent<MainSceneCharacter>());
    //     }
    // }

    private async UniTask SpawnOwnedCharacters()
    {
        var models = Manager.Character.models;
        var instances = Manager.Character.instances;

        foreach (var pair in instances)
        {
            CharacterInstance inst = pair.Value;
            if (!inst.isOwned)
                continue;

            if (!models.TryGetValue(inst.id, out var model))
                continue;

            float spawnX = GetRandomSpawnX();
            Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);

            string address = $"Characters/{model.characterName}";

            var obj = await loader.Spawn(
                address,
                spawnPos,
                characterRoot
            );

            ApplyInitialFeel(obj.GetComponent<MainSceneCharacter>());
        }
    }

    // ==============================
    // Position
    // ==============================

    private float GetRandomSpawnX()
    {
        for (int i = 0; i < 10; i++)
        {
            float x = Random.Range(spawnMinX, spawnMaxX);

            bool tooClose = false;
            foreach (float used in usedXPositions)
            {
                if (Mathf.Abs(used - x) < minSpacing)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                usedXPositions.Add(x);
                return x;
            }
        }

        // fallback (겹쳐도 그냥 배치)
        float fallback = Random.Range(spawnMinX, spawnMaxX);
        usedXPositions.Add(fallback);
        return fallback;
    }

    // ==============================
    // Initial Feel
    // ==============================

    private void ApplyInitialFeel(MainSceneCharacter character)
    {
        if (character == null)
            return;

        // 랜덤 방향 설정
        int dir = Random.value < 0.5f ? -1 : 1;
        character.SetDirection(dir);

        // 랜덤 상태 (이미 걷거나 / 멈춰있던 느낌)
        if (Random.value < idleSpawnChance)
        {
            character.StopMove();   // Idle
        }
        else
        {
            character.StartMove();  // Run
        }
    }

    // todo 어드레서블 사용하기 전 함수
    // private void HandleCharacterAcquired(CharacterInstance inst)
    // {
    //     var model = Manager.Character.models[inst.id];
    //     if (model == null || model.prefab == null)
    //         return;
    // 
    //     float spawnX = GetRandomSpawnX();
    //     Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);
    // 
    //     GameObject obj = Instantiate(
    //         model.prefab,
    //         spawnPos,
    //         Quaternion.identity,
    //         characterRoot
    //     );
    // 
    //     ApplyInitialFeel(obj.GetComponent<MainSceneCharacter>());
    // }

    // 어드레서블 사용 중
    private async void HandleCharacterAcquired(CharacterInstance inst)
    {
        var model = Manager.Character.models[inst.id];
        if (model == null)
            return;

        float spawnX = GetRandomSpawnX();
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);

        string address = $"Characters/{model.characterName}";

        var obj = await loader.Spawn(
            address,
            spawnPos,
            characterRoot
        );

        ApplyInitialFeel(obj.GetComponent<MainSceneCharacter>());
    }

    private void OnDestroy()
    {
        loader?.ReleaseAll();
    }
}
