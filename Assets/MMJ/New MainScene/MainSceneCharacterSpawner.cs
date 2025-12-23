using UnityEngine;
using System.Collections.Generic;

public class MainSceneCharacterSpawner : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private Transform characterParent;
    [SerializeField] private Vector2 spawnXRange = new Vector2(-4f, 4f);
    [SerializeField] private float spawnY = 0f;
    [SerializeField] private float spacing = 1.5f;

    private void Start()
    {
        SpawnOwnedCharacters();
    }

    private void SpawnOwnedCharacters()
    {
        var models = CharacterManager.Instance.models;
        var instances = CharacterManager.Instance.instances;

        float currentX = spawnXRange.x;

        foreach (var pair in instances)
        {
            CharacterInstance inst = pair.Value;

            if (!inst.isOwned)
                continue;

            if (!models.TryGetValue(inst.id, out var model))
                continue;

            if (model.prefab == null)
            {
                Debug.LogWarning($"[Spawner] 캐릭터 {inst.id} prefab 없음");
                continue;
            }

            Vector3 pos = new Vector3(currentX, spawnY, 0f);

            GameObject obj = Instantiate(model.prefab, pos, Quaternion.identity, characterParent);

            // MainSceneCharacter 붙어있는지 확인
            var mainChar = obj.GetComponent<MainSceneCharacter>();
            if (mainChar == null)
            {
                Debug.LogError($"[Spawner] prefab에 MainSceneCharacter 없음: {model.characterName}");
            }

            currentX += spacing;

            // 화면 밖으로 나가면 다시 왼쪽부터
            if (currentX > spawnXRange.y)
                currentX = spawnXRange.x;
        }
    }
}
