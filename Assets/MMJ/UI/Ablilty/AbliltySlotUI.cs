using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilitySlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;

    public void SetEmpty()
    {
        icon.enabled = false;
        nameText.text = "Empty";
    }

    public void SetAbility(AbilityInstance inst)
    {
        var ability = AbilityFactory.Create(inst.abilityId, inst.rarity);
        if (ability == null)
        {
            SetEmpty();
            return;
        }

        icon.enabled = true;
        icon.sprite = AbilityIconProvider.GetIcon(inst.abilityId);
        nameText.text = ability.Name;
    }
}
