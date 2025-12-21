using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 캐릭터 인스턴스를 받아서 AbilityRunner 생성
    // 슬롯 기반 AbilityRunner 생성
    public AbilityRunner CreateRunnerFor(ICombatActor actor, CharacterInstance character)
    {
        var runner = new AbilityRunner(actor);

        if (character == null || character.abilitySlots == null)
            return runner;

        foreach (var slot in character.abilitySlots)
        {
            // 빈 슬롯은 무시
            if (slot.ability == null)
                continue;

            var ability = AbilityFactory.Create(
                slot.ability.abilityId,
                slot.ability.rarity
            );

            if (ability == null)
                continue;

            runner.Set.Add(ability, actor);
        }

        return runner;
    }
}
