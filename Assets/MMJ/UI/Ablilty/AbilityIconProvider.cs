using UnityEngine;

public static class AbilityIconProvider
{
    public static Sprite GetIcon(int abilityId)
    {
        // 예: Resources/AbilityIcons/1001.png
        return Resources.Load<Sprite>($"AbilityIcons/{abilityId}");
    }
}
