using TMPro;
using UnityEngine;

public class CharacterStatusView : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text hpText;
    [SerializeField] TMP_Text adText;

    public void UpdateView(CharacterModel model)
    {
        if (model == null) return;

        nameText.text = model.name;
        hpText.text = $"HP : {model.maxHP}";
        adText.text = $"AD : {model.attack}";
    }
}
