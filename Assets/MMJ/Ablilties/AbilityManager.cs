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
    public AbilityRunner CreateRunnerFor(ICombatActor actor, CharacterInstance character)
    {
        var runner = new AbilityRunner(actor);

        if (character.abilities == null)
            return runner;

        foreach (var inst in character.abilities)
        {
            if (!inst.isUnlocked)
                continue;

            var ability = AbilityFactory.Create(inst.abilityId, inst.rarity);
            if (ability == null)
                continue;

            runner.Set.Add(ability, actor);
        }

        return runner;
    }
}
